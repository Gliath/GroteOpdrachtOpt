using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

using GOO.Utilities;

namespace GOO.Model
{
    public class Clusterer
    {
        private static int startingAmountOfClusters = 30;
        private Random random;

        private Dictionary<int, Order> allOrders;

        public Clusterer(Dictionary<int, Order> allOrders, int startingAmountOfClusters)
        {
            this.allOrders = allOrders;
            this.random = new Random();
            Clusterer.startingAmountOfClusters = startingAmountOfClusters;
        }

        public List<Cluster> createClusters()
        {
            return createClusters(Clusterer.startingAmountOfClusters, this.createStartOrders(Clusterer.startingAmountOfClusters));
        }

        public List<Cluster> createClusters(int amountOfClusters, List<Order> startingPoints)
        {
            List<Cluster> toReturn = new List<Cluster>();

            foreach (Order order in startingPoints)
                toReturn.Add(new Cluster(new Point(order.X, order.Y)));

            for (int i = 0; i < 3000; i++) // Try to reposition the center-point 3000 times for each cluster
            {
                assignOrdersToClustersEuclidean(toReturn);
                toReturn.RemoveAll(c => c.AvailableOrdersInCluster.Count == 0);
            }

            foreach (Cluster cluster in toReturn)
            {
                List<Days> restrictions = new List<Days>();
                List<OrderFrequency> frequenciesPresent = new List<OrderFrequency>();
            }

            return toReturn;
        }

        private void assignOrdersToClustersEuclidean(List<Cluster> clusters)
        {
            // Grouping orders based on Euclidean distance method
            // Note : calculating the root is currently not neccessary due to look at relative points
            foreach (Cluster cluster in clusters)
                cluster.RemoveAllOrdersFromCluster();

            foreach (Order order in allOrders.Values)
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

        private List<Order> createStartOrders(int amountOfClusters)
        {
            List<Order> toReturn = new List<Order>();

            List<List<Order>> freqOrders = new List<List<Order>>(); // The following order is intentional!
            freqOrders.Add(this.findOrdersWithFrequency(OrderFrequency.PWK3));
            freqOrders.Add(this.findOrdersWithFrequency(OrderFrequency.PWK4));

            int numMaxAdditions = (amountOfClusters - freqOrders.Count + 1) / 2;

            freqOrders.Add(this.findOrdersWithFrequencyRandomly(OrderFrequency.PWK2, numMaxAdditions));
            freqOrders.Add(this.findOrdersWithFrequencyRandomly(OrderFrequency.PWK1, numMaxAdditions));

            foreach (List<Order> orders in freqOrders)
            {
                for (int i = 0; i < orders.Count; i++)
                {
                    toReturn.Add(orders[i]);

                    if (toReturn.Count >= amountOfClusters)
                        return toReturn;
                }
            }
            return toReturn;
        }

        private List<Order> findOrdersWithFrequency(OrderFrequency toLookFor)
        {
            List<Order> toReturn = new List<Order>();

            foreach (Order order in allOrders.Values)
                if (order.Frequency == toLookFor)
                    toReturn.Add(order);

            return toReturn;
        }

        private List<Order> findOrdersWithFrequencyRandomly(OrderFrequency toLookFor, int maxAdditions)
        {
            int numAdditions = 0;
            List<Order> toReturn = new List<Order>();
            Random random = new Random();

            for (int i = 0; i < allOrders.Count; i++)
            {
                Order order = allOrders.ElementAt(random.Next(allOrders.Count)).Value;

                if (order.Frequency == toLookFor)
                {
                    if (numAdditions > maxAdditions)
                        return toReturn;
                    else if (!toReturn.Contains(order))
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