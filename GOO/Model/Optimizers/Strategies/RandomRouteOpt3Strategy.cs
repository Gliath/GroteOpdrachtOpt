using System;
using System.Collections.Generic;

using GOO.Utilities;

namespace GOO.Model.Optimizers.Strategies
{
    public class RandomRouteOpt3Strategy : Strategy
    {
        private readonly int NumberOfRoutesCreated = 6;

        private Days day;
        private int truck;
        private Route old_route;
        private Route new_route;
        private Route[] routesToWorkWith;
        private List<Route> RoutesFromSolution;

        public RandomRouteOpt3Strategy()
            : base()
        {
            routesToWorkWith = new Route[NumberOfRoutesCreated];
        }

        public override Solution executeStrategy(Solution toStartFrom)
        {
            Tuple<Days, int, List<Route>> Planning = toStartFrom.getRandomPlanningForATruck();
            day = Planning.Item1;
            truck = Planning.Item2;
            RoutesFromSolution = Planning.Item3;

            //save the begin route for rollback
            old_route = RoutesFromSolution[random.Next(RoutesFromSolution.Count)];

            //copy the begin route for the 2-opt and create the route for the 2-opt check
            for (int i = 0; i < routesToWorkWith.Length; i++)
                routesToWorkWith[i] = new Route(day);

            new_route = new Route(day);
            foreach (Order order in old_route.Orders)
            {
                if (order.OrderNumber != 0)
                {
                    for (int i = 0; i < routesToWorkWith.Length; i++)
                        routesToWorkWith[i].AddOrder(order);

                    new_route.AddOrder(order);
                }
            }

            //start doing the opt-3 algorithm on the route list
            double best_traveltime = old_route.TravelTime;
            double[] new_traveltime = new double[NumberOfRoutesCreated];
            for (int i = 0; i < new_traveltime.Length; i++)
                new_traveltime[i] = double.MaxValue;

            int improvestep = 0;
            while (improvestep < 10)
            {
                for (int i = 0; i < old_route.Orders.Count - 3; i++)
                {
                    for (int j = i + 1; j < old_route.Orders.Count - 2; j++)
                    {
                        for (int k = j + 1; k < old_route.Orders.Count - 1; k++)
                        {
                            //swap the 2 coords
                            Order[] initialRouteI = new Order[NumberOfRoutesCreated];
                            for (int index = 0; index < initialRouteI.Length; index++)
                                initialRouteI[index] = routesToWorkWith[index].Orders[i];

                            Order[] initialRouteJ = new Order[NumberOfRoutesCreated];
                            for (int index = 0; index < initialRouteJ.Length; index++)
                                initialRouteJ[index] = routesToWorkWith[index].Orders[j];

                            Order[] initialRouteK = new Order[NumberOfRoutesCreated];
                            for (int index = 0; index < initialRouteK.Length; index++)
                                initialRouteK[index] = routesToWorkWith[index].Orders[k];

                            for (int index = 0; index < routesToWorkWith.Length; index++)
                            {
                                swapOrders(initialRouteI[index], initialRouteK[index], routesToWorkWith[index]);
                                swapOrders(initialRouteI[index], initialRouteJ[index], routesToWorkWith[index]);
                                new_traveltime[index] = routesToWorkWith[index].TravelTime;
                            }

                            bool hasABetterTime = false;
                            for (int index = 0; index < new_traveltime.Length; index++)
                            {
                                if(new_traveltime[index] < best_traveltime)
                                {
                                    improvestep = 0;
                                    hasABetterTime = true;

                                    new_route = routesToWorkWith[index];
                                    best_traveltime = new_traveltime[index];
                                }
                            }

                            if (hasABetterTime)
                            {
                                for (int index = 0; index < routesToWorkWith.Length; index++)
                                    routesToWorkWith[index] = new Route(day);

                                foreach (Order order in old_route.Orders)
                                    if (order.OrderNumber != 0)
                                        for (int index = 0; index < routesToWorkWith.Length; index++)
                                            routesToWorkWith[index].AddOrder(order);

                            }
                        }
                    }
                }

                improvestep++;
            }

            Console.WriteLine("OPT3");
            Console.WriteLine("Old Travel Time:  {0}", old_route.TravelTime);
            Console.WriteLine("Best Travel Time: {0}", best_traveltime);

            RoutesFromSolution.Remove(old_route);
            RoutesFromSolution.Add(new_route);

            toStartFrom.RemoveItemFromPlanning(day, truck);
            toStartFrom.AddNewItemToPlanning(day, truck, RoutesFromSolution);

            return toStartFrom;
        }

        private void swapOrders(Order A, Order B, Route route)
        {
            route.SwapOrders(A, B);
        }

        public override Solution undoStrategy(Solution toStartFrom)
        {
            // Nette manier zou zijn om de tuple weer op te halen gebaseerd op truck en dag.
            RoutesFromSolution.Remove(new_route);
            RoutesFromSolution.Add(old_route);

            toStartFrom.RemoveItemFromPlanning(day, truck);
            toStartFrom.AddNewItemToPlanning(day, truck, RoutesFromSolution);

            return toStartFrom;
        }
    }
}
