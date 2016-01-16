using System;
using System.Collections.Generic;
using System.Collections;

using GOO.Utilities;
using System.Text;

namespace GOO.Model
{
    public class Solution
    {
        private OrdersCounter ordersCounter;

        private List<ParentCluster> clusters;
        private List<Tuple<Days, int, List<Route>>> truckPlanning;

        public Solution(List<ParentCluster> clusters)
        {
            this.ordersCounter = OrdersCounter.Instance;
            this.clusters = clusters;
            this.truckPlanning = new List<Tuple<Days, int, List<Route>>>();
        }

        public void AddNewItemToPlanning(Days day, int truckID, List<Route> routes)
        {
            this.truckPlanning.Add(new Tuple<Days, int, List<Route>>(day, truckID, routes));
            updateOrdersCounterAfterAdding(routes);
        }

        public void RemoveItemFromPlanning(Days day, int truckID)
        {
            Tuple<Days, int, List<Route>> toRemove = getPlanningForATruck(day, truckID);
            this.truckPlanning.Remove(toRemove);
            updateOrdersCounterAfterRemoval(toRemove.Item3);
        }

        public List<Tuple<Days, int, List<Route>>> getEntirePlanning()
        {
            return this.truckPlanning;
        }

        public void clearTruckPlanning()
        {
            truckPlanning.Clear();
            ordersCounter.ClearAllOccurences();
        }

        public void recountOrdersCounter() // Unused (?)
        {
            ordersCounter.ClearAllOccurences();
            foreach (Tuple<Days, int, List<Route>> tuple in truckPlanning)
                updateOrdersCounterAfterAdding(tuple.Item3);
        }

        private void updateOrdersCounterAfterAdding(List<Route> addedRoutes)
        {
            foreach (Route route in addedRoutes)
                foreach (Order order in route.Orders)
                    if(order.OrderNumber != 0)
                        ordersCounter.AddOccurrence(order.OrderNumber, route.Day);
        }

        private void updateOrdersCounterAfterRemoval(List<Route> removedRoutes)
        {
            foreach (Route route in removedRoutes)
                foreach (Order order in route.Orders)
                    ordersCounter.RemoveOccurrence(order.OrderNumber, route.Day);
        }

        public Tuple<Days, int, List<Route>> getPlanningForATruck(Days day, int truckID)
        {
            return this.truckPlanning.Find(t => t.Item1 == day && t.Item2 == truckID);
        }

        public Tuple<Days, int, List<Route>> getRandomPlanning()
        {
            int random = new Random().Next(truckPlanning.Count);
            return this.truckPlanning[random];
        }

        public Tuple<Days, int, List<Route>> getPlanningForRoute(List<Route> route)
        {
            return this.truckPlanning.Find(t => t.Item3 == route);
        }

        public AbstractCluster getRandomCluster()
        {
            int random = new Random().Next(clusters.Count);
            return this.clusters[random];
        }

        public List<ParentCluster> getAllClusters()
        {
            return this.clusters;
        }

        public double GetSolutionScore() // TODO: Maybe start working with delta's instead of recalculating everytime
        {
            double travelTime = 0.0d;
            double penaltyTime = 0.0d;

            List<int> uncompleteOrders = new List<int>();
            for (int i = 0; i < ordersCounter.CounterList.Count; i++)
            {
                if (!ordersCounter.CounterList[i].IsCompleted())
                {
                    int orderNumber = ordersCounter.CounterList[i].OrderNumber;
                    if (uncompleteOrders.Contains(orderNumber)) // Has already been punished
                        continue;
                    else
                        uncompleteOrders.Add(orderNumber);

                    penaltyTime += Data.Orders[orderNumber].PenaltyTime;
                    break;
                }
            }

            foreach (Tuple<Days, int, List<Route>> tuple in truckPlanning)
                foreach (Route route in tuple.Item3)
                    travelTime += route.TravelTime;

            return travelTime + penaltyTime;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            foreach (Tuple<Days, int, List<Route>> tuple in truckPlanning)
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
    }
}