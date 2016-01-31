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
        private static SimulatedAnnealingOptimizer optimizer;

        public static Solution generateClusters()
        {
            Clusterer clusterer = new Clusterer(Data.Orders, k);
            List<ParentCluster> splitClusters = clusterer.splitClusters(clusterer.createClusters());
            RoutePlanner.AssignDaysToClusters(splitClusters);

            Console.WriteLine("Done generating clusters!");
            return new Solution(splitClusters);
        }

        public static Solution generateSolution()
        {
            Console.WriteLine("Start generating clusters!");
            Clusterer clusterer = new Clusterer(Data.Orders, k);
            List<Cluster> clusters = clusterer.createClusters();
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
            optimizer = new SimulatedAnnealingOptimizer();
            return optimizer.getAnnealingSchedule().getMaximumNumberOfIterations();
        }

        public static Solution optimizeSolution(Solution solution, MainViewModel reportProgressTo = null)
        {
            optimizer = new SimulatedAnnealingOptimizer();
            return optimizer.runOptimizer(solution, reportProgressTo);
        }
    }
}