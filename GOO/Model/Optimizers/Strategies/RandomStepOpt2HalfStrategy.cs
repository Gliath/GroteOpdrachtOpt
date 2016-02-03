using System;
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
            Planning = toStartFrom.GetRandomPlanning();
            if (Planning.Item3.Count == 0)
                return toStartFrom; 

            old_route = Planning.Item3[random.Next(Planning.Item3.Count)];
            if (old_route == null || old_route.Orders.Count < 3)
                return toStartFrom; // Could not find a valid route to shuffle

            new_route = new Route(Planning.Item1);
            foreach (Order order in old_route.Orders)
                new_route.AddOrder(order);

            int firstIndex = random.Next(old_route.Orders.Count - 1);
            int secondIndex = random.Next(old_route.Orders.Count - 1);
            if (firstIndex == secondIndex)
                return toStartFrom;

            double timeLimit = 0.0d; // Check if route can be swapped traveltime-wise
            foreach (Route route in Planning.Item3)
                if (route != old_route)
                    timeLimit += route.TravelTime;

            timeLimit = 43200.0d - timeLimit;

            if (new_route.CanHalfSwapOrder(old_route.Orders[firstIndex], old_route.Orders[secondIndex], timeLimit))
            {
                new_route.HalfSwapOrders(old_route.Orders[firstIndex], old_route.Orders[secondIndex]);
                toStartFrom.AddRoute(new_route);
                toStartFrom.RemoveRouteFromPlanning(Planning.Item1, Planning.Item2, old_route);
                toStartFrom.AddRouteToPlanning(Planning.Item1, Planning.Item2, new_route);
                toStartFrom.RemoveRoute(old_route);

                strategyHasExecuted = true;
            }

            return toStartFrom;
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