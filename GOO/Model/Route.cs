using System;
using System.Collections.Generic;

namespace GOO.Model
{
    public class Route
    {
        List<Cluster> partOfClusters;
        List<Order> orders;

        // add variable to indicate for which day this route is meant for

        public List<Order> Orders { get; private set; } // List of orders for this route
        public double TravelTime { get; private set; } // the total travel time
        public int Weight { get; private set; } //the filled weight of the route
        static Random random = new Random();

        public Route()
        {

        }
    }
}