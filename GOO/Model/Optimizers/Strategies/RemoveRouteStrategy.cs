﻿using System;
using System.Collections.Generic;

using GOO.Utilities;

namespace GOO.Model.Optimizers.Strategies
{
    public class RemoveRouteStrategy : Strategy
    {
        private Tuple<Days, int, List<Route>> Planning;
        private Route routeDeleted;

        public RemoveRouteStrategy()
            : base()
        {
            Planning = null;
            routeDeleted = null;
        }

        public override Solution executeStrategy(Solution toStartFrom)
        {
            if (toStartFrom.GetEntirePlanning().Count == 0)
                return toStartFrom;

            for (int planningCounter = 0; planningCounter < 5; planningCounter++)
            {
                Planning = toStartFrom.GetRandomPlanning();
                if (Planning.Item3.Count > 0)
                    break;
            }

            if (Planning.Item3.Count == 0)
                return toStartFrom;

            int routeIndex = random.Next(Planning.Item3.Count);
            routeDeleted = Planning.Item3[routeIndex];

            toStartFrom.RemoveRouteFromPlanning(Planning.Item1, Planning.Item2, routeDeleted);

            return toStartFrom;
        }

        public override Solution undoStrategy(Solution toStartFrom)
        {
            toStartFrom.AddRouteToPlanning(Planning.Item1, Planning.Item2, routeDeleted);
            return toStartFrom;
        }
    }
}