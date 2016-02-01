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
        private List<Route>[] RoutesFromPlanning;

        public RandomOrderSwapStrategy()
            : base()
        {
            Plans = new Tuple<Days, int, List<Route>>[2];
            oldRoutes = new Route[2];
            newRoutes = new Route[2];
            RoutesFromPlanning = new List<Route>[2];
        }

        public override Solution executeStrategy(Solution toStartFrom)
        {
            if (toStartFrom.GetEntirePlanning().Count < 2)
                return toStartFrom;

            Plans[0] = toStartFrom.GetRandomPlanning();
            Plans[1] = toStartFrom.GetRandomPlanning();
            while (Plans[0].Item1 != Plans[1].Item1)
                Plans[1] = toStartFrom.GetRandomPlanning();

            Order[] ordersToSwitch = new Order[2];

            for (int i = 0; i < 2; i++)
            {
                RoutesFromPlanning[i] = Plans[i].Item3;
                oldRoutes[i] = RoutesFromPlanning[i][random.Next(RoutesFromPlanning[i].Count)];
                while (oldRoutes[i].Orders.Count < 2)
                    oldRoutes[i] = RoutesFromPlanning[i][random.Next(RoutesFromPlanning[i].Count)];

                newRoutes[i] = new Route(Plans[i].Item1);
                foreach (Order order in oldRoutes[i].Orders)
                    newRoutes[i].AddOrder(order);

                ordersToSwitch[i] = newRoutes[i].Orders[random.Next(newRoutes[i].Orders.Count - 2)];
            }

            for (int i = 0; i < 2; i++) // Check if can be swapped
            {
                newRoutes[i].AddOrderAt(ordersToSwitch[(i + 1) % 2], ordersToSwitch[i]);
                newRoutes[i].RemoveOrder(ordersToSwitch[i]);
            }

            for (int i = 0; i < 2; i++)
            {
                RoutesFromPlanning[i].Remove(oldRoutes[i]);
                RoutesFromPlanning[i].Add(newRoutes[i]);

                toStartFrom.RemoveItemFromPlanning(Plans[i].Item1, Plans[i].Item2);
                toStartFrom.AddNewItemToPlanning(Plans[i].Item1, Plans[i].Item2, RoutesFromPlanning[i]);
            }

            return toStartFrom;
        }

        public override Solution undoStrategy(Solution toStartFrom)
        {
            for (int i = 0; i < 2; i++)
            {
                RoutesFromPlanning[i].Remove(newRoutes[i]);
                RoutesFromPlanning[i].Add(oldRoutes[i]);

                toStartFrom.RemoveItemFromPlanning(Plans[i].Item1, Plans[i].Item2);
                toStartFrom.AddNewItemToPlanning(Plans[i].Item1, Plans[i].Item2, RoutesFromPlanning[i]);
            }

            return toStartFrom;
        }
    }
}