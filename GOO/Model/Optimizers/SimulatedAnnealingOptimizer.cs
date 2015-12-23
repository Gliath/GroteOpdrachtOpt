﻿using System;
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
                // Phase 1 : Marry / Divorce Clusters & Schedule Clusters 
                List<AbstractCluster> newClusters = Phase1(currentSolution);

                // Phase 2 : Create routes and use either Opt2, Opt2.5, Opt3, Genetic, Random to optimize
                Phase2RouteGeneration(newClusters);
                currentSolution = Phase2Optimizers(currentSolution);

                // Phase 3 : Assign routes to truckers
                currentSolution = Phase3(currentSolution, newClusters); // TODO : MAKE SURE THE NEW ROUTES ARE ADDED IN THE RIGHT CLUSTER

                // Phase 4 : Accept or reject new solution
                if (Phase4(currentSolution)) // New Solution accepted
                    startSolution = currentSolution;
                else // New solution rejected
                    currentSolution = startSolution;
            }
            /*
             * Phase1
             * Phase2RouteGeneration
             */

            return currentSolution;
        }

        private List<AbstractCluster> Phase1(Solution toStartFrom) // Deal with clusters & marriages
        {
            MarriageCounselorStrategy MarriageCounselor = new MarriageCounselorStrategy();

            return MarriageCounselor.executeStrategy(toStartFrom);
        }

        private void Phase2RouteGeneration(List<AbstractCluster> clustersToPlan) // Deal with clusters & marriages
        {
            RoutePlanner.GenerateRoutesFromClusters(clustersToPlan);
        }

        private Solution Phase2Optimizers(Solution toStartFrom) // Deal with Routes
        {
            //weighted value of each strategy
            double opt2Chance = 10.0d;
            double opt2HalfChance = 10.0d;
            double opt3Chance = 4.0d;
            double opt3HalfChance = 8.0d;
            double geneticChance = 3.0d;
            double randomChance = 5.0d;

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
                strategy = StrategyFactory.GetAllPhase2Strategies()[random.Next(StrategyFactory.GetAllPhase2Strategies().Length)];
            //total random
            else if (select < opt2Chance + opt2HalfChance + opt3Chance + opt3HalfChance + geneticChance + randomChance)
                strategy = StrategyFactory.GetAllPhase2Strategies()[random.Next(StrategyFactory.GetAllPhase2Strategies().Length)];
            else //do nothing wich should never happen actually
                return toStartFrom;

            return strategy.executeStrategy(toStartFrom);
        }

        private Solution Phase3(Solution toStartFrom, List<AbstractCluster> clustersToPlan) // Distribute routes to truckers
        {
            return RoutePlanner.PlanRoutesFromClustersIntoSolution(toStartFrom, clustersToPlan);
        }

        private bool Phase4(Solution toAcceptOrReject) // Accept Solution or not
        {
            double deltaScore = Math.Abs(oldSolutionScore - newSolutionScore);
            double chanceToBeAccepted = Math.Pow(theX, (deltaScore / annealingSchedule.AnnealingTemperature));

            return deltaScore > 0 || random.NextDouble() <= chanceToBeAccepted;
        }
    }
}