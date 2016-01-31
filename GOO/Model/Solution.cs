using System;
using System.Collections.Generic;
using System.Collections;

using GOO.Utilities;
using System.Text;

namespace GOO.Model
{
    public class Solution
    {
        private OrdersTracker ordersTracker;
        private List<ParentCluster> clusters;

        public Solution(List<ParentCluster> clusters)
        {
            this.ordersTracker = OrdersTracker.Instance;
            this.clusters = clusters;
        }

        public ParentCluster getRandomParentCluster()
        {
            int random = new Random().Next(clusters.Count);
            return this.clusters[random];
        }

        public Cluster getRandomCluster()
        {
            Random rng = new Random();
            ParentCluster cluster = this.clusters[rng.Next(clusters.Count)];
            return cluster.Quadrants[rng.Next(cluster.Quadrants.Length)];
        }

        public List<ParentCluster> getAllClusters()
        {
            return this.clusters;
        }

        public double GetSolutionScore() // TODO: Maybe start working with delta's instead of recalculating everytime
        {
            return ordersTracker.GetSolutionScore();
        }

        public override string ToString()
        {
            return ordersTracker.ToString();
        }

        public void AddNewItemToPlanning(Days day, int truckID, List<Route> routes)
        {
            this.ordersTracker.AddNewItemToPlanning(day, truckID, routes);
        }

        public void RemoveItemFromPlanning(Days day, int truckID)
        {
            this.ordersTracker.RemoveItemFromPlanning(day, truckID);
        }

        public List<Tuple<Days, int, List<Route>>> getEntirePlanning()
        {
            return this.ordersTracker.getEntirePlanning();
        }

        public void clearTruckPlanning()
        {
            ordersTracker.clearTruckPlanning();
        }

        public Tuple<Days, int, List<Route>> getPlanningForATruck(Days day, int truckID)
        {
            return this.ordersTracker.getPlanningForATruck(day, truckID);
        }

        public Tuple<Days, int, List<Route>> getRandomPlanning()
        {
            return this.ordersTracker.getRandomPlanning();
        }

        public Tuple<Days, int, List<Route>> getPlanningForRoute(List<Route> route)
        {
            return this.ordersTracker.getPlanningForRoute(route);
        }
    }
}