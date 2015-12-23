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
                toReturn.Add(new Cluster(new Point(order.X, order.Y)));

            for (int i = 0; i < 3000; i++) // Try to reposition the center-point 3000 times for each cluster
            {
                assignOrdersToClustersEuclidean(toReturn, allOrders);
                toReturn.RemoveAll(c => c.OrdersInCluster.Count == 0);
            }

            foreach (Cluster cluster in toReturn)
            {
                List<Days> restrictions = new List<Days>();
                List<OrderFrequency> frequenciesPresent = new List<OrderFrequency>();
            }

            return toReturn;
        }

        public List<ParentCluster> splitClusters(List<Cluster> toSplit)
        {
            List<ParentCluster> toReturn = new List<ParentCluster>();

            bool fre2 = false;
            bool fre3 = false;
            bool fre4 = false;

            foreach (Cluster parentCluster in toSplit)
            {
                fre2 = false;
                fre3 = false;
                fre4 = false;

                Point centroid = parentCluster.CenterPoint;
                List<Cluster> quadrants = new List<Cluster>();

                foreach (Order order in parentCluster.OrdersInCluster)
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
                    quadrants.Add(new Cluster(new Point()));
                    quadrants.Add(new Cluster(new Point()));
                    assignOrdersToClustersCentroid(quadrants, parentCluster.OrdersInCluster, centroid, fre2, fre3, fre4);
                }
                else if (fre3 && !(fre2 || fre4))
                {
                    quadrants.Add(new Cluster(new Point()));
                    quadrants.Add(new Cluster(new Point()));
                    quadrants.Add(new Cluster(new Point()));
                    assignOrdersToClustersCentroid(quadrants, parentCluster.OrdersInCluster, centroid, fre2, fre3, fre4);
                }

                else if (fre2 || fre3 || fre4)
                {
                    quadrants.Add(new Cluster(new Point()));
                    quadrants.Add(new Cluster(new Point()));
                    quadrants.Add(new Cluster(new Point()));
                    quadrants.Add(new Cluster(new Point()));
                    assignOrdersToClustersCentroid(quadrants, parentCluster.OrdersInCluster, centroid, fre2, fre3, fre4);
                }
                else
                {
                    quadrants.Add(new Cluster(new Point()));
                    assignOrdersToClustersCentroid(quadrants, parentCluster.OrdersInCluster, centroid, fre2, fre3, fre4);
                }
                toReturn.Add(new ParentCluster(centroid, parentCluster.OrdersInCluster, quadrants.ToArray())); // TODO : Double-check day restrictions
            }
            return toReturn;
        }

        private void assignOrdersToClustersCentroid(List<Cluster> quadrants, List<Order> toAssign, Point centroid, bool fre2, bool fre3, bool fre4)
        {
            foreach (Order order in toAssign)
            {
                if (quadrants.Count == 1)
                   quadrants[0].AddOrderToCluster(order);
                else if (quadrants.Count == 2)
                {
                    if (order.X >= centroid.X) // right
                        quadrants[0].AddOrderToCluster(order);
                    if (order.X < centroid.X) // left
                        quadrants[1].AddOrderToCluster(order);
                }
                else if (quadrants.Count == 3)
                {
                    if (order.X >= centroid.X && order.Y >= centroid.Y) // right - up
                        quadrants[0].AddOrderToCluster(order);

                    if (order.X >= centroid.X && order.Y < centroid.Y) // right - down
                        quadrants[1].AddOrderToCluster(order);

                    if (order.X < centroid.X) // left
                        quadrants[2].AddOrderToCluster(order);
                }
                else if (quadrants.Count == 4)
                {
                    if (order.X >= centroid.X && order.Y >= centroid.Y) // right - up
                        quadrants[0].AddOrderToCluster(order);

                    if (order.X >= centroid.X && order.Y < centroid.Y) // right - down
                        quadrants[1].AddOrderToCluster(order);

                    if (order.X < centroid.X && order.Y >= centroid.Y) // left - up
                        quadrants[2].AddOrderToCluster(order);

                    if (order.X < centroid.X && order.Y < centroid.Y) // left - down
                        quadrants[3].AddOrderToCluster(order);
                }
            }

            List<Order> AllFre2Orders = new List<Order>();
            List<Order> AllFre3Orders = new List<Order>();
            List<Order> AllFre4Orders = new List<Order>();
            List<Order> fre2Orders1 = new List<Order>();
            List<Order> fre2Orders2 = new List<Order>();

            if (fre2)
                AllFre2Orders = findOrdersWithFrequency(toAssign, OrderFrequency.PWK2);
            if (fre3)
                AllFre3Orders = findOrdersWithFrequency(toAssign, OrderFrequency.PWK3);
            if (fre4)
                AllFre4Orders = findOrdersWithFrequency(toAssign, OrderFrequency.PWK4);

            foreach (Order order in AllFre2Orders)
            {
                if (order.X >= centroid.X) // right
                    fre2Orders1.Add(order);
                if (order.X < centroid.X) // left
                    fre2Orders2.Add(order);
            }
            if(!(fre2 || fre3 || fre4))
            {
                Days[] DaysRestrictions = new Days[] {
                    Days.Monday | Days.Tuesday | Days.Wednesday | Days.Thursday | Days.Friday };

                for (int quadrantIndex = 0; quadrantIndex < quadrants.Count; quadrantIndex++)
                    quadrants[quadrantIndex].initialRestrictions = DaysRestrictions[quadrantIndex];
            }
            if (fre2 && !(fre3 || fre4))
            {
                multiOrderAssignFre2Excusively(quadrants, AllFre2Orders);

                Days[] DaysRestrictions = new Days[] { 
                    Days.Monday | Days.Tuesday, 
                    Days.Thursday | Days.Friday };

                for (int quadrantIndex = 0; quadrantIndex < quadrants.Count; quadrantIndex++)
                    quadrants[quadrantIndex].initialRestrictions = DaysRestrictions[quadrantIndex];
            }
            else if (fre3 && !(fre2 || fre4))
            { // randomly assign the fre 3 orders to three clusters
                multiOrderAssignFre4(quadrants, AllFre3Orders);

                Days[] DaysRestrictions = new Days[] { 
                    Days.Monday | Days.Wednesday | Days.Friday, 
                    Days.Monday | Days.Wednesday | Days.Friday, 
                    Days.Monday | Days.Wednesday | Days.Friday };

                for (int quadrantIndex = 0; quadrantIndex < quadrants.Count; quadrantIndex++)
                    quadrants[quadrantIndex].initialRestrictions = DaysRestrictions[quadrantIndex];
            }
            else if (fre4 && !(fre2 || fre3))
            { // assign one fre4 to every cluster
                multiOrderAssignFre4(quadrants, AllFre4Orders);

                Days[] DaysRestrictions = new Days[] {
                    Days.Monday | Days.Tuesday | Days.Wednesday | Days.Thursday,
                    Days.Monday | Days.Tuesday | Days.Wednesday | Days.Friday,
                    Days.Monday | Days.Tuesday | Days.Thursday | Days.Friday,
                    Days.Tuesday | Days.Wednesday | Days.Thursday | Days.Friday };

                for (int quadrantIndex = 0; quadrantIndex < quadrants.Count; quadrantIndex++)
                    quadrants[quadrantIndex].initialRestrictions = DaysRestrictions[quadrantIndex];
            }
            else if (fre2 && fre3 && !fre4)
            {
                // assign fre 3 randomly to three clusters
                // create a group of fre 2 orders and assign them exclusively
                // fre2 may only occur once together with a fre 3 order
                multiOrderAssignFre3(quadrants, AllFre3Orders);
                multiOrderAssignFre2BasedOnFre3(quadrants, AllFre2Orders);
                
                Days[] DaysRestrictions = new Days[] {
                    Days.Monday | Days.Friday,
                    Days.Monday | Days.Wednesday | Days.Friday,
                    Days.Monday | Days.Wednesday | Days.Friday,
                    Days.Tuesday | Days.Thursday };

                for (int quadrantIndex = 0; quadrantIndex < quadrants.Count; quadrantIndex++)
                    quadrants[quadrantIndex].initialRestrictions = DaysRestrictions[quadrantIndex];
            }

            else if (fre2 && !fre3 && fre4)
            {
                // create two groups of fre2 orders, and assign them exclusively
                // assign one fre4 to every cluster
                multiOrderAssignFre2Excusively(quadrants, fre2Orders1, fre2Orders2);
                multiOrderAssignFre4(quadrants, AllFre4Orders);
                
                Days[] DaysRestrictions = new Days[] {
                    Days.Monday | Days.Thursday,
                    Days.Monday | Days.Thursday,
                    Days.Tuesday | Days.Friday,
                    Days.Tuesday | Days.Friday };

                for (int quadrantIndex = 0; quadrantIndex < quadrants.Count; quadrantIndex++)
                    quadrants[quadrantIndex].initialRestrictions = DaysRestrictions[quadrantIndex];
            }

            else if (!fre2 && fre3 && fre4)
            {
                // randomly assign the fre 3 orders to three clusters
                // assign one fre4 to every cluster

                multiOrderAssignFre3(quadrants, AllFre3Orders);
                multiOrderAssignFre4(quadrants, AllFre4Orders);
                
                Days[] DaysRestrictions = new Days[] {
                    Days.Monday | Days.Wednesday | Days.Friday,
                    Days.Monday | Days.Wednesday | Days.Friday,
                    Days.Monday | Days.Wednesday | Days.Friday,
                    Days.Tuesday | Days.Thursday };

                for (int quadrantIndex = 0; quadrantIndex < quadrants.Count; quadrantIndex++)
                    quadrants[quadrantIndex].initialRestrictions = DaysRestrictions[quadrantIndex];
            }

            else if (fre2 && fre3 && fre4)
            {
                // assign one fre4 to every cluster
                // randomly assign the fre 3 orders to three clusters
                // create a group of fre 2 orders and assign them exclusively
                // fre2 may only occur once together with a fre 3 order

                multiOrderAssignFre3(quadrants, AllFre3Orders);
                multiOrderAssignFre4(quadrants, AllFre4Orders);
                multiOrderAssignFre2BasedOnFre3(quadrants, AllFre2Orders);
                
                Days[] DaysRestrictions = new Days[] {
                    Days.Monday | Days.Friday,
                    Days.Monday | Days.Wednesday | Days.Friday,
                    Days.Monday | Days.Wednesday | Days.Friday,
                    Days.Tuesday | Days.Thursday };

                for (int quadrantIndex = 0; quadrantIndex < quadrants.Count; quadrantIndex++)
                    quadrants[quadrantIndex].initialRestrictions = DaysRestrictions[quadrantIndex];
            }
        }

        private void multiOrderAssignFre2BasedOnFre3(List<Cluster> quadrants, List<Order> allFre2Orders)
        {
            bool addedToFre3 = false;
            foreach (Cluster cluster in quadrants)
            {
                if (!addedToFre3 && cluster.OrdersInCluster.Find(o => o.Frequency == OrderFrequency.PWK3) != null)
                {
                    cluster.OrdersInCluster.AddRange(allFre2Orders);
                    addedToFre3 = true;
                }
                else if (cluster.OrdersInCluster.Find(o => o.Frequency == OrderFrequency.PWK3) == null)
                    cluster.OrdersInCluster.AddRange(allFre2Orders);
            }
        }

        private void multiOrderAssignFre2Excusively(List<Cluster> quadrants, List<Order> fre2Orders)
        {
            // Assign one cluster for fre2orders1
            // randomly choose another 

            // assign the rest to the remaining clusters

            foreach (Cluster quadrant in quadrants)
                quadrant.OrdersInCluster.AddRange(fre2Orders);
        }

        private void multiOrderAssignFre2Excusively(List<Cluster> quadrants, List<Order> fre2Orders1, List<Order> fre2Orders2)
        {
            // Assign one cluster for fre2orders1
            // randomly choose another 

            // assign the rest to the remaining clusters
            List<Cluster> copy = new List<Cluster>();
            copy.AddRange(quadrants);

            Cluster firstCluster = copy[random.Next(copy.Count)];
            copy.Remove(firstCluster);
            Cluster secondCluster = copy[random.Next(copy.Count)];
            copy.Remove(secondCluster);

            firstCluster.OrdersInCluster.AddRange(fre2Orders1);
            secondCluster.OrdersInCluster.AddRange(fre2Orders2);

        }

        private void multiOrderAssignFre4(List<Cluster> quadrants, List<Order> freOrders)
        {
            foreach (Cluster cluster in quadrants)
                foreach (Order order in freOrders)
                    if (!cluster.OrdersInCluster.Contains(order))
                        cluster.OrdersInCluster.Add(order);
        }

        private void multiOrderAssignFre3(List<Cluster> quadrants, List<Order> freOrders)
        {
            Cluster noAssign = quadrants[random.Next(quadrants.Count)];
            foreach (Cluster cluster in quadrants)
                if (cluster != noAssign)
                    foreach (Order order in freOrders)
                        if (!cluster.OrdersInCluster.Contains(order))
                            cluster.OrdersInCluster.Add(order);
        }

        private void assignOrdersToClustersEuclidean(List<Cluster> clusters, Dictionary<int, Order> toAssign)
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

        private List<Order> findOrdersWithFrequency(List<Order> toLookIn, OrderFrequency toLookFor)
        {
            List<Order> toReturn = new List<Order>();

            foreach (Order order in toLookIn)
                if (order.Frequency == toLookFor)
                    toReturn.Add(order);

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