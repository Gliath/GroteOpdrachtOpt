using System;
using System.Collections.Generic;

using GOO.Utilities;

namespace GOO.Model.Optimizers.Strategies
{
    public class DestroyRouteStrategy : Strategy
    {
        private Tuple<Days, int, List<Route>> Planning;
        private List<Order> ordersDestroyed;
        private Days dayDestroyed;
        
        public DestroyRouteStrategy()
            : base()
        {
            Planning = null;
            ordersDestroyed = null;
        }

        public override Solution executeStrategy(Solution toStartFrom)
        {
            for (int planningCounter = 0; planningCounter < 5; planningCounter++)
            {
                Planning = toStartFrom.getRandomPlanning();
                if (Planning.Item3.Count > 0)
                    break;
            }

            if (Planning.Item3.Count == 0)
                return toStartFrom;

            int routeIndex = random.Next(Planning.Item3.Count);
            Route routeDestroyed = Planning.Item3[routeIndex];
            ordersDestroyed = routeDestroyed.Orders;
            dayDestroyed = routeDestroyed.Day;
            Planning.Item3.RemoveAt(routeIndex);

            foreach (Order order in ordersDestroyed)
                routeDestroyed.RemoveOrder(order);

            toStartFrom.RemoveItemFromPlanning(Planning.Item1, Planning.Item2);
            toStartFrom.AddNewItemToPlanning(Planning.Item1, Planning.Item2, Planning.Item3);

            return toStartFrom;
        }

        public override Solution undoStrategy(Solution toStartFrom)
        {
            Route routeRestored = new Route(dayDestroyed);
            foreach (Order order in ordersDestroyed)
                routeRestored.AddOrder(order);

            Planning.Item3.Add(routeRestored);

            toStartFrom.RemoveItemFromPlanning(Planning.Item1, Planning.Item2);
            toStartFrom.AddNewItemToPlanning(Planning.Item1, Planning.Item2, Planning.Item3);

            return toStartFrom;
        }
    }
}