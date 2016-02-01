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
            if (toStartFrom.AvailableRoutes.Count == 0)
                return toStartFrom; // No routes to plan

            for (int numOfTries = 16; numOfTries > 0; numOfTries--)
            {
                double totalTravelTime = 0.0d;
                for (int planningCounter = 0; planningCounter < 5; planningCounter++)
                {
                    Planning = toStartFrom.GetRandomPlanning();
                    totalTravelTime = 0.0d;

                    foreach (Route route in Planning.Item3)
                        totalTravelTime += route.TravelTime;

                    if (totalTravelTime > 43200.0d)
                        continue;
                }

                if (totalTravelTime > 43200.0d)
                    continue;

                int routeIndex = -1;
                for (int routeCounter = 0; routeCounter < 8 && toStartFrom.AvailableRoutes.Count > 0; routeCounter++)
                {
                    routeIndex = random.Next(toStartFrom.AvailableRoutes.Count);
                    routePlanned = toStartFrom.AvailableRoutes[routeIndex];

                    if (totalTravelTime + routePlanned.TravelTime <= 43200.0d)
                        break;
                }

                if (totalTravelTime + routePlanned.TravelTime > 43200.0d)
                    continue;

                toStartFrom.AddRouteToPlanning(Planning.Item1, Planning.Item2, routePlanned);
                break;
            }

            return toStartFrom;
        }

        public override Solution undoStrategy(Solution toStartFrom)
        {
            toStartFrom.RemoveRouteFromPlanning(Planning.Item1, Planning.Item2, routePlanned);
            return toStartFrom;
        }
    }
}