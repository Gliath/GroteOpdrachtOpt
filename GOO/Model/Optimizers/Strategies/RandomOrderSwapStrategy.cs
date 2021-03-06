﻿using System;
using System.Collections.Generic;

using GOO.Utilities;

namespace GOO.Model.Optimizers.Strategies
{
    public class RandomOrderSwapStrategy : Strategy
    {
        private Tuple<Days, int, List<Route>>[] Plans;
        private Route[] originalRoutes;
        private Order[] ordersSwapped;
        private Order[] ordersBehindOrderMarkedForSwap;

        public RandomOrderSwapStrategy()
            : base()
        {
            Plans = new Tuple<Days, int, List<Route>>[2];
            originalRoutes = new Route[2];
            ordersSwapped = new Order[2];
            ordersBehindOrderMarkedForSwap = new Order[2];
        }

        public override Solution executeStrategy(Solution toStartFrom)
        {
            Plans[0] = toStartFrom.GetRandomPlanning();
            Plans[1] = toStartFrom.GetRandomPlanning();

            if (Plans[0].Item3.Count == 0 || Plans[1].Item3.Count == 0 || Plans[0].Item1 != Plans[1].Item1 || (Plans[0].Equals(Plans[1]) && Plans[0].Item3.Count < 2))
                return toStartFrom;

            for (int i = 0; i < 2; i++)
            {
                originalRoutes[i] = Plans[i].Item3[random.Next(Plans[i].Item3.Count)];
                if (originalRoutes[i].Orders.Count < 2 || (i == 1 && originalRoutes[0].Equals(originalRoutes[1])))
                    return toStartFrom;

                int swapOrderIndex = random.Next(originalRoutes[i].Orders.Count - 1);
                ordersSwapped[i] = originalRoutes[i].Orders[swapOrderIndex];
                ordersBehindOrderMarkedForSwap[i] = swapOrderIndex == 0 ? null : originalRoutes[i].Orders[swapOrderIndex - 1];
            }

            for (int i = 0; i < 2; i++)
            {
                double timeLimit = 0.0d; // Check if route can be swapped traveltime-wise
                foreach (Route route in Plans[i].Item3)
                    if (route != originalRoutes[i])
                        timeLimit += route.TravelTime;

                timeLimit = 43200.0d - timeLimit;

                if (!originalRoutes[i].CanSwapOrderFromDifferentRoutes(ordersSwapped[(i + 1) % 2], ordersSwapped[i], timeLimit))
                    return toStartFrom; // if a route could not be swapped...
            }

            for (int i = 0; i < 2; i++)
                originalRoutes[i].RemoveOrder(ordersSwapped[i]);

            for (int i = 0; i < 2; i++)
                if (ordersBehindOrderMarkedForSwap[i] == null)
                    originalRoutes[i].AddOrderAtStart(ordersSwapped[(i + 1) % 2]);
                else
                    originalRoutes[i].AddOrderAt(ordersSwapped[(i + 1) % 2], ordersBehindOrderMarkedForSwap[i]);

            strategyHasExecuted = true;
            return toStartFrom;
        }

        public override Solution undoStrategy(Solution toStartFrom)
        {
            if (!strategyHasExecuted)
                return toStartFrom;

            for (int i = 0; i < 2; i++)
                originalRoutes[i].RemoveOrder(ordersSwapped[(i + 1) % 2]);

            for (int i = 0; i < 2; i++)
                if (ordersBehindOrderMarkedForSwap[i] == null)
                    originalRoutes[i].AddOrderAtStart(ordersSwapped[i]);
                else
                    originalRoutes[i].AddOrderAt(ordersSwapped[i], ordersBehindOrderMarkedForSwap[i]);

            return toStartFrom;
        }
    }
}