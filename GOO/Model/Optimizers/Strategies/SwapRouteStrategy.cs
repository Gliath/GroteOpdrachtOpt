using System;
using System.Collections.Generic;

using GOO.Utilities;

namespace GOO.Model.Optimizers.Strategies
{
    public class SwapRouteStrategy : Strategy
    {
        private Tuple<Days, int, List<Route>>[] Plannings;
        private int[] routeIndicesSwapped;

        public SwapRouteStrategy()
            : base()
        {
            Plannings = new Tuple<Days, int, List<Route>>[2];
            routeIndicesSwapped = new int[2];

            for (int i = 0; i < 2; i++)
                Plannings[i] = null;
        }

        public override Solution executeStrategy(Solution toStartFrom) // switch two routes from different(?) plannings
        {
            for (int i = 0; i < 2; i++)
            {
                for (int planningCounter = 0; planningCounter < 5; planningCounter++)
                {
                    Plannings[i] = toStartFrom.GetRandomPlanning();
                    if (Plannings[i].Item3.Count > 0)
                        break;
                }

                if (Plannings[i].Item3.Count == 0)
                    return toStartFrom;

                routeIndicesSwapped[i] = random.Next(Plannings[i].Item3.Count);
            }

            Route first = Plannings[0].Item3[routeIndicesSwapped[0]];
            Route second = Plannings[1].Item3[routeIndicesSwapped[1]];

            Plannings[0].Item3[routeIndicesSwapped[0]] = second;
            Plannings[1].Item3[routeIndicesSwapped[1]] = first;

            for (int i = 0; i < 2; i++)
            {
                toStartFrom.RemoveItemFromPlanning(Plannings[i].Item1, Plannings[i].Item2);
                toStartFrom.AddNewItemToPlanning(Plannings[i].Item1, Plannings[i].Item2, Plannings[i].Item3);
            }

            return toStartFrom;
        }

        public override Solution undoStrategy(Solution toStartFrom)
        {
            for (int i = 0; i < 2; i++)
                if (Plannings[i].Item3.Count == 0)
                    return toStartFrom;

            Route first = Plannings[0].Item3[routeIndicesSwapped[0]];
            Route second = Plannings[1].Item3[routeIndicesSwapped[1]];

            Plannings[0].Item3[routeIndicesSwapped[0]] = second;
            Plannings[1].Item3[routeIndicesSwapped[1]] = first;

            for (int i = 0; i < 2; i++)
            {
                toStartFrom.RemoveItemFromPlanning(Plannings[i].Item1, Plannings[i].Item2);
                toStartFrom.AddNewItemToPlanning(Plannings[i].Item1, Plannings[i].Item2, Plannings[i].Item3);
            }

            return toStartFrom;
        }
    }
}