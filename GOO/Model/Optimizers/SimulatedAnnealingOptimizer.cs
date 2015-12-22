using System;
using System.Collections.Generic;

using GOO.Model;
using GOO.Model.Optimizers.Strategies;

namespace GOO.Model.Optimizers
{
    public class SimulatedAnnealingOptimizer
    {
        private static double theX;

        private AnnealingSchedule annealingSchedule;
        private Random random;
        private double oldSolutionScore;
        private double newSolutionScore;

        public SimulatedAnnealingOptimizer()
        {
            theX = 2d;

            annealingSchedule = new AnnealingSchedule();
            random = new Random();
            oldSolutionScore = Double.MaxValue;
            newSolutionScore = Double.MaxValue;
        }

        public Solution runOptimizer(Solution startSolution)
        {
            Solution currentSolution = startSolution;

            for (annealingSchedule.AnnealingIterations = 0; annealingSchedule.AnnealingTemperature > 0.0d; annealingSchedule.AnnealingIterations++)
            {
                // Phase 1 : Marry / Divorce Clusters


                // Phase 2 : Create routes and use either Opt2, Opt2.5, Opt3, Genetic, Random to optimize
                currentSolution = Phase2RouteGeneration(currentSolution);
                currentSolution = Phase2Optimizers(currentSolution);

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

        private List<Cluster> Phase1(Solution toStartFrom) // Deal with clusters & marriages
        {
            return new List<Cluster>();
        }

        private Solution Phase2RouteGeneration(Solution toStartFrom) // Deal with clusters & marriages
        {
            return toStartFrom;
        }

        private Solution Phase2Optimizers(Solution toStartFrom) // Deal with Routes
        {
            //weighted value of each strategy
            double opt2Chance       = 10.0d;
            double opt2HalfChance   = 10.0d;
            double opt3Chance       = 4.0d;
            double opt3HalfChance   = 8.0d;
            double geneticChance    = 3.0d;
            double randomChance     = 5.0d;

            Strategy strategy = null;

            double select = random.NextDouble() * (opt2Chance + opt2HalfChance + opt3Chance + opt3HalfChance + geneticChance + randomChance);

            //opt2
            if (select < opt2Chance)
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
                strategy = StrategyFactory.GetAllPhase2Strategies()[random.Next(StrategyFactory.GetAllPhase2Strategies().Length)];
            else //do nothing wich should never happen actually
                return toStartFrom;

            return strategy.executeStrategy(toStartFrom);
        }

        private Solution Phase3(Solution toStartFrom) // Destribute routes to truckers
        {
            return toStartFrom;
        }

        private bool Phase4(Solution toAcceptOrReject) // Accept Solution or not
        {
            double deltaScore = Math.Abs(oldSolutionScore - newSolutionScore);

            if (deltaScore > 0)
                return true;
            else
            {
                double chanceToBeAccepted = Math.Pow(theX, (deltaScore / annealingSchedule.AnnealingTemperature));

                if (random.NextDouble() <= chanceToBeAccepted)
                    return true;
            }

            return false;
        }
    }
}