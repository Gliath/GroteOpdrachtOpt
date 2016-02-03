using System;

namespace GOO.Model.Optimizers.Strategies
{
    public abstract class Strategy
    {
        protected Random random;
        protected bool strategyHasExecuted;

        public Strategy()
        {
            random = new Random();
            strategyHasExecuted = false;
        }

        public abstract Solution executeStrategy(Solution toStartFrom);
        public abstract Solution undoStrategy(Solution toStartFrom);
    }
}