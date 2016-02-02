using System;
using System.Collections.Generic;

using GOO.Utilities;

namespace GOO.Model.Optimizers.Strategies
{
    public class RandomOrderSwapStrategy : Strategy
    {
        private Tuple<Days, int, List<Route>>[] Plans;
        private Route[] oldRoutes;
        private Route[] newRoutes;

        public RandomOrderSwapStrategy()
            : base()
        {
            Plans = new Tuple<Days, int, List<Route>>[2];
            oldRoutes = new Route[2];
            newRoutes = new Route[2];
        }

        public override Solution executeStrategy(Solution toStartFrom)
        {
            Plans[0] = toStartFrom.GetRandomPlanning();
            for (int planCounter = 0; planCounter < 8 && Plans[0].Item3.Count == 0; planCounter++)
                Plans[0] = toStartFrom.GetRandomPlanning();

            Plans[1] = toStartFrom.GetRandomPlanning();
            for (int planCounter = 0; planCounter < 8 && (Plans[0].Item1 != Plans[1].Item1 || Plans[1].Item3.Count == 0); planCounter++)
                Plans[1] = toStartFrom.GetRandomPlanning();

            if (Plans[0].Item3.Count == 0 || Plans[1].Item3.Count == 0 || (Plans[0].Equals(Plans[1]) && Plans[0].Item3.Count < 2))
                return toStartFrom;

            Order[] ordersToSwitch = new Order[2];

            for (int i = 0; i < 2; i++)
            {
                oldRoutes[i] = Plans[i].Item3[random.Next(Plans[i].Item3.Count)];

                for (int routeCounter = 0; routeCounter < 8 && (oldRoutes[i].Orders.Count < 2 || (i == 1 && oldRoutes[0].Equals(oldRoutes[1]))); routeCounter++)
                    oldRoutes[i] = Plans[i].Item3[random.Next(Plans[i].Item3.Count)];

                if (oldRoutes[i].Orders.Count < 2 || (i == 1 && oldRoutes[0].Equals(oldRoutes[1])))
                    return toStartFrom;

                newRoutes[i] = new Route(Plans[i].Item1);
                foreach (Order order in oldRoutes[i].Orders)
                    if (order.OrderNumber != 0)
                        newRoutes[i].AddOrder(order);

                ordersToSwitch[i] = newRoutes[i].Orders[random.Next(newRoutes[i].Orders.Count - 1)];
            }

            for (int i = 0; i < 2; i++)
            {
                newRoutes[i].AddOrderAt(ordersToSwitch[(i + 1) % 2], ordersToSwitch[i]);
                newRoutes[i].RemoveOrder(ordersToSwitch[i]);

                if (!newRoutes[i].isValid())
                    return toStartFrom;
            }

            for (int i = 0; i < 2; i++)
            {
                toStartFrom.RemoveRouteFromPlanning(Plans[i].Item1, Plans[i].Item2, oldRoutes[i]);
                toStartFrom.AddRouteToPlanning(Plans[i].Item1, Plans[i].Item2, newRoutes[i]);

                toStartFrom.ReplaceRoutes(oldRoutes[i], newRoutes[i]);
            }

            return toStartFrom;
        }

        public override Solution undoStrategy(Solution toStartFrom)
        {
            for (int i = 0; i < 2; i++)
                if (newRoutes[i] == null || !newRoutes[i].isValid())
                    return toStartFrom;

            for (int i = 0; i < 2; i++)
            {
                toStartFrom.RemoveRouteFromPlanning(Plans[i].Item1, Plans[i].Item2, newRoutes[i]);
                toStartFrom.AddRouteToPlanning(Plans[i].Item1, Plans[i].Item2, oldRoutes[i]);

                toStartFrom.ReplaceRoutes(newRoutes[i], oldRoutes[i]);
            }

            return toStartFrom;
        }
    }
}