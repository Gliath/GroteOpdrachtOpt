﻿using System;
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
                day = (Days)dayArray.GetValue(random.Next(dayArray.Length) - 1);

            routeCreated = new Route(day);
            Cluster cluster = toStartFrom.getRandomCluster();

            for (int i = 0; i < 64; i++)
            {
                while (cluster.OrdersInCluster.Count == 0)
                    cluster = toStartFrom.getRandomCluster();

                int randomIndex = random.Next(cluster.OrdersInCluster.Count);
                Order order = cluster.OrdersInCluster[randomIndex];

                if (routeCreated.CanAddOrder(order))
                    routeCreated.AddOrder(order);
                else
                    continue;

                cluster.OrdersInCluster.RemoveAt(randomIndex);
            }

            if (routeCreated.Orders.Count > 1) // Does have orders
                OrdersTracker.Instance.AllRoutes.Add(routeCreated);

            return toStartFrom; // Nottin' really changed in the solution itself
        }

        public override Solution undoStrategy(Solution toStartFrom)
        {
            foreach (Order order in routeCreated.Orders)
                if (!order.ClusterOrderIsLocatedIn.OrdersInCluster.Contains(order))
                    order.ClusterOrderIsLocatedIn.OrdersInCluster.Add(order);

            OrdersTracker.Instance.AllRoutes.Remove(routeCreated);

            return toStartFrom; // Again nottin' changed
        }
    }
}