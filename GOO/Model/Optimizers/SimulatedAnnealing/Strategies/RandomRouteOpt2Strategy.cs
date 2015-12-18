using System;
using System.Collections.Generic;

namespace GOO.Model.Optimizers.SimulatedAnnealing.Strategies
{
    public class RandomRouteOpt2Strategy : Strategy
    {
        public RandomRouteOpt2Strategy()
            : base()
        {

        }

        public override Solution executeStrategy(Solution toStartFrom)
        {
            return toStartFrom;
        }
    }
}