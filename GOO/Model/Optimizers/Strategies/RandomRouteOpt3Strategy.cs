using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GOO.Utilities;

namespace GOO.Model.Optimizers.Strategies
{
    public class RandomRouteOpt3Strategy : Strategy
    {
        private Days day;
        private int truck;
        private Route old_route;
        private Route new_route;
        private Route routeToWorkWith1;
        private Route routeToWorkWith2;
        private Route routeToWorkWith3;
        private Route routeToWorkWith4;
        private Route routeToWorkWith5;
        private Route routeToWorkWith6;
        private List<Route> RoutesFromSolution;

        public override Solution executeStrategy(Solution toStartFrom)
        {
            Tuple<Days, int, List<Route>> Planning = toStartFrom.getRandomPlanningForATruck();
            day = Planning.Item1;
            truck = Planning.Item2;
            RoutesFromSolution = Planning.Item3;

            //save the begin route for rollback
            old_route = RoutesFromSolution[new Random().Next(RoutesFromSolution.Count)];

            //copy the begin route for the 2-opt and create the route for the 2-opt check
            routeToWorkWith1 = new Route(day);
            routeToWorkWith2 = new Route(day);
            routeToWorkWith3 = new Route(day);
            routeToWorkWith4 = new Route(day);
            routeToWorkWith5 = new Route(day);
            routeToWorkWith6 = new Route(day);
            new_route = new Route(day);
            foreach (Order order in old_route.Orders)
            {
                routeToWorkWith1.AddOrder(order);
                routeToWorkWith2.AddOrder(order);
                routeToWorkWith3.AddOrder(order);
                routeToWorkWith4.AddOrder(order);
                routeToWorkWith5.AddOrder(order);
                routeToWorkWith6.AddOrder(order);
                new_route.AddOrder(order);
            }

            //start doing the opt-3 algorithm on the route list
            double best_traveltime = old_route.TravelTime;
            double new_traveltime1 = double.MaxValue;
            double new_traveltime2 = double.MaxValue;
            double new_traveltime3 = double.MaxValue;
            double new_traveltime4 = double.MaxValue;
            double new_traveltime5 = double.MaxValue;
            double new_traveltime6 = double.MaxValue;

            int improvestep = 0;
            while (improvestep < 30)
            {
                for (int i = 0; i < routeToWorkWith1.Orders.Count - 3; i++)
                {
                    for (int j = i + 1; j < routeToWorkWith1.Orders.Count - 2; j++)
                    {
                        for (int k = j + 1; k < routeToWorkWith1.Orders.Count - 1; k++)
                        {
                            //swap the 2 coords
                            
                            Order initialRouteI1 = routeToWorkWith1.Orders[i];
                            Order initialRouteI2 = routeToWorkWith2.Orders[i];
                            Order initialRouteI3 = routeToWorkWith3.Orders[i];
                            Order initialRouteI4 = routeToWorkWith4.Orders[i];
                            Order initialRouteI5 = routeToWorkWith5.Orders[i];
                            Order initialRouteI6 = routeToWorkWith6.Orders[i];

                            Order initialRouteJ1 = routeToWorkWith1.Orders[j];
                            Order initialRouteJ2 = routeToWorkWith2.Orders[j];
                            Order initialRouteJ3 = routeToWorkWith3.Orders[j];
                            Order initialRouteJ4 = routeToWorkWith1.Orders[j];
                            Order initialRouteJ5 = routeToWorkWith2.Orders[j];
                            Order initialRouteJ6 = routeToWorkWith3.Orders[j];

                            Order initialRouteK1 = routeToWorkWith1.Orders[k];
                            Order initialRouteK2 = routeToWorkWith2.Orders[k];
                            Order initialRouteK3 = routeToWorkWith3.Orders[k];
                            Order initialRouteK4 = routeToWorkWith1.Orders[k];
                            Order initialRouteK5 = routeToWorkWith2.Orders[k];
                            Order initialRouteK6 = routeToWorkWith3.Orders[k];

                            swapOrders(initialRouteI1, initialRouteK1, routeToWorkWith1);
                            swapOrders(initialRouteI1, initialRouteJ1, routeToWorkWith1);
                            new_traveltime1 = routeToWorkWith1.TravelTime;

                            swapOrders(initialRouteI2, initialRouteJ2, routeToWorkWith2);
                            swapOrders(initialRouteI2, initialRouteK2, routeToWorkWith2);
                            new_traveltime2 = routeToWorkWith2.TravelTime;

                            swapOrders(initialRouteJ3, initialRouteI3, routeToWorkWith3);
                            swapOrders(initialRouteJ3, initialRouteK3, routeToWorkWith3);
                            new_traveltime3 = routeToWorkWith3.TravelTime;

                            swapOrders(initialRouteJ4, initialRouteK4, routeToWorkWith4);
                            swapOrders(initialRouteJ4, initialRouteI4, routeToWorkWith4);
                            new_traveltime4 = routeToWorkWith4.TravelTime;

                            swapOrders(initialRouteK5, initialRouteI5, routeToWorkWith5);
                            swapOrders(initialRouteK5, initialRouteJ5, routeToWorkWith5);
                            new_traveltime5 = routeToWorkWith5.TravelTime;

                            swapOrders(initialRouteK6, initialRouteJ6, routeToWorkWith6);
                            swapOrders(initialRouteK6, initialRouteI6, routeToWorkWith6);
                            new_traveltime6 = routeToWorkWith6.TravelTime;

                            if (new_traveltime1 < best_traveltime || new_traveltime2 < best_traveltime || new_traveltime3 < best_traveltime || new_traveltime4 < best_traveltime || new_traveltime5 < best_traveltime || new_traveltime6 < best_traveltime)
                            {
                                improvestep = 0;

                                if (new_traveltime1 < new_traveltime2 && new_traveltime1 < new_traveltime3 && new_traveltime1 < new_traveltime4 && new_traveltime1 < new_traveltime5 && new_traveltime1 < new_traveltime6)
                                {
                                    new_route = routeToWorkWith1;
                                    best_traveltime = new_traveltime1;
                                }
                                else if (new_traveltime2 < new_traveltime3 && new_traveltime2 < new_traveltime4 && new_traveltime2 < new_traveltime5 && new_traveltime2 < new_traveltime6)
                                {
                                    new_route = routeToWorkWith2;
                                    best_traveltime = new_traveltime2;
                                }
                                else if (new_traveltime3 < new_traveltime4 && new_traveltime3 < new_traveltime5 && new_traveltime3 < new_traveltime6)
                                {
                                    new_route = routeToWorkWith3;
                                    best_traveltime = new_traveltime3;
                                }
                                else if (new_traveltime4 < new_traveltime5 && new_traveltime4 < new_traveltime6)
                                {
                                    new_route = routeToWorkWith4;
                                    best_traveltime = new_traveltime4;
                                }
                                else if (new_traveltime5 < new_traveltime6)
                                {
                                    new_route = routeToWorkWith5;
                                    best_traveltime = new_traveltime5;
                                }
                                else
                                {
                                    new_route = routeToWorkWith6;
                                    best_traveltime = new_traveltime6;
                                }

                                Console.WriteLine(best_traveltime);
                                routeToWorkWith1 = new Route(day);
                                routeToWorkWith2 = new Route(day);
                                routeToWorkWith3 = new Route(day);
                                routeToWorkWith4 = new Route(day);
                                routeToWorkWith5 = new Route(day);
                                routeToWorkWith6 = new Route(day);
                                foreach (Order order in new_route.Orders)
                                {
                                    if (order.OrderNumber != 0)
                                    {
                                        routeToWorkWith1.AddOrder(order);
                                        routeToWorkWith2.AddOrder(order);
                                        routeToWorkWith3.AddOrder(order);
                                        routeToWorkWith4.AddOrder(order);
                                        routeToWorkWith5.AddOrder(order);
                                        routeToWorkWith6.AddOrder(order);
                                    }
                                }
                            }
                        }
                    }
                }

                improvestep++;
            }

            Console.WriteLine("OPT3");
            Console.WriteLine("Old Travel Time : {0}", old_route.TravelTime);
            Console.WriteLine("Best Travel Time : {0}", best_traveltime);

            RoutesFromSolution.Remove(old_route);
            RoutesFromSolution.Add(new_route);

            toStartFrom.RemoveItemFromPlanning(day, truck);
            toStartFrom.AddNewItemToPlanning(day, truck, RoutesFromSolution);

            return toStartFrom;
        }

        private void swapOrders(Order A, Order B, Route route)
        {
            //route.RemoveOrder(B);
            //route.AddOrderAt(B, A);
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
