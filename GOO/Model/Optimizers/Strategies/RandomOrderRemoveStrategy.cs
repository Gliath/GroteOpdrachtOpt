using System;
using System.Collections.Generic;

using GOO.Utilities;

namespace GOO.Model.Optimizers.Strategies
{
    public class RandomOrderRemoveStrategy : Strategy
    {
        private Tuple<Days, int, List<Route>> Planning;
        private Order OrderRemoved;
        private Order OrderBefore;
        private Route OriginalRoute;

        public RandomOrderRemoveStrategy()
            : base()
        {
            Planning = null;
            OrderRemoved = null;
            OriginalRoute = null;
            OrderBefore = null;
        }

        public override Solution executeStrategy(Solution toStartFrom)
        {
            for (int planningCounter = 0; planningCounter < 5; planningCounter++)
            {
                Planning = toStartFrom.GetRandomPlanning();
                if (Planning.Item3.Count == 0)
                    continue;

                OriginalRoute = Planning.Item3[random.Next(Planning.Item3.Count)];

                for (int counter = 0; counter < 5 && OriginalRoute.Orders.Count < 3; counter++)
                    OriginalRoute = Planning.Item3[random.Next(Planning.Item3.Count)];

                if (OriginalRoute.Orders.Count >= 3)
                    break;
            }

            if (OriginalRoute.Orders.Count < 3)
                return toStartFrom;

            int orderIndex = random.Next(OriginalRoute.Orders.Count - 1);
            OrderRemoved = OriginalRoute.Orders[orderIndex];
            OrderBefore = orderIndex == 0 ? null : OriginalRoute.Orders[orderIndex - 1];
            OrderRemoved.ClusterOrderIsLocatedIn.AvailableOrdersInCluster.Add(OrderRemoved);
            OriginalRoute.RemoveOrder(OrderRemoved);

            if (OriginalRoute.Orders.Count == 1) // Basically delete the route (remove it from planning)
                toStartFrom.RemoveRouteFromPlanning(Planning.Item1, Planning.Item2, OriginalRoute);

            return toStartFrom;
        }

        public override Solution undoStrategy(Solution toStartFrom)
        {
            if (OriginalRoute.Orders.Count == 1) // If empty 
                toStartFrom.AddRouteToPlanning(Planning.Item1, Planning.Item2, OriginalRoute);

            OrderRemoved.ClusterOrderIsLocatedIn.AvailableOrdersInCluster.Remove(OrderRemoved);
            if (OrderBefore == null)
                OriginalRoute.AddOrderAtStart(OrderRemoved);
            else
                OriginalRoute.AddOrderAt(OrderRemoved, OrderBefore);

            return toStartFrom;
        }
    }
}