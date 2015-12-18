using System;

namespace GOO.Model.Optimizers.SimulatedAnnealing.Strategies
{
    public abstract class Strategy
    {
        protected Random random;

        public Strategy(Random random)
        {
            this.random = random;

        }

        public virtual Solution executeStrategy(Solution toStartFrom)
        {
            return null;
        }
    }
}