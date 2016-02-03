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
                for (int planningCounter = 0; planningCounter < 5; planningCounter++)
                {
                    Plans[i] = toStartFrom.GetRandomPlanning();
                    if (Plans[i].Item3.Count > 0)
                        break;
                }

                if (Plans[i].Item3.Count == 0)
                    return toStartFrom;

                routeIndicesSwapped[i] = random.Next(Plans[i].Item3.Count);
            }

            Route[] routesToSwap = new Route[2];
            for (int i = 0; i < 2; i++)
                routesToSwap[i] = Plans[i].Item3[routeIndicesSwapped[i]];

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