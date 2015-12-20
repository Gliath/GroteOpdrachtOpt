using System;
using System.Collections.Generic;

using GOO.Model;
using GOO.Utilities;

namespace GOO.KMeansModel
{
    public class KSolution
    {
        private OrdersCounter ordersCounter;

        private List<Cluster> clusters;
        private List<Tuple<Days, int, List<Route>>> truckPlanning;

        public KSolution(List<Cluster> clusters)
        {
            this.ordersCounter = new OrdersCounter();
            this.clusters = clusters;
            this.truckPlanning = new List<Tuple<Days, int, List<Route>>>();
        }

        public void AddNewItemToPlanning(Days day, int truckID, List<Route> routes)
        {
            this.truckPlanning.Add(new Tuple<Days, int, List<Route>>(day, truckID, routes));
        }

        public Tuple<Days, int, List<Route>> getPlanningForATruck(Days day, int truckID)
        {
            return this.truckPlanning.Find(t => t.Item1 == day && t.Item2 == truckID);
        }

        public void RemoveItemFromPlanning(Days day, int truckID)
        {
            this.truckPlanning.RemoveAll(t => t.Item1 == day && t.Item2 == truckID);
        }

        public void recountOrdersCounter()
        {
            ordersCounter.ClearAllOccurences();
            foreach (Tuple<Days, int, List<Route>> tuple in truckPlanning)
            {
                updateOrdersCounterAfterAdding(tuple.Item3);   
            }
        }

        private void updateOrdersCounterAfterRemoval(List<Route> removedRoutes)
        {
            foreach (Route route in removedRoutes)
            {
                foreach (Order order in route.Orders)
	            {
                    ordersCounter.RemoveOccurrence(order.OrderNumber);
	            }
            }
        }

        private void updateOrdersCounterAfterAdding(List<Route> addedRoutes)
        {
            foreach (Route route in addedRoutes)
            {
                foreach (Order order in route.Orders)
                {
                    ordersCounter.AddOccurrence(order.OrderNumber);
                }
            }
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
                        uncompleteOrders.Add(orderNumber); // Is going to be punished, add it to the list

                    foreach (Order order in FilesInitializer._Orders)
                        if (order != null)
                            if (order.OrderNumber == orderNumber)
                            {
                                penaltyTime += order.PenaltyTime;
                                break;
                            }
                }
            } // penaltyTime has been calculated

            foreach (Tuple<Days, int, List<Route>> tuple in truckPlanning)
                foreach (Route route in tuple.Item3) // Item3 in the tuple is always a List<Route>
                    travelTime += route.TravelTime;

            return travelTime + penaltyTime;
        }
    }
}
