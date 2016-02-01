using System;
using System.Collections.Generic;

using GOO.Utilities;

namespace GOO.Model.Optimizers.Strategies
{
    public class AddRouteStrategy : Strategy
    {
        private Route routeCreated;

        public AddRouteStrategy()
            : base()
        {
            routeCreated = null;
        }

        public override Solution executeStrategy(Solution toStartFrom)
        {
            var dayArray = Enum.GetValues(typeof(Days));
            Days day = Days.None;
            while (day.Equals(Days.None))
            {
                int dayIndex = random.Next(dayArray.Length);
                day = (Days)dayArray.GetValue(dayIndex);
            }

            routeCreated = new Route(day);
            Cluster cluster = toStartFrom.GetRandomCluster();

            for (int i = 0; i < 64; i++)
            {
                for (int clusterCounter = 0; clusterCounter < 16 && cluster.AvailableOrdersInCluster.Count == 0; clusterCounter++)
                    cluster = toStartFrom.GetRandomCluster();

                if (cluster.AvailableOrdersInCluster.Count == 0)
                    break;

                int randomIndex = random.Next(cluster.AvailableOrdersInCluster.Count);
                Order order = cluster.AvailableOrdersInCluster[randomIndex];

                if (routeCreated.CanAddOrder(order))
                    routeCreated.AddOrder(order);
                else
                    continue;

                cluster.AvailableOrdersInCluster.RemoveAt(randomIndex);
            }

            if (routeCreated.Orders.Count > 1) // Does have orders
            {
                toStartFrom.AllRoutes.Add(routeCreated);
                toStartFrom.AvailableRoutes.Add(routeCreated);
            }

            return toStartFrom;
        }

        public override Solution undoStrategy(Solution toStartFrom)
        {
            foreach (Order order in routeCreated.Orders)
                if (!order.ClusterOrderIsLocatedIn.AvailableOrdersInCluster.Contains(order))
                    order.ClusterOrderIsLocatedIn.AvailableOrdersInCluster.Add(order);

            toStartFrom.AllRoutes.Remove(routeCreated);
            toStartFrom.AvailableRoutes.Remove(routeCreated);
            routeCreated.Destroy();

            return toStartFrom;
        }
    }
}