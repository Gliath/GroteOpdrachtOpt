using System;
using System.Collections.Generic;
using System.Text;

using GOO.Model;

namespace GOO.KMeansModel
{
    public class KRoute
    {
        List<Cluster> partOfClusters;
        List<Order> orders;

        public double TravelTime { get; private set; }
        public int Weight { get; private set; }

        public KRoute()
        {

        }
    }
}
