using System;

using GOO.Model.Optimizers.Strategies;

namespace GOO.Model.Optimizers
{
    public class StrategyFactory
    {
        public static Strategy[] GetAllPhase1Strategies()
        {
            return new Strategy[] { 
                new GeneticRouteStrategy(), 
                new RandomRouteOpt2Strategy(), 
                new RandomRouteOpt2HalfStrategy(), 
                new RandomRouteOpt3Strategy(), 
                new RandomRouteOpt3HalfStrategy() };
        }

        public static Strategy[] GetAllPhase2Strategies()
        {
            return new Strategy[] { 
                new GeneticRouteStrategy(), 
                new RandomRouteOpt2Strategy(), 
                new RandomRouteOpt2HalfStrategy(), 
                new RandomRouteOpt3Strategy(), 
                new RandomRouteOpt3HalfStrategy() };
        }

        public static Strategy[] GetAllPhase3Strategies()
        {
            return new Strategy[] { 
                new GeneticRouteStrategy(), 
                new RandomRouteOpt2Strategy(), 
                new RandomRouteOpt2HalfStrategy(), 
                new RandomRouteOpt3Strategy(), 
                new RandomRouteOpt3HalfStrategy() };
        }
    }
}