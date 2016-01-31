using System;
using System.Collections.Generic;

using GOO.Utilities;

namespace GOO.Model
{
    public class OrdersTracker
    {
        public List<OrderCounter> CounterList { get; private set; }
        public List<Route> AllRoutes { get; private set; }

        private static OrdersTracker instance;

        private OrdersTracker()
        {
            CounterList = new List<OrderCounter>();
            AllRoutes = new List<Route>();
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
            if (!CounterList.Exists(o => o.OrderNumber == OrderNumber))
                CounterList.Add(new OrderCounter(OrderNumber, new List<Days>(Data.Orders[OrderNumber].DayRestrictions)));

            foreach (OrderCounter order in CounterList)
                if (order.OrderNumber == OrderNumber)
                {
                    order.OrderDayOccurrences |= OccurredIn.Day;
                    UpdateOrderDayRestrictions(order);
                    if (!order.PartOfRoutes.Contains(OccurredIn))
                        order.PartOfRoutes.Add(OccurredIn);
                    break;
                }
        }

        public void RemoveOrderOccurrence(int OrderNumber, Route OccurredIn)
        {
            foreach (OrderCounter order in CounterList)
                if (order.OrderNumber == OrderNumber)
                {
                    order.OrderDayOccurrences ^= (order.OrderDayOccurrences & OccurredIn.Day);
                    UpdateOrderDayRestrictions(order);
                    order.PartOfRoutes.Remove(OccurredIn);
                    break;
                }

            CounterList.RemoveAll(o => o.OrderNumber == OrderNumber && o.OrderDayOccurrences.Equals(Days.None));
        }

        private void UpdateOrderDayRestrictions(OrderCounter order)
        {
            order.OrderDayRestrictions.Clear();

            foreach (Days restrictions in Data.Orders[order.OrderNumber].DayRestrictions)
                if (restrictions.HasFlag(order.OrderDayOccurrences))
                    order.OrderDayRestrictions.Add(restrictions);
        }

        public bool CanAddOrder(int orderNumber, Days day)
        {
            foreach (OrderCounter order in CounterList)
                if (order.OrderNumber == orderNumber)
                {
                    if (order.OrderDayOccurrences.HasFlag(day))
                        return false; // Already occurred on specified day

                    foreach (Days restrictions in order.OrderDayRestrictions)
                        if (restrictions.HasFlag(day))
                            return true; // Order has yet to occur on specified day and can be added accorrding to the restrictions

                    return false; // restrictions is either empty or does not have the specified day in its restriction
                }

            // Order has yet to be added to the list of CounterList
            foreach (Days restrictions in Data.Orders[orderNumber].DayRestrictions)
                if (restrictions.HasFlag(day))
                    return true; // Order has never been added to this list, but according to the order.DayRestrictions it should be able to add to the specified day

            return false; // Could not be found in the CounterList or the orders restrictions, it is not supposed to be added this day
        }

        public Boolean OrderHasOccurence(Days day, int OrderNumber)
        {
            return CounterList.Find(o => o.OrderNumber == OrderNumber && o.OrderDayOccurrences.HasFlag(day)) != null;
        }

        public Boolean IsCompleted()
        {
            foreach (OrderCounter order in CounterList)
                if (!order.IsCompleted())
                    return false;

            return true;
        }

        public Boolean IsOrderCompleted(int OrderNumber)
        {
            foreach (OrderCounter order in CounterList)
                if (order.OrderNumber == OrderNumber)
                    return order.IsCompleted();

            return false;
        }

        public class OrderCounter
        {
            public int OrderNumber { get; private set; }
            public List<Days> OrderDayRestrictions { get; private set; }
            public List<Route> PartOfRoutes { get; private set; }
            public Days OrderDayOccurrences { get; set; }

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
                    if (OrderDayOccurrences.Equals(restrictions))
                        return true;

                return false;
            }
        }
    }
}