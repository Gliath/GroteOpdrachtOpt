using System;
using System.Collections.Generic;

using GOO.Model.Optimizers;
using GOO.Utilities;
using GOO.ViewModel;

namespace GOO.Model
{
    public class Solver
    {
        private static readonly int k = 30;

        private static Clusterer clusterer;
        private static List<Cluster> clusters;

        public static Solution generateSolution()
        {
            Console.WriteLine("Start generating clusters!");
            clusterer = new Clusterer(Data.Orders, k);
            clusters = clusterer.createClusters();
            Console.WriteLine("Done generating clusters!");
            Console.WriteLine("Starting to split clusters!");

            List<ParentCluster> splitClusters = clusterer.splitClusters(clusters);
            Console.WriteLine("Done splitting clusters!");

            List<AbstractCluster> abstractClusters = new List<AbstractCluster>();
            List<ParentCluster> parentClusters = RoutePlanner.PlanStartClusters(splitClusters);
            foreach (ParentCluster parentCluster in parentClusters)
	            abstractClusters.Add(parentCluster);

            return RoutePlanner.PlanRoutesFromClustersIntoSolution(new Solution(splitClusters), abstractClusters);
        }

        private static SimulatedAnnealingOptimizer sAO;
        public static double getMaximumNumberOfSAIterations()
        {
            sAO = new SimulatedAnnealingOptimizer();
            return sAO.getAnnealingSchedule().getMaximumNumberOfIterations();
        }

        public static Solution optimizeSolution(Solution solution, MainViewModel reportProgressTo = null)
        {
            return sAO.runOptimizer(solution, reportProgressTo);
        }
    }
}