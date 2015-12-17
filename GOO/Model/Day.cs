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
            _SC_routesPerTruck(toCopy.routesPerTruck);
        }

        public Day()
        {
            routesPerTruck = new List<Route>[NUMBER_OF_TRUCKS];
        }

        public Day GetShallowCopy()
        {
            return new Day(this);
        }

        private List<Route>[] _SC_routesPerTruck(List<Route>[] toCopyFrom)
        {
            this.routesPerTruck = new List<Route>[toCopyFrom.Length - 1]; //ToDo: DOUBLE CHECK
            for (int i = 0; i < routesPerTruck.Length; i++)
            {
                List<Route> l = routesPerTruck[i];
                List<Route> toAdd = new List<Route>(l.Count);

                for (int j = 0; j < l.Count; j++)
                {
                    toAdd[j] = l[j].GetShallowCopy();
                }
                routesPerTruck[i] = toAdd;
            }
            return routesPerTruck;
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