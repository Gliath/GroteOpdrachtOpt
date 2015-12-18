using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GOO.Model.Optimizers.SimulatedAnnealing.Strategies
{
    public class GeneticSolutionStrategy : Strategy
    {
        public GeneticSolutionStrategy() : base(new Random())
        {

        }

        public override Solution executeStrategy(Solution toStartFrom){
            return toStartFrom;
        }
    }
}
