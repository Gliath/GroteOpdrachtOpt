using System;

namespace GOO.Model.Optimizers.Strategies
{
    public class Strategy
    {
        // TODO : Figure out how to undo strategies
        public virtual Solution executeStrategy(Solution toStartFrom)
        {
            return toStartFrom;
        }

        public virtual Solution undoStrategy(Solution toStartFrom)
        {
            return toStartFrom;
        }
    }
}
