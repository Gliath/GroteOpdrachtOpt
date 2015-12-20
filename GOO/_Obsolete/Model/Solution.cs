using System;
using System.Collections.Generic;
using System.Text;

using GOO.Utilities;

namespace GOO.Obsolete.Model
{
    public class Solution
    {
        private static readonly int NUMBER_OF_DAYS = 5;

        private Day[] days;
        private OrdersCounter ordersCounter;

        private Solution(Solution toCopy)
        {
            this.ordersCounter = DeepCopy<OrdersCounter>.CopyFrom(toCopy.GetOrdersCounter());
            this.days = SC_days(toCopy.days);
        }

        private Day[] SC_days(Day[] toCopyFrom)
        {
            Day[] toFill = new Day[toCopyFrom.Length];
            for (int i = 0; i < toFill.Length; i++)
            {
                toFill[i] = toCopyFrom[i].GetShallowCopy();
            }
            return toFill;
        }

        public Solution()
        {
            days = new Day[NUMBER_OF_DAYS];
            ordersCounter = new OrdersCounter();
        }

        public void GenerateSolution()
        {
            for (int i = 0; i < days.Length; i++)
            {
                days[i] = new Day();
                days[i].GenerateRoutes(ordersCounter);
            }
        }

        public Solution GetShallowCopy()
        {
            return new Solution(this);
        }

        public double GetSolutionScore()
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

            for (int i = 0; i < days.Length; i++)
                for (int j = 0; j < Day.NUMBER_OF_TRUCKS; j++)
                    for (int k = 0; k < days[i].GetRoutes(j).Count; k++)
                        travelTime += days[i].GetRoutes(j)[k].TravelTime;

            return travelTime + penaltyTime;
        }

        public Day[] GetDays()
        {
            return days;
        }

        public void SetDays(Day[] newDayArray)
        {
            this.days = newDayArray;
        }

        public OrdersCounter GetOrdersCounter()
        {
            return ordersCounter;
        }

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

                            sb.AppendLine(String.Format("{0};{1};{2};{3}", truckID + 1, dayID + 1, ++sequenceID, order.OrderNumber));
                        }
                    }
                }
            }

            return sb.ToString();
        }
    }
}