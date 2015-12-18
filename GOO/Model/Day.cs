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
            this.routesPerTruck = _SC_routesPerTruck(toCopy.routesPerTruck);
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
            List<Route>[] toFill = new List<Route>[toCopyFrom.Length];
            for (int i = 0; i < toFill.Length; i++)
            {
                List<Route> l = toFill[i];
                List<Route> toAdd = new List<Route>(l.Count);

                for (int j = 0; j < l.Count; j++)
                {
                    toAdd[j] = l[j].GetShallowCopy();
                }
                toFill[i] = toAdd;
            }
            return toFill;
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