﻿using System;
using System.Collections.Generic;

using GOO.Utilities;

namespace GOO.Model.Optimizers.Strategies
{
    public class RandomRouteOpt2HalfStrategy : Strategy
    {
        private Tuple<Days, int, List<Route>> Planning;
        private Route old_route;
        private Route new_route;
        private Route routeToWorkWith;
        private List<Route> RoutesFromSolution;

        public RandomRouteOpt2HalfStrategy()
            : base()
        {

        }

        public override Solution executeStrategy(Solution toStartFrom)
        {
            Planning = toStartFrom.GetRandomPlanning();
            RoutesFromSolution = Planning.Item3;

            //save the begin route for rollback
            old_route = RoutesFromSolution[random.Next(RoutesFromSolution.Count)];

            //copy the begin route for the 2-opt and create the route for the 2-opt check
            routeToWorkWith = new Route(Planning.Item1);
            new_route = new Route(Planning.Item1);
            foreach (Order order in old_route.Orders)
            {
                routeToWorkWith.AddOrder(order);
                new_route.AddOrder(order);
            }

            //start doing the opt-2 algorithm on the route list
            double best_traveltime = old_route.TravelTime;
            double new_traveltime = double.MaxValue;

            int improvestep = 0;
            while (improvestep < 50)
            {
                for (int i = 0; i < routeToWorkWith.Orders.Count - 2; i++)
                {
                    for (int k = i + 1; k < routeToWorkWith.Orders.Count - 1; k++)
                    {
                        //swap the 2 coords
                        swapOrders(routeToWorkWith.Orders[i], routeToWorkWith.Orders[k], routeToWorkWith);
                        new_traveltime = routeToWorkWith.TravelTime;
                        if (new_traveltime < best_traveltime)
                        {
                            improvestep = 0;
                            new_route = routeToWorkWith;
                            best_traveltime = new_traveltime;

                            routeToWorkWith = new Route(Planning.Item1);
                            foreach (Order order in new_route.Orders)
                                routeToWorkWith.AddOrder(order);
                        }
                    }
                }

                improvestep++;
            }

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
            // Nette manier zou zijn om de tuple weer op te halen gebaseerd op Planning.Item2 en dag.
            RoutesFromSolution.Remove(new_route);
            RoutesFromSolution.Add(old_route);

            toStartFrom.RemoveItemFromPlanning(Planning.Item1, Planning.Item2);
            toStartFrom.AddNewItemToPlanning(Planning.Item1, Planning.Item2, RoutesFromSolution);

            return toStartFrom;
        }
    }
}