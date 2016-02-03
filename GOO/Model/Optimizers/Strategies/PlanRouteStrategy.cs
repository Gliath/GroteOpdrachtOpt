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

            Planning = toStartFrom.GetRandomPlanning();
            routePlanned = toStartFrom.AvailableRoutes[random.Next(toStartFrom.AvailableRoutes.Count)];

            double totalTravelTime = 0.0d;
            foreach (Route route in Planning.Item3)
                totalTravelTime += route.TravelTime;

            if (routePlanned.Day != Planning.Item1 || totalTravelTime + routePlanned.TravelTime > 43200.0d)
                return toStartFrom;

            toStartFrom.AddRouteToPlanning(Planning.Item1, Planning.Item2, routePlanned);
            strategyHasExecuted = true;

            return toStartFrom;
        }

        public override Solution undoStrategy(Solution toStartFrom)
        {
            if (!strategyHasExecuted)
                return toStartFrom;
            
            toStartFrom.RemoveRouteFromPlanning(Planning.Item1, Planning.Item2, routePlanned);

            return toStartFrom;
        }
    }
}