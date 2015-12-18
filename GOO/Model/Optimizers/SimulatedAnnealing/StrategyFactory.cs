using System;

namespace GOO.Model.Optimizers.SimulatedAnnealing
{
    public class StrategyFactory
    {
        public static Strategy[] GetAllStrategies()
        {
            return new Strategy[] {new RandomSwitchOrdersInRouteStrategy()};
        }
    }
}