using System;
using System.Collections.Generic;

using GOO.Utilities;

namespace GOO.Model
{
    public class Solver
    {
        private static readonly int k = 30;

        private static Clusterer clusterer;
        private static List<Cluster> clusters;

        public static Solution generateSolution()
        {
            //System.Diagnostics.Stopwatch sw = System.Diagnostics.Stopwatch.StartNew();
            //for (int i = 10; i <= 100; i += 5)
            //{
            //    sw.Restart();
            //    clusterer = new Clusterer(Data.Orders, i);
            //    clusters = clusterer.createKClusters();

            //    int numOfEmptyClusters = 0;
            //    for (int k = 0; k < clusters.Count; k++)
            //    {
            //        if (clusters[k].OrdersInCluster.Count == 0)
            //            numOfEmptyClusters++;
            //    }
            //    sw.Stop();

            //    Console.WriteLine("Generated a k-cluster solution with: {0} clusters with {1} not empty ones.", i, clusters.Count);
            //    //Console.WriteLine("This solution had {0} empty clusters.", numOfEmptyClusters);
            //    Console.WriteLine("It took {0}ms to generate this solution.", sw.ElapsedMilliseconds);

            //}
            
            clusterer = new Clusterer(Data.Orders, k);
            clusters = clusterer.createKClusters();

            foreach (Cluster cluster in clusters)
                Console.WriteLine(cluster);

            List<ParentCluster> splitClusters = clusterer.splitClusters(clusters);

            return RoutePlanner.PlanRoutesFromClustersIntoSolution(new Solution(splitClusters), RoutePlanner.PlanStartClusters(splitClusters));
        }

        public static Solution optimizeSolution()
        {
            // To do fix this method, use generated solution to optimize it

            return generateSolution(); 
        }
    }
}