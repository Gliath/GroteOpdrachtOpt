using System;
using System.Collections.Generic;
using System.Text;

namespace GOO.Model
{
    public class Solution
    {
        private static readonly int NUMBER_OF_DAYS = 5;

        private Day[] days;

        public Solution()
        {
            days = new Day[NUMBER_OF_DAYS];
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