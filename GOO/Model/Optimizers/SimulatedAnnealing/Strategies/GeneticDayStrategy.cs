using System;
using System.Collections.Generic;

namespace GOO.Model.Optimizers.SimulatedAnnealing.Strategies
{
    public class GeneticDayStrategy : Strategy
    {
        public GeneticDayStrategy()
            : base()
        {

        }

        public override Solution executeStrategy(Solution toStartFrom){
            return toStartFrom;
        }
    }
}
