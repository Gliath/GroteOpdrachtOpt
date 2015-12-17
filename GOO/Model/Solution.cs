using System;
using System.Collections.Generic;
using System.Text;

using GOO.Utilities;

namespace GOO.Model
{
    public class Solution
    {
        private static readonly int NUMBER_OF_DAYS = 5;

        private Day[] days;
        private OrdersCounter ordersCounter;

        public Solution()
        {
            days = new Day[NUMBER_OF_DAYS];
            ordersCounter = new OrdersCounter();
        }

        public double GetSolutionScore()
        {
            double travelTime = 0.0d;
            double penaltyTime = 0.0d;

            #if DEBUG
            Console.WriteLine("Solution contains every order: {0}", ordersCounter.IsCompleted());
            #endif

            List<int> uncompleteOrders = new List<int>();
            for (int i = 0; i < ordersCounter.CounterList.Count; i++)
            {
                if (!ordersCounter.CounterList[i].IsOrderCompleted())
                {
                    int orderNumber = ordersCounter.CounterList[i].OrderNumber;
                    if (uncompleteOrders.Contains(orderNumber)) // Has already been punished
                        continue;
                    else
                        uncompleteOrders.Add(orderNumber); // Is going to be punished, add it to the list

                    foreach (Order order in FilesInitializer._Orders)
                        if (order.OrderNumber == orderNumber)
                        {
                            penaltyTime += order.PenaltyTime;
                            break;
                        }
                }
            } // penaltyTime has been calculated

            return travelTime + penaltyTime;
        }

        // To do optimize method, by storing it in a variable and know when it has been changed (and where)
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            for (int dayID = 0; dayID < days.Length; dayID++)
            {
                for (int truckID = 0; truckID < Day.NUMBER_OF_TRUCKS; truckID++)
                {
                    int sequenceID = 0;
                    List<Route> routes = days[dayID].GetRoutes(truckID);

                    for (int routeID = 0; routeID < routes.Count; routeID++)
                    {
                        for (int orderID = 0; orderID < routes[routeID].Orders.Count; orderID++)
                        {
                            Order order = routes[routeID].Orders[orderID];

                            sb.AppendLine(String.Format("{0};{1};{2};{3}", dayID + 1, truckID + 1, ++sequenceID, order.OrderNumber));
                        }
                    }
                }
            }

            return sb.ToString();
        }
    }
}