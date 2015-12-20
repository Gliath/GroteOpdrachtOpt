﻿using System;
using System.Collections.Generic;

using GOO.Model;
using GOO.Utilities;

namespace GOO.KMeansModel
{
    public class KSolution
    {
        private OrdersCounter ordersCounter;

        private List<Cluster> clusters;
        private List<Tuple<Days, int, List<Route>>> truckPlanning;

        public KSolution(List<Cluster> clusters)
        {
            this.ordersCounter = new OrdersCounter();
            this.clusters = clusters;
            this.truckPlanning = new List<Tuple<Days, int, List<Route>>>();
        }

        public void AddNewItemToPlanning(Days day, int truckID, List<Route> routes)
        {
            this.truckPlanning.Add(new Tuple<Days, int, List<Route>>(day, truckID, routes));
        }

        public void RemoveItemFromPlanning(Days day, int truckID)
        {
            this.truckPlanning.RemoveAll(t => t.Item1 == day && t.Item2 == truckID);
        }

        public double GetSolutionScore()
        {
            return 0.0d;
        }
    }
}
