using System;
using System.Collections.Generic;

using GOO.Model;
using GOO.Model.Optimizers.Strategies;

namespace GOO.Model.Optimizers
{
    public class SimulatedAnnealingOptimizer
    {
        private static double theX = 2d;

        private Strategy[] phase_1_strategies;
        private Strategy[] phase_2_strategies;
        private Strategy[] phase_3_strategies;

        private AnnealingSchedule annealingSchedule;

        private Random random;

        private double oldSolutionScore;

        public SimulatedAnnealingOptimizer()
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
                // Phase 1 : Marry / Divorce Clusters

                // Phase 2 : Create routes and use either Opt2, Opt2.5, Opt3, Genetic, Random to optimize

                // Phase 3 : Schedule Clusters & Assign routes to truckers

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
            //weighted value of each strategy
            double opt2Chance = 10d;
            double opt2HalfChance = 10d;
            double opt3Chance = 4d;
            double opt3HalfChance = 8d;
            double geneticChance = 3d;
            double randomChance = 5d;

            Random random = new Random();
            Strategy strategy = null;

            double select = random.NextDouble() * (opt2Chance + opt2HalfChance + opt3Chance + opt3HalfChance + geneticChance + randomChance);

            //opt2
            if(select < opt2Chance)
                strategy = new Strategies.RandomRouteOpt2Strategy();
            //opt 2 alt
            else if (select < opt2Chance + opt2HalfChance)
                strategy = new Strategies.RandomRouteOpt2HalfStrategy();
            //opt3
            else if (select < opt2Chance + opt2HalfChance + opt3Chance)
                strategy = new Strategies.RandomRouteOpt3Strategy();
            //opt3 alt
            else if (select < opt2Chance + opt2HalfChance + opt3Chance + opt3HalfChance)
                strategy = new Strategies.RandomRouteOpt3HalfStrategy();
            //genetic
            else if (select < opt2Chance + opt2HalfChance + opt3Chance + opt3HalfChance + geneticChance)
                strategy = new Strategies.GeneticRouteStrategy();
            //total random
            else if (select < opt2Chance + opt2HalfChance + opt3Chance + opt3HalfChance + geneticChance + randomChance)
                strategy = new Strategies.RandomRouteOpt2Strategy();
            else //do nothing wich should never happen actualy
                return toStartFrom;

            strategy.executeStrategy(toStartFrom);
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