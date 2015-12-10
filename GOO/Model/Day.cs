using System;
using System.Collections.Generic;
using System.Text;

namespace GOO.Model
{
    public class Day
    {
        public static readonly int NUMBER_OF_TRUCKS = 2;

        private List<Route>[] routesPerTruck;

        public Day()
        {
            routesPerTruck = new List<Route>[NUMBER_OF_TRUCKS];
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