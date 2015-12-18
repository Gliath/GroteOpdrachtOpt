using System;

namespace GOO.Model.Optimizers.SimulatedAnnealing.Strategies
{
    public abstract class Strategy
    {
        protected static Random random = new Random();

        public Strategy()
        {
        }

        public virtual Solution executeStrategy(Solution toStartFrom)
        {
            return null;
        }
    }
}