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

        private List<Cluster> clusters;
        private List<Tuple<Days, int, List<Route>>> truckPlanning;

        public Solution(List<Cluster> clusters)
        {
            this.ordersCounter = new OrdersCounter();
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
                    ordersCounter.RemoveOccurrence(order.OrderNumber, route.Day);
	            }
            }
        }

        private void updateOrdersCounterAfterAdding(List<Route> addedRoutes)
        {
            foreach (Route route in addedRoutes)
            {
                foreach (Order order in route.Orders)
                {
                    if(order.OrderNumber != 0)
                        ordersCounter.AddOccurrence(order.OrderNumber, route.Day);
                }
            }
        }

        public Tuple<Days, int, List<Route>> getPlanningForATruck(Days day, int truckID)
        {
            return this.truckPlanning.Find(t => t.Item1 == day && t.Item2 == truckID);
        }

        public Tuple<Days, int, List<Route>> getRandomPlanningForATruck()
        {
            int random = new Random().Next(truckPlanning.Count);
            return this.truckPlanning[random];
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

                    penaltyTime += FilesInitializer._Orders[orderNumber].PenaltyTime;
                    break;
                }
            } // penaltyTime has been calculated

            foreach (Tuple<Days, int, List<Route>> tuple in truckPlanning)
                foreach (Route route in tuple.Item3) // Item3 in the tuple is always a List<Route>
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
                {
                    for (int orderID = 0; orderID < routes[routeID].Orders.Count; orderID++)
                    {
                        sb.AppendLine(String.Format("{0};{1};{2};{3}", tuple.Item2 + 1, tuple.Item1, ++sequenceID, routes[routeID].Orders[orderID].OrderNumber));
                    }
                }
            }

            return sb.ToString();
        }
    }
}