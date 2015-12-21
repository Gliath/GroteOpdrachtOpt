using System;
using System.Collections.Generic;

using GOO.Model;
using GOO.Model.Optimizers;
using GOO.Model.Optimizers.Strategies;

namespace GOO.KMeansModel
{
    public class KSimulatedAnnealingOptimizer
    {
        private static double theX = 2d;

        private Strategy[] phase_1_strategies;
        private Strategy[] phase_2_strategies;
        private Strategy[] phase_3_strategies;

        private AnnealingSchedule annealingSchedule;

        private Random random;

        private double oldSolutionScore;

        public KSimulatedAnnealingOptimizer()
        {
            phase_1_strategies = StrategyFactory.GetAllStrategies();
            phase_2_strategies = StrategyFactory.GetAllStrategies();
            phase_3_strategies = StrategyFactory.GetAllStrategies();

            annealingSchedule = new AnnealingSchedule();

            random = new Random();

            oldSolutionScore = Double.MaxValue;
        }

        public Solution runOptimizer(Solution startSolution)
        {
            Solution currentSolution = startSolution;

            for (annealingSchedule.AnnealingIterations = 0; annealingSchedule.AnnealingTemperature > 0.0d; annealingSchedule.AnnealingIterations++)
            {
                // Phase 1 : Marry / Divorce & Schedule Clusters

                // Phase 2 : Create routes and use either Opt2, Opt2.5, Opt3, Genetic, Random to optimize

                // Phase 3 : Assign routes to truckers

                // Phase 4 : Accept or reject new solution

                /* OLD stuff
                double currentSolutionScore = currentSolution.GetSolutionScore();

                double deltaScore = currentSolutionScore - newSolutionScore;
                double chanceToBeAccepted = Math.Pow(theX, (deltaScore / annealingSchedule.AnnealingTemperature));

                if (deltaScore > 0)
                {
                    currentSolution = newSolution;
                }

                else if (random.NextDouble() <= chanceToBeAccepted)
                {
                    currentSolution = newSolution;
                } // No new solution accepted.
                */
            }

            return currentSolution;
        }

        private List<Cluster> Phase_1_dealWithClusters(Solution toStartFrom)
        {
            return new List<Cluster>();
        }

        private Solution Phase_2_dealWithRoutes(Solution toStartFrom)
        {
            return toStartFrom;
        }

        private Solution Phase_3_distributeRoutesToTruckers(Solution toStartFrom)
        {
            return toStartFrom;
        }

        private Solution Phase_4_AcceptOrReject(Solution toAcceptOrReject)
        {
            return toAcceptOrReject;
        }

    }
}