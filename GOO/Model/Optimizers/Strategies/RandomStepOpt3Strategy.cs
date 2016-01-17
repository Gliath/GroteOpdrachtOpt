﻿using System;
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
            Planning = toStartFrom.getRandomPlanning();
            RoutesFromSolution = Planning.Item3;
            old_route = RoutesFromSolution[random.Next(RoutesFromSolution.Count)];

            new_route = new Route(Planning.Item1);
            foreach (Order order in old_route.Orders)
                new_route.AddOrder(order);

            while (old_route.Orders.Count < 4) // Prevent infinite runs? (if it has only routes with less than 4 orders, one being Order0)
                old_route = RoutesFromSolution[random.Next(RoutesFromSolution.Count)];

            int firstIndex = random.Next(old_route.Orders.Count - 2);
            int secondIndex = random.Next(old_route.Orders.Count - 2);
            int thirdIndex = random.Next(old_route.Orders.Count - 2);

            while (firstIndex == secondIndex)
                secondIndex = random.Next(old_route.Orders.Count - 2);
            while (firstIndex == thirdIndex || secondIndex == thirdIndex)
                thirdIndex = random.Next(old_route.Orders.Count - 2);

            new_route.SwapOrders(old_route.Orders[firstIndex], old_route.Orders[thirdIndex]); // Check if can be swapped
            new_route.SwapOrders(old_route.Orders[firstIndex], old_route.Orders[secondIndex]); // Check if can be swapped

            RoutesFromSolution.Remove(old_route);
            RoutesFromSolution.Add(new_route);

            toStartFrom.RemoveItemFromPlanning(Planning.Item1, Planning.Item2);
            toStartFrom.AddNewItemToPlanning(Planning.Item1, Planning.Item2, RoutesFromSolution);

            return toStartFrom;
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