using System;
using System.Collections.Generic;

using GOO.Utilities;

namespace GOO.Model.Optimizers.Strategies
{
    public class PlanRouteStrategy : Strategy
    {
        private Tuple<Days, int, List<Route>> Planning;
        private Route routePlanned;

        public PlanRouteStrategy()
            : base()
        {
            Planning = null;
            routePlanned = null;
        }

        public override Solution executeStrategy(Solution toStartFrom)
        {
            for (int numOfTries = 16; numOfTries > 0; numOfTries--)
            {
                double totalTravelTime = 0.0d;
                for (int planningCounter = 0; planningCounter < 5; planningCounter++)
                {
                    Planning = toStartFrom.getRandomPlanning();
                    totalTravelTime = 0.0d;

                    foreach (Route route in Planning.Item3)
                        totalTravelTime += route.TravelTime;

                    if (totalTravelTime > 43200.0d)
                        continue;
                }

                if (totalTravelTime > 43200.0d)
                    continue;

                int routeIndex = -1;
                for (int routeCounter = 0; routeCounter < 8; routeCounter++)
                {
                    routeIndex = random.Next(OrdersTracker.Instance.AllRoutes.Count);
                    routePlanned = OrdersTracker.Instance.AllRoutes[routeIndex];

                    if (totalTravelTime + routePlanned.TravelTime <= 43200.0d)
                        break;
                }

                if (totalTravelTime + routePlanned.TravelTime > 43200.0d)
                    continue;

                OrdersTracker.Instance.AllRoutes.RemoveAt(routeIndex);
                Planning.Item3.Add(routePlanned);

                toStartFrom.RemoveItemFromPlanning(Planning.Item1, Planning.Item2);
                toStartFrom.AddNewItemToPlanning(Planning.Item1, Planning.Item2, Planning.Item3);

                break;
            }

            return toStartFrom;
        }

        public override Solution undoStrategy(Solution toStartFrom)
        {
            OrdersTracker.Instance.AllRoutes.Add(routePlanned);
            Planning.Item3.Remove(routePlanned);

            toStartFrom.RemoveItemFromPlanning(Planning.Item1, Planning.Item2);
            toStartFrom.AddNewItemToPlanning(Planning.Item1, Planning.Item2, Planning.Item3);

            return toStartFrom;
        }
    }
}