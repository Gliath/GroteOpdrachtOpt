using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GOO.Utilities;

namespace GOO.Model.Optimizers.Strategies
{
    public class RandomRouteOpt2Strategy : Strategy
    {
        private Days day;
        private int truck;
        private Route old_route;
        private Route route;

        public override Solution executeStrategy(Solution toStartFrom)
        {
            Solution toReturn = toStartFrom;

            Tuple<Days, int, List<Route>> Planning = toStartFrom.getRandomPlanningForATruck();
            day = Planning.Item1;
            truck = Planning.Item2;
            List<Route> Routes = Planning.Item3;
            route = Routes[new Random().Next(Routes.Count)];

            //copy the begin route for rollback
            old_route = new Route(day);
            foreach(Order order in route.Orders)
            {
                old_route.AddOrder(order); 
            }

            //create the route for the 2-opt check
            Route new_route = new Route(day);
            foreach (Order order in route.Orders)
            {
                new_route.AddOrder(order);
            }

            //start doing the opt-2 algorithm on the route list
            int improvestep = 0;
            while( improvestep < 3)
            {
                double best_traveltime = route.TravelTime;
                for ( int i = 0; i < route.Orders.Count-1; i ++)
                {
                    for (int k = i + 1; k < route.Orders.Count-1; k++)
                    {
                        //swap the 2 coords
                        swapOrders(route.Orders[i], route.Orders[k], new_route);
                        double new_traveltime = new_route.TravelTime;
                        if(new_traveltime < best_traveltime)
                        {
                            improvestep = 0;
                            route = new_route;
                            best_traveltime = new_traveltime;
                        }
                    }
                }
                improvestep++;
            }
            return toReturn;
        }

        private void swapOrders(Order A, Order B, Route route)
        {
            route.RemoveOrder(B);
            route.AddOrderAt(B, A);
        }

        public override Solution undoStrategy(Solution toStartFrom)
        {
            Solution toReturn = toStartFrom;
            route = old_route; // should update the corresponding refferences to the created list
            return toReturn;
        }
    }
}
