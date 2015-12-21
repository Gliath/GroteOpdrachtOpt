using System;
using System.Collections.Generic;

using GOO.Utilities;

namespace GOO.Model
{
    public class RoutePlanner
    {
        private static Dictionary<Days, Order> PlanRoute(List<Cluster> clusters)
        {
            Dictionary<Days, Order> planRoutes = null;

            // fill PlanRoutes

            // for loop freq 3
            // loop freq 2 and plan it for the best (based on location) (average must be acceptable)
            // put freq 4 into the planning
            // add freq 1 so that the average is maintained

            // try to plan Solution that the average traveltime on each day is similar

            return planRoutes;
        }
    }
}