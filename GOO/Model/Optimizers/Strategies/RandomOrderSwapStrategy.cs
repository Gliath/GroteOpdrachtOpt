using System;
using System.Collections.Generic;

using GOO.Utilities;

namespace GOO.Model.Optimizers.Strategies
{
    public class RandomOrderSwapStrategy : Strategy
    {
        private Tuple<Days, int, List<Route>>[] Plans;
        private Route[] originalRoutes;
        private Order[] ordersSwapped;

        public RandomOrderSwapStrategy()
            : base()
        {
            Plans = new Tuple<Days, int, List<Route>>[2];
            originalRoutes = new Route[2];
            ordersSwapped = new Order[2];
        }

        public override Solution executeStrategy(Solution toStartFrom)
        {
            Plans[0] = toStartFrom.GetRandomPlanning();
            for (int planCounter = 0; planCounter < 8 && Plans[0].Item3.Count == 0; planCounter++)
                Plans[0] = toStartFrom.GetRandomPlanning();

            Plans[1] = toStartFrom.GetRandomPlanning();
            for (int planCounter = 0; planCounter < 8 && (Plans[0].Item1 != Plans[1].Item1 || Plans[1].Item3.Count == 0); planCounter++)
                Plans[1] = toStartFrom.GetRandomPlanning();

            if (Plans[0].Item3.Count == 0 || Plans[1].Item3.Count == 0 || Plans[0].Item1 != Plans[1].Item1 || (Plans[0].Equals(Plans[1]) && Plans[0].Item3.Count < 2))
                return toStartFrom;

            for (int i = 0; i < 2; i++)
            {
                originalRoutes[i] = Plans[i].Item3[random.Next(Plans[i].Item3.Count)];

                for (int routeCounter = 0; routeCounter < 8 && (originalRoutes[i].Orders.Count < 2 || (i == 1 && originalRoutes[0].Equals(originalRoutes[1]))); routeCounter++)
                    originalRoutes[i] = Plans[i].Item3[random.Next(Plans[i].Item3.Count)];

                if (originalRoutes[i].Orders.Count < 2 || (i == 1 && originalRoutes[0].Equals(originalRoutes[1])))
                    return toStartFrom;

                int swapOrderIndex = random.Next(originalRoutes[i].Orders.Count - 1);
                ordersSwapped[i] = originalRoutes[i].Orders[swapOrderIndex];
            }

            for (int i = 0; i < 2; i++)
                if (!originalRoutes[i].CanSwapOrderFromDifferentRoutes(ordersSwapped[(i + 1) % 2], ordersSwapped[i]))
                    return toStartFrom; // if a route could not be swapped...

            for (int i = 0; i < 2; i++)
            {
                originalRoutes[i].AddOrderAt(ordersSwapped[(i + 1) % 2], ordersSwapped[i]);
                originalRoutes[i].RemoveOrder(ordersSwapped[i]);
            }

            strategyHasExecuted = true;
            return toStartFrom;
        }

        public override Solution undoStrategy(Solution toStartFrom)
        {
            if (!strategyHasExecuted)
                return toStartFrom;

            for (int i = 0; i < 2; i++)
            {
                originalRoutes[i].AddOrderAt(ordersSwapped[i], ordersSwapped[(i + 1) % 2]);
                originalRoutes[i].RemoveOrder(ordersSwapped[(i + 1) % 2]);
            }
            
            return toStartFrom;
        }
    }
}