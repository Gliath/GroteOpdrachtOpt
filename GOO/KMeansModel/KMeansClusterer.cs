using System;
using System.Collections.Generic;
using System.Windows;

using GOO.Model;
using GOO.Utilities;

namespace GOO.KMeansModel
{
    public class KMeansClusterer
    {
        private static int startingAmountOfClusters = 30;

        private Order[] allOrders;

        public KMeansClusterer(Order[] allOrders, int startingAmountOfClusters)
        {
            this.allOrders = allOrders;
            KMeansClusterer.startingAmountOfClusters = startingAmountOfClusters;
        }

        public List<Cluster> createKClusters()
        {
            return createKClusters(KMeansClusterer.startingAmountOfClusters, this.createStartOrders(this.allOrders, KMeansClusterer.startingAmountOfClusters), this.allOrders);
        }

        public List<Cluster> createKClusters(int amountOfClusters, List<Order> startingPoints, Order[] allOrders)
        {
            List<Cluster> toReturn = new List<Cluster>();

            foreach (Order order in startingPoints)
            {
                toReturn.Add(new Cluster(new Point(order.X, order.Y)));
            }

            for (int i = 0; i < 3000; i++) // Try to reposition the center-point 3000 times for each cluster
            {
                assignOrdersToClusters(toReturn, allOrders);
                toReturn.RemoveAll(c => c.OrdersInCluster.Count == 0);
            }

            foreach (Cluster cluster in toReturn)
            {
                List<Days> restrictions = new List<Days>();
                List<OrderFrequency> frequenciesPresent = new List<OrderFrequency>();

                foreach (Order order in cluster.OrdersInCluster)
                    if (order.Frequency != OrderFrequency.PWK1 && !frequenciesPresent.Contains(order.Frequency))
                    {
                        restrictions.AddRange(DayRestrictionFactory.GetDayRestrictions(order.Frequency));
                        frequenciesPresent.Add(order.Frequency);
                    }

                cluster.DaysRestrictions = restrictions;
            }
            
            return toReturn;
        }

        private void assignOrdersToClusters(List<Cluster> clusters, Order[] toAssign)
        {
            // Grouping orders based on Euclidean distance method
            // Note : calculating the root is currently not neccessary due to look at relative points
            foreach (Cluster cluster in clusters)
            {
                cluster.RemoveAllOrdersFromCluster();
            }

            foreach (Order order in allOrders)
            {
                Cluster nearest = null;
                double nearestEuclidianDistance = Double.MaxValue;
                foreach (Cluster cluster in clusters)
                {
                    double distance = Math.Pow(order.X - cluster.CenterPoint.X, 2) + Math.Pow(order.Y - cluster.CenterPoint.Y, 2);
                    if (distance < nearestEuclidianDistance)
                    {
                        nearestEuclidianDistance = distance;
                        nearest = cluster;
                    }
                }
                nearest.AddOrderToCluster(order);
            }
        }

        private List<Order> createStartOrders(Order[] allOrders, int amountOfClusters)
        {
            List<Order> toReturn = new List<Order>();

            List<List<Order>> freqOrders = new List<List<Order>>(); // The following order is intentional!
            freqOrders.Add(this.findOrdersWithFrequency(allOrders, OrderFrequency.PWK3));
            freqOrders.Add(this.findOrdersWithFrequency(allOrders, OrderFrequency.PWK4));

            int numMaxAdditions = (amountOfClusters - freqOrders.Count + 1) / 2;

            freqOrders.Add(this.findOrdersWithFrequencyRandomly(allOrders, OrderFrequency.PWK2, numMaxAdditions));
            freqOrders.Add(this.findOrdersWithFrequencyRandomly(allOrders, OrderFrequency.PWK1, numMaxAdditions));

            foreach (List<Order> orders in freqOrders)
            {
                for (int i = 0; i < orders.Count; i++)
                {
                    if (toReturn.Count >= amountOfClusters)
                        return toReturn;

                    toReturn.Add(orders[i]);
                }
            }
            return toReturn;
        }

        private List<Order> findOrdersWithFrequency(Order[] toLookIn, OrderFrequency toLookFor)
        {
            List<Order> toReturn = new List<Order>();

            for (int i = 0; i < toLookIn.Length; i++)
            {
                Order order = toLookIn[i];
                if (order.Frequency == toLookFor)
                    toReturn.Add(order);
            }
            return toReturn;
        }

        private List<Order> findOrdersWithFrequencyRandomly(Order[] toLookIn, OrderFrequency toLookFor, int maxAdditions)
        {
            int numAdditions = 0;
            List<Order> toReturn = new List<Order>();
            Random random = new Random();

            for (int i = 0; i < toLookIn.Length; i++)
            {
                Order order = toLookIn[random.Next(toLookIn.Length)];
                if (order.Frequency == toLookFor)
                {
                    if (numAdditions > maxAdditions)
                        return toReturn;
                    else if(!toReturn.Contains(order))
                    {
                        toReturn.Add(order);
                        numAdditions++;
                    }
                }
            }

            return toReturn;
        }
    }
}
