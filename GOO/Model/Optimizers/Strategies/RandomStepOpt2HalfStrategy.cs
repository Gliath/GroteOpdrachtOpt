﻿using System;
using System.Collections.Generic;

using GOO.Utilities;

namespace GOO.Model.Optimizers.Strategies
{
    public class RandomStepOpt2HalfStrategy : Strategy
    {
        private Tuple<Days, int, List<Route>> Planning;
        private Route old_route;
        private Route new_route;

        public RandomStepOpt2HalfStrategy()
            : base()
        {
            Planning = null;
            old_route = null;
            new_route = null;
        }

        public override Solution executeStrategy(Solution toStartFrom)
        {
            for (int planningCounter = 0; planningCounter < 5; planningCounter++)
            {
                Planning = toStartFrom.GetRandomPlanning();
                if (Planning.Item3.Count == 0)
                    continue;

                old_route = Planning.Item3[random.Next(Planning.Item3.Count)];

                for (int counter = 0; counter < 5 && old_route.Orders.Count < 3; counter++)
                    old_route = Planning.Item3[random.Next(Planning.Item3.Count)];

                if (old_route.Orders.Count >= 3)
                    break;
            }

            if (old_route == null || old_route.Orders.Count < 3)
                return toStartFrom; // Could not find a valid route to shuffle

            new_route = new Route(Planning.Item1);
            foreach (Order order in old_route.Orders)
                new_route.AddOrder(order);

            int firstIndex = random.Next(old_route.Orders.Count - 1);
            int secondIndex = random.Next(old_route.Orders.Count - 1);
            while (firstIndex == secondIndex)
                secondIndex = random.Next(old_route.Orders.Count - 1);

            swapOrders(old_route.Orders[firstIndex], old_route.Orders[secondIndex], new_route);

            toStartFrom.AddRoute(new_route);
            toStartFrom.RemoveRouteFromPlanning(Planning.Item1, Planning.Item2, old_route);
            toStartFrom.AddRouteToPlanning(Planning.Item1, Planning.Item2, new_route);
            toStartFrom.RemoveRoute(old_route);

            return toStartFrom;
        }

        private void swapOrders(Order A, Order B, Route route)
        {
            if (route.CanHalfSwapOrder(A, B))
                route.HalfSwapOrders(A, B);
        }

        public override Solution undoStrategy(Solution toStartFrom)
        {
            if (new_route != null)
            {
                toStartFrom.AddRoute(old_route);
                toStartFrom.RemoveRouteFromPlanning(Planning.Item1, Planning.Item2, new_route);
                toStartFrom.AddRouteToPlanning(Planning.Item1, Planning.Item2, old_route);
                toStartFrom.RemoveRoute(new_route);
            }

            return toStartFrom;
        }
    }
}