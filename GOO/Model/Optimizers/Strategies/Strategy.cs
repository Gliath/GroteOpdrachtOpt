using System;

namespace GOO.Model.Optimizers.Strategies
{
    public abstract class Strategy
    {
        protected Random random;

        public Strategy()
        {
            random = new Random();
        }

        public abstract Solution executeStrategy(Solution toStartFrom);
        public abstract Solution undoStrategy(Solution toStartFrom);
    }
}