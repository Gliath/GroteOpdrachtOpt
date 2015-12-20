using System;
using System.Collections.Generic;

using GOO.Utilities;
using GOO.Model;

namespace GOO.KMeansModel
{
    public class KSolver
    {
        public static KSolution generateSolution()
        {
            Order[] allOrders = FilesInitializer._Orders;

            KMeansClusterer clusterer = new KMeansClusterer(allOrders);
            List<Cluster> clusters = clusterer.createKClusters();

            foreach (Cluster cluster in clusters)
            {
                Console.WriteLine(cluster);    
            }            

            return new KSolution();
        }

    }
}
