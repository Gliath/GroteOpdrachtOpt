using System;
using System.Collections.Generic;

using GOO.Utilities;

namespace GOO.Model.Optimizers.Strategies
{
    public class RandomOrderAddStrategy : Strategy
    {
        private Tuple<Days, int, List<Route>> Planning;
        private Order OrderAdded;
        private Route OriginalRoute;

        public RandomOrderAddStrategy()
            : base()
        {
            Planning = null;
            OrderAdded = null;
            OriginalRoute = null;
        }

        public override Solution executeStrategy(Solution toStartFrom)
        {
            for (int planningCounter = 0; planningCounter < 5; planningCounter++)
            {
                Planning = toStartFrom.GetRandomPlanning();
                OriginalRoute = Planning.Item3[random.Next(Planning.Item3.Count)];

                for (int counter = 0; counter < 5 && OriginalRoute.Orders.Count < 2; counter++)
                    OriginalRoute = Planning.Item3[random.Next(Planning.Item3.Count)];

                if (OriginalRoute.Orders.Count >= 2)
                    break;
            }

            if (OriginalRoute.Orders.Count < 2)
                return toStartFrom;

            Cluster cluster = toStartFrom.GetRandomCluster();
            for (int clusterCounter = 0; clusterCounter < 8 && cluster.AvailableOrdersInCluster.Count == 0; clusterCounter++)
                cluster = toStartFrom.GetRandomCluster();

            if (cluster.AvailableOrdersInCluster.Count == 0)
                return toStartFrom;

            int randomIndex = random.Next(cluster.AvailableOrdersInCluster.Count);
            OrderAdded = cluster.AvailableOrdersInCluster[randomIndex];
            cluster.AvailableOrdersInCluster.RemoveAt(randomIndex);

            int typeOfInsert = random.Next(3); // 3 choices, start, after and at the end
            switch (typeOfInsert)
            {
                case 0:
                    if (OriginalRoute.CanAddOrderAtStart(OrderAdded))
                        OriginalRoute.AddOrderAtStart(OrderAdded);
                    break;
                case 1:
                    Order orderToInsertAfter = OriginalRoute.Orders[random.Next(OriginalRoute.Orders.Count - 1)];
                    if (OriginalRoute.CanAddOrderAfter(OrderAdded, orderToInsertAfter))
                        OriginalRoute.AddOrderAt(OrderAdded, orderToInsertAfter);
                    break;
                case 2:
                    if (OriginalRoute.CanAddOrder(OrderAdded))
                        OriginalRoute.AddOrder(OrderAdded);
                    break;
                default:
                    Console.WriteLine("THE END IS NIGH! (Impossible error at RandomOrderAddStrategy, line 73~");
                    Console.Beep(); // Sign that the end is coming
                    break;
            }

            return toStartFrom;
        }

        public override Solution undoStrategy(Solution toStartFrom)
        {
            OrderAdded.ClusterOrderIsLocatedIn.AvailableOrdersInCluster.Add(OrderAdded);
            OriginalRoute.RemoveOrder(OrderAdded);

            return toStartFrom;
        }
    }
}