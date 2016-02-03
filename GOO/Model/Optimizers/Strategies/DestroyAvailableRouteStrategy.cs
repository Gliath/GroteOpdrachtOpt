using System;
using System.Collections.Generic;

using GOO.Utilities;

namespace GOO.Model.Optimizers.Strategies
{
    public class DestroyAvailableRouteStrategy : Strategy
    {
        private List<Order> ordersDestroyed;
        private Days dayDestroyed;

        public DestroyAvailableRouteStrategy()
            : base()
        {
            ordersDestroyed = null;
        }

        public override Solution executeStrategy(Solution toStartFrom)
        {
            if (toStartFrom.AvailableRoutes.Count == 0)
                return toStartFrom;

            int routeIndex = random.Next(toStartFrom.AvailableRoutes.Count);
            Route routeToDestroy = toStartFrom.AvailableRoutes[routeIndex];
            toStartFrom.RemoveRoute(routeToDestroy);
            ordersDestroyed = routeToDestroy.Orders;
            dayDestroyed = routeToDestroy.Day;
            routeToDestroy.Destroy();

            strategyHasExecuted = true;
            return toStartFrom;
        }

        public override Solution undoStrategy(Solution toStartFrom)
        {
            if (!strategyHasExecuted)
                return toStartFrom;

            Route routeRestored = new Route(dayDestroyed);
            foreach (Order order in ordersDestroyed)
            {
                routeRestored.AddOrder(order);
                order.RemoveAvailableOrderFromCluster();
            }
            toStartFrom.AddRoute(routeRestored);

            return toStartFrom;
        }
    }
}