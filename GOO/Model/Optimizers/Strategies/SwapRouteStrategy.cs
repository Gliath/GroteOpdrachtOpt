using System;
using System.Collections.Generic;

using GOO.Utilities;

namespace GOO.Model.Optimizers.Strategies
{
    public class SwapRouteStrategy : Strategy
    {
        private Tuple<Days, int, List<Route>>[] Plans;
        private int[] routeIndicesSwapped;

        public SwapRouteStrategy()
            : base()
        {
            Plans = new Tuple<Days, int, List<Route>>[2];
            routeIndicesSwapped = new int[2];

            for (int i = 0; i < 2; i++)
                Plans[i] = null;
        }

        public override Solution executeStrategy(Solution toStartFrom)
        {
            for (int i = 0; i < 2; i++)
            {
                Plans[i] = toStartFrom.GetRandomPlanning();
                if (Plans[i].Item3.Count == 0 || (i == 1 && Plans[0].Item1 != Plans[1].Item1)) // Make sure it has routes to swap and is on the same day
                    return toStartFrom;

                routeIndicesSwapped[i] = random.Next(Plans[i].Item3.Count);
                if (i == 1 && routeIndicesSwapped[0] == routeIndicesSwapped[1])
                    return toStartFrom;
            }

            Route[] routesToSwap = new Route[2];
            for (int i = 0; i < 2; i++)
                routesToSwap[i] = Plans[i].Item3[routeIndicesSwapped[i]];

            for (int i = 0; i < 2; i++) // Check if route can be swapped traveltime-wise
            {
                double tempTT = 0.0d;
                foreach (Route route in Plans[i].Item3)
                    if(route != routesToSwap[i])
                        tempTT += route.TravelTime;

                tempTT += routesToSwap[(i + 1) % 2].TravelTime;

                if (tempTT > 43200.0d)
                    return toStartFrom;
            }

            for (int i = 0; i < 2; i++)
            {
                toStartFrom.RemoveRouteFromPlanning(Plans[i].Item1, Plans[i].Item2, routesToSwap[i]);
                toStartFrom.AddRouteToPlanning(Plans[(i + 1) % 2].Item1, Plans[(i + 1) % 2].Item2, routesToSwap[i]);
            }

            strategyHasExecuted = true;
            return toStartFrom;
        }

        public override Solution undoStrategy(Solution toStartFrom)
        {
            if (!strategyHasExecuted)
                return toStartFrom;

            Route[] routesToSwap = new Route[2];
            for (int i = 0; i < 2; i++)
                routesToSwap[i] = Plans[i].Item3[routeIndicesSwapped[i]];

            for (int i = 0; i < 2; i++)
            {
                toStartFrom.RemoveRouteFromPlanning(Plans[i].Item1, Plans[i].Item2, routesToSwap[i]);
                toStartFrom.AddRouteToPlanning(Plans[(i + 1) % 2].Item1, Plans[(i + 1) % 2].Item2, routesToSwap[i]);
            }

            return toStartFrom;
        }
    }
}