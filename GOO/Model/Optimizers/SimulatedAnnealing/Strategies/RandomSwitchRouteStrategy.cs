using System;
using System.Collections.Generic;

namespace GOO.Model.Optimizers.SimulatedAnnealing.Strategies
{
    public class RandomSwitchRouteStrategy : Strategy
    {
        public RandomSwitchRouteStrategy()
            : base()
        {

        }

        public override Solution executeStrategy(Solution toStartFrom)
        {
            return toStartFrom;
        }
    }
}