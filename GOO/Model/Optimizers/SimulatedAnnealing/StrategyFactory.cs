using System;

using GOO.Model.Optimizers.SimulatedAnnealing.Strategies;

namespace GOO.Model.Optimizers.SimulatedAnnealing
{
    public class StrategyFactory
    {
        public static Strategy[] GetAllStrategies()
        {
            return new Strategy[] { new RandomSwitchOrdersInRouteStrategy() };
        }
    }
}