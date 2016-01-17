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
            Planning = toStartFrom.getRandomPlanning();
            RoutesFromSolution = Planning.Item3;
            old_route = RoutesFromSolution[random.Next(RoutesFromSolution.Count)];

            new_route = new Route(Planning.Item1);
            foreach (Order order in old_route.Orders)
                new_route.AddOrder(order);

            while (old_route.Orders.Count < 3) // Prevent infinite runs? (if it has only routes with less than 3 orders, one being Order0)
                old_route = RoutesFromSolution[random.Next(RoutesFromSolution.Count)];

            int firstIndex = random.Next(old_route.Orders.Count - 2);
            int secondIndex = random.Next(old_route.Orders.Count - 2);
            while (firstIndex == secondIndex)
                secondIndex = random.Next(old_route.Orders.Count - 2);

            swapOrders(old_route.Orders[firstIndex], old_route.Orders[secondIndex], new_route); // Check if can be swapped

            RoutesFromSolution.Remove(old_route);
            RoutesFromSolution.Add(new_route);

            toStartFrom.RemoveItemFromPlanning(Planning.Item1, Planning.Item2);
            toStartFrom.AddNewItemToPlanning(Planning.Item1, Planning.Item2, RoutesFromSolution);

            return toStartFrom;
        }

        private void swapOrders(Order A, Order B, Route route)
        {
            route.RemoveOrder(B);
            route.AddOrderAt(B, A);
        }

        public override Solution undoStrategy(Solution toStartFrom)
        {
            RoutesFromSolution.Remove(new_route);
            RoutesFromSolution.Add(old_route);

            toStartFrom.RemoveItemFromPlanning(Planning.Item1, Planning.Item2);
            toStartFrom.AddNewItemToPlanning(Planning.Item1, Planning.Item2, RoutesFromSolution);

            return toStartFrom;
        }
    }
}