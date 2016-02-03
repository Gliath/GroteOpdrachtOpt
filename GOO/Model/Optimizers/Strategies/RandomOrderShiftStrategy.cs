using System;
using System.Collections.Generic;

using GOO.Utilities;

namespace GOO.Model.Optimizers.Strategies
{
    public class RandomOrderShiftStrategy : Strategy
    {
        private Tuple<Days, int, List<Route>>[] Plans;
        private Route[] originalRoutes;
        private Order[] ordersShifted;
        private Order orderInFrontOfTheShiftedOrder;

        public RandomOrderShiftStrategy()
            : base()
        {
            Plans = new Tuple<Days, int, List<Route>>[2];
            originalRoutes = new Route[2];
            ordersShifted = new Order[2];
            orderInFrontOfTheShiftedOrder = null;
        }

        public override Solution executeStrategy(Solution toStartFrom)
        {
            Plans[0] = toStartFrom.GetRandomPlanning();
            Plans[1] = toStartFrom.GetRandomPlanning();

            if (Plans[0].Item3.Count == 0 || Plans[1].Item3.Count == 0 || (Plans[0].Equals(Plans[1]) && Plans[0].Item3.Count < 2))
                return toStartFrom;

            for (int i = 0; i < 2; i++)
            {
                originalRoutes[i] = Plans[i].Item3[random.Next(Plans[i].Item3.Count)];
                if (originalRoutes[i].Orders.Count < 2 || (i == 1 && originalRoutes[0].Equals(originalRoutes[1])))
                    return toStartFrom;

                int shiftOrderIndex = random.Next(originalRoutes[i].Orders.Count - 1);
                ordersShifted[i] = originalRoutes[i].Orders[shiftOrderIndex];

                if (i == 0)
                    orderInFrontOfTheShiftedOrder = shiftOrderIndex == 0 ? null : originalRoutes[0].Orders[shiftOrderIndex - 1];
            }

            double timeLimit = 0.0d; // Check if route can be swapped traveltime-wise
            foreach (Route route in Plans[1].Item3)
                if (route != originalRoutes[1])
                    timeLimit += route.TravelTime;

            timeLimit = 43200.0d - timeLimit;
            
            if (originalRoutes[1].CanAddOrderAfter(ordersShifted[0], ordersShifted[1], timeLimit)) // Check if can be shifted
            {
                originalRoutes[0].RemoveOrder(ordersShifted[0]);
                originalRoutes[1].AddOrderAt(ordersShifted[0], ordersShifted[1]);
                strategyHasExecuted = true;

                if (originalRoutes[0].Orders.Count == 1) // if route has only 0order (because other order has been shifted away...)
                {
                    toStartFrom.RemoveRouteFromPlanning(Plans[0].Item1, Plans[0].Item2, originalRoutes[0]);
                    toStartFrom.RemoveRoute(originalRoutes[0]);
                }
            }

            return toStartFrom;
        }

        public override Solution undoStrategy(Solution toStartFrom)
        {
            if (!strategyHasExecuted)
                return toStartFrom;

            originalRoutes[1].RemoveOrder(ordersShifted[0]);

            if (originalRoutes[0].Orders.Count == 1) // if route has only 0order (because other order has been shifted away and the route has been deleted)
            {
                toStartFrom.AddRoute(originalRoutes[0]);
                toStartFrom.AddRouteToPlanning(Plans[0].Item1, Plans[0].Item2, originalRoutes[0]);
            }

            if (orderInFrontOfTheShiftedOrder != null)
                originalRoutes[0].AddOrderAt(ordersShifted[0], orderInFrontOfTheShiftedOrder);
            else
                originalRoutes[0].AddOrderAtStart(ordersShifted[0]);

            return toStartFrom;
        }
    }
}