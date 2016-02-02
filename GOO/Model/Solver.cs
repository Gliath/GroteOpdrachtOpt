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
            var orders = Data.Orders;
            orders.Remove(0);

            Clusterer clusterer = new Clusterer(orders, k);
            var clusters = clusterer.createClusters();
            Solution solution = new Solution(clusterer.createClusters());
            solution.MakeBasicPlannings();

            return solution;
        }

        public static Solution optimizeSolution(Solution solution, MainViewModel reportProgressTo = null)
        {
            optimizer = new SimulatedAnnealingOptimizer();
            return optimizer.runOptimizer(solution, reportProgressTo);
        }

        public static double getMaximumNumberOfSAIterations()
        {
            optimizer = new SimulatedAnnealingOptimizer();
            return optimizer.getAnnealingSchedule().getMaximumNumberOfIterations();
        }
    }
}