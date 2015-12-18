using System;
using System.Collections.Generic;

namespace GOO.Model.Optimizers.SimulatedAnnealing.Strategies
{
    public class RandomRouteOpt3Strategy : Strategy
    {
        public RandomRouteOpt3Strategy()
            : base()
        {

        }

        public override Solution executeStrategy(Solution toStartFrom)
        {
            return toStartFrom;
        }
    }
}