using System;

namespace GOO.Model.Optimizers.SimulatedAnnealing
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