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

        private static SimulatedAnnealingOptimizer optimizer = new SimulatedAnnealingOptimizer();

        public static Solution generateSolution()
        {
            Console.WriteLine("Start generating clusters!");
            clusterer = new Clusterer(Data.Orders, k);
            clusters = clusterer.createClusters();
            Console.WriteLine("Done generating clusters!");
            Console.WriteLine("Starting to split clusters!");

            List<ParentCluster> splitClusters = clusterer.splitClusters(clusters);
            Console.WriteLine("Done splitting clusters!");

            List<ParentCluster> parentClusters = RoutePlanner.PlanStartClusters(splitClusters);

            List<AbstractCluster> abstractClusters = new List<AbstractCluster>();
            foreach (ParentCluster parentCluster in parentClusters)
	            abstractClusters.Add(parentCluster);

            return RoutePlanner.PlanRoutesFromClustersIntoSolution(new Solution(splitClusters), abstractClusters);
        }

        public static double getMaximumNumberOfSAIterations()
        {
            return optimizer.getAnnealingSchedule().getMaximumNumberOfIterations();
        }

        public static Solution optimizeSolution(Solution solution, MainViewModel reportProgressTo = null)
        {
            return optimizer.runOptimizer(solution, reportProgressTo);
        }
    }
}