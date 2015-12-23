using System;
using System.Collections.Generic;

namespace GOO.Model.Optimizers.Strategies
{
    public class DivorceAttourneyStrategy : Strategy
    {
        public DivorceAttourneyStrategy()
            : base()
        {

        }

        public override Solution executeStrategy(Solution toStartFrom)
        {
            // Randomly divorce clusters or divorce them because it can make filled clusters?


            return toStartFrom;
        }

        public override Solution undoStrategy(Solution toStartFrom)
        {


            return toStartFrom;
        }
    }
}