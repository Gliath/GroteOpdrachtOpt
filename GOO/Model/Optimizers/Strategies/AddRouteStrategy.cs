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
            var allClusters = toStartFrom.GetAllClusters();
            int clusterIndex = random.Next(allClusters.Count);

            for (int i = 0; i < 64; i++)
            {
                for (int clusterCounter = 0; clusterCounter < 16 && allClusters[clusterIndex].AvailableOrdersInCluster.Count == 0; clusterCounter++)
                    clusterIndex = random.Next(allClusters.Count);

                if (allClusters[clusterIndex].AvailableOrdersInCluster.Count == 0)
                    break;

                int randomIndex = random.Next(allClusters[clusterIndex].AvailableOrdersInCluster.Count);
                Order order = allClusters[clusterIndex].AvailableOrdersInCluster[randomIndex];

                if (!routeCreated.CanAddOrder(order))
                    continue;

                routeCreated.AddOrder(order);
                toStartFrom.GetAllClusters()[clusterIndex].AvailableOrdersInCluster.RemoveAt(randomIndex);
            }

            if (routeCreated.Orders.Count > 1) // Does the route have orders added to it
            {
                toStartFrom.AddRoute(routeCreated);
                strategyHasExecuted = true;
            }

            return toStartFrom;
        }

        public override Solution undoStrategy(Solution toStartFrom)
        {
            if (!strategyHasExecuted)
                return toStartFrom;

            foreach (Order order in routeCreated.Orders)
                order.AddAvailableOrderBackToCluster();

            toStartFrom.RemoveRoute(routeCreated);
            routeCreated.Destroy();

            return toStartFrom;
        }
    }
}