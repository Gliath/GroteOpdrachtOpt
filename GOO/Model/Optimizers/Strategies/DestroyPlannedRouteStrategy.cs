using System;
using System.Collections.Generic;

using GOO.Utilities;

namespace GOO.Model.Optimizers.Strategies
{
    public class DestroyPlannedRouteStrategy : Strategy
    {
        private Tuple<Days, int, List<Route>> Planning;
        private List<Order> ordersDestroyed;
        private Days dayDestroyed;
        
        public DestroyPlannedRouteStrategy()
            : base()
        {
            Planning = null;
            ordersDestroyed = null;
        }

        public override Solution executeStrategy(Solution toStartFrom)
        {
            for (int planningCounter = 0; planningCounter < 5; planningCounter++)
            {
                Planning = toStartFrom.GetRandomPlanning();
                if (Planning.Item3.Count > 0)
                    break;
            }

            if (Planning.Item3.Count == 0)
                return toStartFrom;

            int routeIndex = random.Next(Planning.Item3.Count);
            Route routeToDestroy = Planning.Item3[routeIndex];
            toStartFrom.AllRoutes.Remove(routeToDestroy);
            ordersDestroyed = routeToDestroy.Orders;
            dayDestroyed = routeToDestroy.Day;
            
            toStartFrom.RemoveRouteFromPlanning(Planning.Item1, Planning.Item2, routeToDestroy);
            toStartFrom.AvailableRoutes.RemoveAt(routeIndex);
            routeToDestroy.Destroy();

            return toStartFrom;
        }

        public override Solution undoStrategy(Solution toStartFrom)
        {
            Route routeRestored = new Route(dayDestroyed);
            foreach (Order order in ordersDestroyed)
                if(order.OrderNumber != 0)
                    routeRestored.AddOrder(order);

            toStartFrom.AllRoutes.Add(routeRestored);
            toStartFrom.AddRouteToPlanning(Planning.Item1, Planning.Item2, routeRestored);

            return toStartFrom;
        }
    }
}