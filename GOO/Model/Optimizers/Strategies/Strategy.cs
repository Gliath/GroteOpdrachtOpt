using System;

namespace GOO.Model.Optimizers.Strategies
{
    public abstract class Strategy
    {
        public abstract Solution executeStrategy(Solution toStartFrom);
        public abstract Solution undoStrategy(Solution toStartFrom);
    }
}