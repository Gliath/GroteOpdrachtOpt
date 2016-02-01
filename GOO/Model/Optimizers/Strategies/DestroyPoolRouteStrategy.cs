using System;
using System.Collections.Generic;

using GOO.Utilities;

namespace GOO.Model.Optimizers.Strategies
{
    public class DestroyPoolRouteStrategy : Strategy
    {
        private Tuple<Days, int, List<Route>> Planning;
        private List<Order> ordersDestroyed;
        private Days dayDestroyed;
        
        public DestroyPoolRouteStrategy()
            : base()
        {
            Planning = null;
            ordersDestroyed = null;
        }

        public override Solution executeStrategy(Solution toStartFrom)
        {
            if (toStartFrom.AllRoutes.Count == 0)
                return toStartFrom;

            int routeIndex = random.Next(toStartFrom.AllRoutes.Count);
            Route routeToDestroy = toStartFrom.AllRoutes[routeIndex];
            toStartFrom.AllRoutes.RemoveAt(routeIndex);
            ordersDestroyed = routeToDestroy.Orders;
            dayDestroyed = routeToDestroy.Day;
            routeToDestroy.Destroy();

            return toStartFrom;
        }

        public override Solution undoStrategy(Solution toStartFrom)
        {
            Route routeRestored = new Route(dayDestroyed);
            foreach (Order order in ordersDestroyed)
                routeRestored.AddOrder(order);

            toStartFrom.AllRoutes.Add(routeRestored);

            return toStartFrom;
        }
    }
}