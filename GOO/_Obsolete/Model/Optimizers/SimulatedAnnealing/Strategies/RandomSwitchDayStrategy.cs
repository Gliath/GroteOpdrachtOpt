using System;
using System.Collections.Generic;

namespace GOO.Obsolete.Model.Optimizers.SimulatedAnnealing.Strategies
{
    public class RandomSwitchDayStrategy : Strategy
    {
        public RandomSwitchDayStrategy()
            : base()
        {

        }

        public override Solution executeStrategy(Solution toStartFrom)
        {
            return toStartFrom;
        }
    }
}