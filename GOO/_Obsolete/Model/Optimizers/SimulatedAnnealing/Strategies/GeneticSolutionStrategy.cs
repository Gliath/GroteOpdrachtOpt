using System;
using System.Collections.Generic;

namespace GOO.Obsolete.Model.Optimizers.SimulatedAnnealing.Strategies
{
    public class GeneticSolutionStrategy : Strategy
    {
        public GeneticSolutionStrategy()
            : base()
        {

        }

        public override Solution executeStrategy(Solution toStartFrom)
        {
            return toStartFrom;
        }
    }
}