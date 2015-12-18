using System;
using System.Collections.Generic;

namespace GOO.Model.Optimizers.SimulatedAnnealing.Strategies
{
    public class GeneticRouteStrategy: Strategy
    {
        public GeneticRouteStrategy()
            : base()
        {

        }

        public override Solution executeStrategy(Solution toStartFrom)
        {
            return toStartFrom;
        }
    }
}