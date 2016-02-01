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
        private List<Route> RoutesFromSolution;

        public RandomStepOpt2HalfStrategy()
            : base()
        {

        }

        public override Solution executeStrategy(Solution toStartFrom)
        {
            for (int planningCounter = 0; planningCounter < 5; planningCounter++)
            {
                Planning = toStartFrom.GetRandomPlanning();
                RoutesFromSolution = Planning.Item3;
                old_route = RoutesFromSolution[random.Next(RoutesFromSolution.Count)];

                for (int counter = 0; counter < 5 && old_route.Orders.Count < 3; counter++)
                    old_route = RoutesFromSolution[random.Next(RoutesFromSolution.Count)];

                if (old_route.Orders.Count >= 3)
                    break;
            }

            if (old_route.Orders.Count < 3)
                return toStartFrom; // Could not find a valid route to shuffle

            new_route = new Route(Planning.Item1);
            foreach (Order order in old_route.Orders)
                new_route.AddOrder(order);

            int firstIndex = random.Next(old_route.Orders.Count - 2);
            int secondIndex = random.Next(old_route.Orders.Count - 2);
            while (firstIndex == secondIndex)
                secondIndex = random.Next(old_route.Orders.Count - 2);

            swapOrders(old_route.Orders[firstIndex], old_route.Orders[secondIndex], new_route);

            toStartFrom.AllRoutes.Remove(old_route);
            toStartFrom.AllRoutes.Add(new_route);

            toStartFrom.RemoveRouteFromPlanning(Planning.Item1, Planning.Item2, old_route);
            toStartFrom.AddRouteToPlanning(Planning.Item1, Planning.Item2, new_route);

            toStartFrom.AvailableRoutes.Remove(old_route);

            return toStartFrom;
        }

        private void swapOrders(Order A, Order B, Route route)
        {
            if (route.CanHalfSwapOrder(A, B))
                route.HalfSwapOrders(A, B);
        }

        public override Solution undoStrategy(Solution toStartFrom)
        {
            RoutesFromSolution.Remove(new_route);
            RoutesFromSolution.Add(old_route);

            toStartFrom.AllRoutes.Remove(new_route);
            toStartFrom.AllRoutes.Add(old_route);

            toStartFrom.RemoveItemFromPlanning(Planning.Item1, Planning.Item2);
            toStartFrom.AddNewItemToPlanning(Planning.Item1, Planning.Item2, RoutesFromSolution);

            return toStartFrom;
        }
    }
}