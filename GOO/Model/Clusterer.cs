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
            return createClusters(Clusterer.startingAmountOfClusters, this.createStartOrders(this.allOrders, Clusterer.startingAmountOfClusters), this.allOrders);
        }

        public List<Cluster> createClusters(int amountOfClusters, List<Order> startingPoints, Dictionary<int, Order> allOrders)
        {
            List<Cluster> toReturn = new List<Cluster>();

            foreach (Order order in startingPoints)
            {
                toReturn.Add(new Cluster(new Point(order.X, order.Y)));
            }

            for (int i = 0; i < 3000; i++) // Try to reposition the center-point 3000 times for each cluster
            {
                assignOrdersToClustersEuclidean(toReturn, allOrders);
                toReturn.RemoveAll(c => c.OrdersInCluster.Count == 0);
            }

            foreach (Cluster cluster in toReturn)
            {
                List<Days> restrictions = new List<Days>();
                List<OrderFrequency> frequenciesPresent = new List<OrderFrequency>();

                //foreach (Order order in cluster.OrdersInCluster)
                //    if (order.Frequency != OrderFrequency.PWK1 && !frequenciesPresent.Contains(order.Frequency))
                //    {
                //        restrictions.AddRange(DayRestrictionFactory.GetDayRestrictions(order.Frequency));
                //        frequenciesPresent.Add(order.Frequency);
                //    }

                //cluster.DaysRestrictions = restrictions;
            }

            return toReturn;
        }

        public List<ParentCluster> splitClusters(List<Cluster> toSplit)
        {
            List<ParentCluster> toReturn = new List<ParentCluster>();

            bool fre2 = false;
            bool fre3 = false;
            bool fre4 = false;

            foreach (Cluster cluster in toSplit)
            {
                Point centroid = cluster.CenterPoint;
                List<Cluster> toUse = new List<Cluster>();

                foreach (Order order in cluster.OrdersInCluster)
                {
                    if (order.Frequency == OrderFrequency.PWK2)
                        fre2 = true;
                    else if (order.Frequency == OrderFrequency.PWK3)
                        fre3 = true;
                    else if (order.Frequency == OrderFrequency.PWK4)
                        fre4 = true;
                }

                // Per cluster split based on different order frequencies, ignoring frequency 1
                // For all of the following splits, logically link the new clusters.

                if (fre2 && !(fre3 || fre4))
                {
                    // if only fre2
                    // split in two clusters
                    // draw grid on centroid to split area in two
                    toUse.Add(new Cluster(new Point()));
                    toUse.Add(new Cluster(new Point()));
                    assignOrdersToClustersCentroid(toUse, cluster.OrdersInCluster, centroid);
                }

                else if (fre3 && !(fre3 || fre4))
                {
                    // if only fre 3
                    // split in three clusters
                    // draw grid on centroid to split area in four
                    // and randomly join two of the splitted areas together, as long as they are neighbouring

                    toUse.Add(new Cluster(new Point()));
                    toUse.Add(new Cluster(new Point()));
                    toUse.Add(new Cluster(new Point()));
                    toUse.Add(new Cluster(new Point()));
                    assignOrdersToClustersCentroid(toUse, cluster.OrdersInCluster, centroid);

                    // Fuse two of the returned clusters. TODO : Check if Opposite quadrants are a problem if fused
                    int randomq1 = random.Next(toUse.Count);
                    Cluster q1 = toUse[randomq1];
                    
                    int randomq2 = -1;
                    do
                    {
                        randomq2 = random.Next(toUse.Count);
                    } while (randomq2 < 0 && randomq1 == randomq2);
                    
                    Cluster q2 = toUse[randomq2];

                    foreach (Order order in q2.OrdersInCluster)
	                {
                        if (!q1.OrdersInCluster.Contains(order))
                            q1.OrdersInCluster.Add(order);
	                }

                    toUse.Remove(q2);
                }

                else if (fre2 || fre3 || fre4)
                {
                    // if any other combination
                    // split in four clusters
                    // draw grid on centroid to split area in four

                    toUse.Add(new Cluster(new Point()));
                    toUse.Add(new Cluster(new Point()));
                    toUse.Add(new Cluster(new Point()));
                    toUse.Add(new Cluster(new Point()));
                    assignOrdersToClustersCentroid(toUse, cluster.OrdersInCluster, centroid);
                }
                toReturn.Add(new ParentCluster(centroid, cluster.OrdersInCluster, toUse.ToArray())); // TODO : Double-check day restrictions
            }
            return toReturn;
        }

        private void assignOrdersToClustersCentroid(List<Cluster> clusters, List<Order> toAssign, Point centroid)
        {
            foreach (Order order in toAssign)
            {
                if (clusters.Count == 2)
                {
                    if (order.X >= centroid.X) // right
                        clusters[0].AddOrderToCluster(order);
                    if (order.X <= centroid.X) // left
                        clusters[1].AddOrderToCluster(order);
                }
                else if (clusters.Count >= 3)
                {
                    if (order.X >= centroid.X && order.Y >= centroid.Y) // right - up
                        clusters[0].AddOrderToCluster(order);

                    if (order.X >= centroid.X && order.Y <= centroid.Y) // right - down
                        clusters[1].AddOrderToCluster(order);

                    if (order.X <= centroid.X && order.Y >= centroid.Y) // left - up
                        clusters[2].AddOrderToCluster(order);

                    if (order.X <= centroid.X && order.Y <= centroid.Y) // left - down
                        clusters[3].AddOrderToCluster(order);
                }
            }

            foreach (Order order in toAssign)
            {
                if (order.FrequencyNumber > 1)
                {
                    for (int i = 0; i < order.FrequencyNumber-1; i++)
                    {
                        int j = 0;
                        bool inserted = false;
                        do
                        {
                            if (!clusters[j].OrdersInCluster.Contains(order)) { 
                                clusters[j].OrdersInCluster.Add(order);
                                inserted = true;
                            }
                            j++;
                        } while (j < clusters.Count && !inserted);
                    }
                }
            }
        }

        private void assignOrdersToClustersEuclidean(List<Cluster> clusters, Dictionary<int, Order> toAssign)
        {
            // Grouping orders based on Euclidean distance method
            // Note : calculating the root is currently not neccessary due to look at relative points
            foreach (Cluster cluster in clusters)
            {
                cluster.RemoveAllOrdersFromCluster();
            }

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

        private List<Order> createStartOrders(Dictionary<int, Order> allOrders, int amountOfClusters)
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

        private List<Order> findOrdersWithFrequency(Dictionary<int, Order> toLookIn, OrderFrequency toLookFor)
        {
            List<Order> toReturn = new List<Order>();

            foreach (Order order in toLookIn.Values)
                if (order.Frequency == toLookFor)
                    toReturn.Add(order);

            return toReturn;
        }

        private List<Order> findOrdersWithFrequencyRandomly(Dictionary<int, Order> toLookIn, OrderFrequency toLookFor, int maxAdditions)
        {
            int numAdditions = 0;
            List<Order> toReturn = new List<Order>();
            Random random = new Random();

            for (int i = 0; i < toLookIn.Count; i++)
            {
                Order order = toLookIn.ElementAt(random.Next(toLookIn.Count)).Value;

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