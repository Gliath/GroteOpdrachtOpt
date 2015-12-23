using System;

using GOO.Model.Optimizers.Strategies;

namespace GOO.Model.Optimizers
{
    public class StrategyFactory
    {
        public static Strategy[] GetAllPhase2Strategies()
        {
            return new Strategy[] { // Create routes and use either Opt2, Opt2.5, Opt3, Genetic, Random to optimize Strategies
                //new GeneticRouteStrategy(), 
                new RandomRouteOpt2Strategy(), 
                new RandomRouteOpt2HalfStrategy(), 
                new RandomRouteOpt3Strategy(), 
                new RandomRouteOpt3HalfStrategy() };
        }
    }
}