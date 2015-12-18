using System;
using System.Collections.Generic;
using System.Text;

namespace GOO.Model
{
    public class Day
    {
        public static readonly int NUMBER_OF_TRUCKS = 2;

        private List<Route>[] routesPerTruck;

        private Day(Day toCopy)
        {
            this.routesPerTruck = SC_routesPerTruck(toCopy.routesPerTruck);
        }

        private List<Route>[] SC_routesPerTruck(List<Route>[] toCopyFrom)
        {
            List<Route>[] anArrayOfListsToReturn = new List<Route>[toCopyFrom.Length];

            for (int i = 0; i < toCopyFrom.Length; i++)
            {
                List<Route> listToCopy = toCopyFrom[i];
                List<Route> listToAdd = new List<Route>(listToCopy.Count);

                for (int j = 0; j < listToCopy.Count; j++)
                    listToAdd.Add(listToCopy[j].GetShallowCopy());

                anArrayOfListsToReturn[i] = listToAdd;
            }

            return anArrayOfListsToReturn;
        }

        public Day()
        {
            routesPerTruck = new List<Route>[NUMBER_OF_TRUCKS];
        }

        public void GenerateRoutes(OrdersCounter ordersCounter)
        {
            for (int truckID = 0; truckID < routesPerTruck.Length; truckID++)
            {
                routesPerTruck[truckID] = new List<Route>();

                double maxTimeLeft = 43200.0d;

                for (int numOfTries = 50; numOfTries > 0; numOfTries--) // Tries to make 5 routes for a day (for each truck)
                {
                    Route route = new Route();
                    route.CreateRouteList(100000, maxTimeLeft, 200, ordersCounter);

                    if (route.Orders.Count == 0)
                        route = null;
                    else
                    {
                        maxTimeLeft -= route.TravelTime;
                        routesPerTruck[truckID].Add(route);
                    }

                    if (maxTimeLeft <= 1800.0d)
                        break;
                }
            }
        }

        public Day GetShallowCopy()
        {
            return new Day(this);
        }

        public void SetRoutes(int truckID, List<Route> routes)
        {
            routesPerTruck[truckID] = routes;
        }

        // Needs to be locked for parallelism because this method will be used to change the list/array
        public List<Route> GetRoutes(int truckID)
        {
            return routesPerTruck[truckID];
        }
    }
}