using System;
using System.Collections.Generic;

using GOO.Utilities;
using System.Text;

namespace GOO.Model
{
    public class OrdersTracker
    {
        private static OrdersTracker instance;
        
        public List<OrderCounter> CounterList { get; private set; }
        public List<Route> AllRoutes { get; private set; }
        public List<Route> AvailableRoutes { get; private set; }
        
        public double SolutionScore { get; private set; }
        public double PenaltyScore { get; private set; }
        public double TravelTimeScore { get; private set; }

        private List<Tuple<Days, int, List<Route>>> TruckPlanning;

        private OrdersTracker()
        {
            CounterList = new List<OrderCounter>();
            AllRoutes = new List<Route>();
            AvailableRoutes = new List<Route>();
            TruckPlanning = new List<Tuple<Days, int, List<Route>>>();
        }
        
        public static OrdersTracker Instance
        {
            get
            {
                if (instance == null)
                    instance = new OrdersTracker();

                return instance;
            }
        }

        public void InitializeOrdersTracker()
        {
            OrdersTracker instance = OrdersTracker.Instance;
            foreach (Order order in Data.Orders.Values)
            {
                instance.FindOrCreateOrderCounter(order.OrderNumber);
            }
        }

        public void AddNewItemToPlanning(Days day, int truckID, List<Route> routes)
        {
            TruckPlanning.Add(new Tuple<Days, int, List<Route>>(day, truckID, routes));
            foreach (Route route in routes)
                foreach (Order order in route.Orders)
                    this.AddPlannedOccurence(order.OrderNumber, route);
        }

        public void RemoveItemFromPlanning(Days day, int truckID)
        {
            Tuple<Days, int, List<Route>> toRemove = getPlanningForATruck(day, truckID);
            TruckPlanning.Remove(toRemove);
            foreach (Route route in toRemove.Item3)
                foreach (Order order in route.Orders)
                    this.RemovePlannedOccurrence(order.OrderNumber, route);
        }

        public Tuple<Days, int, List<Route>> getPlanningForATruck(Days day, int truckID)
        {
            return this.TruckPlanning.Find(t => t.Item1 == day && t.Item2 == truckID);
        }

        public Tuple<Days, int, List<Route>> getRandomPlanning()
        {
            int random = new Random().Next(TruckPlanning.Count);
            return this.TruckPlanning[random];
        }

        public Tuple<Days, int, List<Route>> getPlanningForRoute(List<Route> route)
        {
            return this.TruckPlanning.Find(t => t.Item3 == route);
        }

        public List<Tuple<Days, int, List<Route>>> getEntirePlanning()
        {
            return this.TruckPlanning;
        }
        
        public void clearTruckPlanning()
        {
            foreach (OrderCounter oc in CounterList)
                oc.PlannedDayOccurences = Days.None;

            foreach(Tuple<Days, int, List<Route>> t in TruckPlanning)
                foreach(Route route in t.Item3)
                    if(!AvailableRoutes.Contains(route))
                        AvailableRoutes.Add(route);
                    
            this.TruckPlanning.Clear();
        }
        
        public void updateOrdersCounterAfterAdding(List<Route> addedRoutes)
        {
            foreach (Route route in addedRoutes)
                foreach (Order order in route.Orders)
                    if (order.OrderNumber != 0)
                        AddOrderOccurrence(order.OrderNumber, route);
        }

        public void updateOrdersCounterAfterRemoval(List<Route> removedRoutes)
        {
            foreach (Route route in removedRoutes)
                foreach (Order order in route.Orders)
                    RemoveOrderOccurrence(order.OrderNumber, route);
        }

        public void ClearAllOccurences() // TODO: How to deal with routes containing the order?
        {
            CounterList.Clear();
            CounterList = new List<OrderCounter>();
        }

        public void AddOrderOccurrence(int OrderNumber, Route OccurredIn)
        {
            OrderCounter counter = this.FindOrCreateOrderCounter(OrderNumber);
            counter.OrderDayOccurrences |= OccurredIn.Day;
            counter.UpdateOrderDayRestrictions();
            if (!counter.PartOfRoutes.Contains(OccurredIn))
                counter.PartOfRoutes.Add(OccurredIn);
        }

        public void AddPlannedOccurence(int OrderNumber, Route OccurredIn)
        {
            OrderCounter counter = this.FindOrCreateOrderCounter(OrderNumber);
            counter.PlannedDayOccurences |= OccurredIn.Day;
        }

        public void RemovePlannedOccurrence(int OrderNumber, Route OccurredIn)
        {
            OrderCounter counter = this.FindOrCreateOrderCounter(OrderNumber);
            counter.OrderDayOccurrences ^= (counter.OrderDayOccurrences & OccurredIn.Day);
        }

        public void RemoveOrderOccurrence(int OrderNumber, Route OccurredIn)
        {
            OrderCounter counter = this.FindOrCreateOrderCounter(OrderNumber);
            counter.OrderDayOccurrences ^= (counter.OrderDayOccurrences & OccurredIn.Day);
            counter.UpdateOrderDayRestrictions();
            counter.PartOfRoutes.Remove(OccurredIn);
             
            // Why remove if all orders need to be added in anyway?
            // CounterList.RemoveAll(o => o.OrderNumber == OrderNumber && o.OrderDayOccurrences.Equals(Days.None));
        }

        public bool CanAddOrder(int OrderNumber, Days day)
        {
            OrderCounter counter = this.FindOrCreateOrderCounter(OrderNumber);
            if (counter.OrderDayOccurrences.HasFlag(day))
                return false; // Already occurred on specified day

            foreach (Days restrictions in counter.OrderDayRestrictions)
                if (restrictions.HasFlag(day))
                    return true; // Order has yet to occur on specified day and can be added accorrding to the restrictions

            return false; // restrictions is either empty or does not have the specified day in its restriction
        }

        public Boolean OrderHasOccurence(Days day, int OrderNumber)
        {
            return CounterList.Find(o => o.OrderNumber == OrderNumber && o.OrderDayOccurrences.HasFlag(day)) != null;
        }

        public Boolean OrderHasPlannedOccurence(Days day, int OrderNumber)
        {
            return CounterList.Find(o => o.OrderNumber == OrderNumber && o.PlannedDayOccurences.HasFlag(day)) != null;
        }

        public Boolean AllOrdersCompleted()
        {
            foreach (OrderCounter counter in CounterList)
                if (!counter.IsCompleted())
                    return false;

            return true;
        }

        public Boolean IsOrderCompleted(int OrderNumber)
        {
            foreach (OrderCounter counter in CounterList)
                if (counter.OrderNumber == OrderNumber)
                    return counter.IsCompleted();

            return false;
        }

        private OrderCounter FindOrCreateOrderCounter(int OrderNumber)
        {
            if (!CounterList.Exists(o => o.OrderNumber == OrderNumber))
                CounterList.Add(new OrderCounter(OrderNumber, new List<Days>(Data.Orders[OrderNumber].DayRestrictions)));

            return CounterList.Find(o => o.OrderNumber == OrderNumber); // CounterList.Add states that it adds to end, maybe return [Count - 1]?
        }

        public double GetSolutionScore() // TODO: Maybe start working with delta's instead of recalculating everytime
        {
            double travelTime = 0.0d;
            double penaltyTime = 0.0d;

            List<int> uncompleteOrders = new List<int>();
            for (int i = 0; i < CounterList.Count; i++)
            {
                if (!CounterList[i].IsCompleted())
                {
                    int orderNumber = CounterList[i].OrderNumber;
                    if (uncompleteOrders.Contains(orderNumber)) // Has already been punished
                        continue;
                    else
                        uncompleteOrders.Add(orderNumber);

                    penaltyTime += Data.Orders[orderNumber].PenaltyTime;
                    break;
                }
            }

            foreach (Tuple<Days, int, List<Route>> tuple in TruckPlanning)
                foreach (Route route in tuple.Item3)
                    travelTime += route.TravelTime;

            return travelTime + penaltyTime;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            foreach (Tuple<Days, int, List<Route>> tuple in TruckPlanning)
            {
                List<Route> routes = tuple.Item3;
                int sequenceID = 0;
                for (int routeID = 0; routeID < routes.Count; routeID++)
                    for (int orderID = 0; orderID < routes[routeID].Orders.Count; orderID++)
                        sb.AppendLine(String.Format("{0};{1};{2};{3}", tuple.Item2 + 1, (int)DayInt(tuple.Item1), ++sequenceID, routes[routeID].Orders[orderID].OrderNumber));
            }

            return sb.ToString();
        }

        private int DayInt(Days day)
        {
            switch (day)
            {
                case Days.Monday:
                    return 1;
                case Days.Tuesday:
                    return 2;
                case Days.Wednesday:
                    return 3;
                case Days.Thursday:
                    return 4;
                case Days.Friday:
                    return 5;
                default:
                    return -1;
            }
        }

        public class OrderCounter
        {
            public int OrderNumber { get; private set; }
            public List<Days> OrderDayRestrictions { get; private set; }
            public List<Route> PartOfRoutes { get; private set; }
            public Days OrderDayOccurrences { get; set; }
            public Days PlannedDayOccurences { get; set; }

            public OrderCounter(int OrderNumber, List<Days> OrderDayRestrictions)
            {
                this.OrderNumber = OrderNumber;
                this.OrderDayRestrictions = OrderDayRestrictions;

                this.OrderDayOccurrences = Days.None;
                this.PartOfRoutes = new List<Route>();
            }

            public Boolean IsCompleted()
            {
                foreach (Days restrictions in OrderDayRestrictions)
                    if (PlannedDayOccurences.Equals(restrictions))
                        return true;

                return false;
            }

            public void UpdateOrderDayRestrictions()
            {
                OrderDayRestrictions.Clear();

                foreach (Days restrictions in Data.Orders[OrderNumber].DayRestrictions)
                    if (restrictions.HasFlag(OrderDayOccurrences))
                        OrderDayRestrictions.Add(restrictions);
            }

        }
    }
}