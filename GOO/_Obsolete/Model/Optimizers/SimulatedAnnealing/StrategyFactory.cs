using System;

using GOO.Obsolete.Model.Optimizers.SimulatedAnnealing.Strategies;

namespace GOO.Obsolete.Model.Optimizers.SimulatedAnnealing
{
    public class StrategyFactory
    {
        public static Strategy[] GetAllStrategies()
        {
            return new Strategy[] { new RandomSwitchOrdersInRouteStrategy() };
        }
    }
}