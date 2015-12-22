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
            new_route = new Route(day);
            foreach (Order order in old_route.Orders)
            {
                routeToWorkWith1.AddOrder(order);
                routeToWorkWith2.AddOrder(order);
                routeToWorkWith3.AddOrder(order);
                new_route.AddOrder(order);
            }

            //start doing the opt-3 algorithm on the route list
            double best_traveltime = old_route.TravelTime;
            double new_traveltime1 = double.MaxValue;
            double new_traveltime2 = double.MaxValue;
            double new_traveltime3 = double.MaxValue;

            int improvestep = 0;
            while (improvestep < 30)
            {
                for (int i = 0; i < routeToWorkWith1.Orders.Count - 3; i++)
                {
                    for (int k = i + 1; k < routeToWorkWith1.Orders.Count - 2; k++)
                    {
                        for (int j = k + 1; j < routeToWorkWith1.Orders.Count - 1; j++)
                        {
                            //swap the 2 coords
                            swapOrders(routeToWorkWith1.Orders[j], routeToWorkWith1.Orders[k], routeToWorkWith1);
                            swapOrders(routeToWorkWith1.Orders[j], routeToWorkWith1.Orders[i], routeToWorkWith1);
                            new_traveltime1 = routeToWorkWith1.TravelTime;

                            swapOrders(routeToWorkWith2.Orders[i], routeToWorkWith2.Orders[k], routeToWorkWith2);
                            swapOrders(routeToWorkWith2.Orders[i], routeToWorkWith2.Orders[j], routeToWorkWith2);
                            new_traveltime2 = routeToWorkWith2.TravelTime;

                            swapOrders(routeToWorkWith3.Orders[j], routeToWorkWith3.Orders[i], routeToWorkWith3);
                            swapOrders(routeToWorkWith3.Orders[j], routeToWorkWith3.Orders[k], routeToWorkWith3);
                            new_traveltime3 = routeToWorkWith3.TravelTime;

                            if (new_traveltime1 < best_traveltime || new_traveltime2 < best_traveltime || new_traveltime3 < best_traveltime)
                            {
                                improvestep = 0;

                                if (new_traveltime1 < new_traveltime2 && new_traveltime1 < new_traveltime3)
                                {
                                    new_route = routeToWorkWith1;
                                    best_traveltime = new_traveltime1;
                                }
                                else if (new_traveltime2 <= new_traveltime1 && new_traveltime2 < new_traveltime3)
                                {
                                    new_route = routeToWorkWith2;
                                    best_traveltime = new_traveltime2;
                                }
                                else
                                {
                                    new_route = routeToWorkWith3;
                                    best_traveltime = new_traveltime3;
                                }

                                routeToWorkWith1 = new Route(day);
                                routeToWorkWith2 = new Route(day);
                                routeToWorkWith3 = new Route(day);
                                foreach (Order order in new_route.Orders)
                                {
                                    if (order.OrderNumber != 0)
                                    {
                                        routeToWorkWith1.AddOrder(order);
                                        routeToWorkWith2.AddOrder(order);
                                        routeToWorkWith3.AddOrder(order);
                                    }
                                }
                            }
                        }
                    }
                }

                improvestep++;
            }

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
            route.RemoveOrder(B);
            route.AddOrderAt(B, A);
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
