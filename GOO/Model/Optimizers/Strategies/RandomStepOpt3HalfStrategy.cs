using System;
using System.Collections.Generic;

using GOO.Utilities;

namespace GOO.Model.Optimizers.Strategies
{
    public class RandomStepOpt3HalfStrategy : Strategy
    {
        private Tuple<Days, int, List<Route>> Planning;
        private Route old_route;
        private Route new_route;

        public RandomStepOpt3HalfStrategy()
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

                for (int counter = 0; counter < 5 && old_route.Orders.Count < 4; counter++)
                    old_route = Planning.Item3[random.Next(Planning.Item3.Count)];

                if (old_route.Orders.Count >= 4)
                    break;
            }

            if (old_route == null || old_route.Orders.Count < 4)
                return toStartFrom; // Could not find a valid route to shuffle

            new_route = new Route(Planning.Item1);
            foreach (Order order in old_route.Orders)
                new_route.AddOrder(order);

            int firstIndex = random.Next(old_route.Orders.Count - 1);
            int secondIndex = random.Next(old_route.Orders.Count - 1);
            int thirdIndex = random.Next(old_route.Orders.Count - 1);

            while (firstIndex == secondIndex)
                secondIndex = random.Next(old_route.Orders.Count - 1);
            while (firstIndex == thirdIndex || secondIndex == thirdIndex)
                thirdIndex = random.Next(old_route.Orders.Count - 1);

            double timeLimit = 0.0d; // Check if route can be swapped traveltime-wise
            foreach (Route route in Planning.Item3)
                if (route != old_route)
                    timeLimit += route.TravelTime;

            timeLimit = 43200.0d - timeLimit;

            swapOrders(old_route.Orders[firstIndex], old_route.Orders[thirdIndex], old_route.Orders[thirdIndex], new_route, timeLimit);

            toStartFrom.AddRoute(new_route);
            toStartFrom.RemoveRouteFromPlanning(Planning.Item1, Planning.Item2, old_route);
            toStartFrom.AddRouteToPlanning(Planning.Item1, Planning.Item2, new_route);
            toStartFrom.RemoveRoute(old_route);

            strategyHasExecuted = true;
            return toStartFrom;
        }

        private void swapOrders(Order A, Order B, Order C, Route route, double timeLimit)
        {
            if (route.CanHalfSwapOrder(A, B, C, timeLimit))
                route.HalfSwapOrders(A, B, C);
        }

        public override Solution undoStrategy(Solution toStartFrom)
        {
            if (!strategyHasExecuted)
                return toStartFrom;

            toStartFrom.AddRoute(old_route);
            toStartFrom.RemoveRouteFromPlanning(Planning.Item1, Planning.Item2, new_route);
            toStartFrom.AddRouteToPlanning(Planning.Item1, Planning.Item2, old_route);
            toStartFrom.RemoveRoute(new_route);

            return toStartFrom;
        }
    }
}