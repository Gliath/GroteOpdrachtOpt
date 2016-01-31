using System;
using System.Collections.Generic;

using GOO.Utilities;

namespace GOO.Model.Optimizers.Strategies
{
    public class RandomStepOpt3Strategy : Strategy
    {
        private Tuple<Days, int, List<Route>> Planning;
        private Route old_route;
        private Route new_route;
        private List<Route> RoutesFromSolution;

        public RandomStepOpt3Strategy()
            : base()
        {

        }

        public override Solution executeStrategy(Solution toStartFrom)
        {
            for (int planningCounter = 0; planningCounter < 5; planningCounter++)
            {
                Planning = toStartFrom.getRandomPlanning();
                RoutesFromSolution = Planning.Item3;
                old_route = RoutesFromSolution[random.Next(RoutesFromSolution.Count)];

                for (int counter = 0; counter < 5 && old_route.Orders.Count < 4; counter++)
                    old_route = RoutesFromSolution[random.Next(RoutesFromSolution.Count)];

                if (old_route.Orders.Count >= 4)
                    break;
            }

            if (old_route.Orders.Count < 4)
                return toStartFrom; // Could not find a valid route to shuffle

            new_route = new Route(Planning.Item1);
            foreach (Order order in old_route.Orders)
                new_route.AddOrder(order);

            int firstIndex = random.Next(old_route.Orders.Count - 2);
            int secondIndex = random.Next(old_route.Orders.Count - 2);
            int thirdIndex = random.Next(old_route.Orders.Count - 2);

            while (firstIndex == secondIndex)
                secondIndex = random.Next(old_route.Orders.Count - 2);
            while (firstIndex == thirdIndex || secondIndex == thirdIndex)
                thirdIndex = random.Next(old_route.Orders.Count - 2);

            swapOrders(old_route.Orders[firstIndex], old_route.Orders[secondIndex], old_route.Orders[thirdIndex], new_route);

            RoutesFromSolution.Remove(old_route);
            RoutesFromSolution.Add(new_route);

            toStartFrom.RemoveItemFromPlanning(Planning.Item1, Planning.Item2);
            toStartFrom.AddNewItemToPlanning(Planning.Item1, Planning.Item2, RoutesFromSolution);

            return toStartFrom;
        }

        private void swapOrders(Order A, Order B, Order C, Route route)
        {
            if (route.CanSwapOrder(A, B, C))
                route.SwapOrders(A, B, C);
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