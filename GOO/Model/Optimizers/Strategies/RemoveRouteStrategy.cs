using System;
using System.Collections.Generic;

using GOO.Utilities;

namespace GOO.Model.Optimizers.Strategies
{
    public class RemoveRouteStrategy : Strategy
    {
        private Tuple<Days, int, List<Route>> Planning;
        private Route routeDeleted;

        public RemoveRouteStrategy()
            : base()
        {
            Planning = null;
            routeDeleted = null;
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
            routeDeleted = Planning.Item3[routeIndex];
            Planning.Item3.RemoveAt(routeIndex);
            OrdersTracker.Instance.AllRoutes.Add(routeDeleted);

            toStartFrom.RemoveItemFromPlanning(Planning.Item1, Planning.Item2);
            toStartFrom.AddNewItemToPlanning(Planning.Item1, Planning.Item2, Planning.Item3);

            return toStartFrom;
        }

        public override Solution undoStrategy(Solution toStartFrom)
        {
            OrdersTracker.Instance.AllRoutes.Remove(routeDeleted);
            Planning.Item3.Add(routeDeleted);

            toStartFrom.RemoveItemFromPlanning(Planning.Item1, Planning.Item2);
            toStartFrom.AddNewItemToPlanning(Planning.Item1, Planning.Item2, Planning.Item3);

            return toStartFrom;
        }
    }
}