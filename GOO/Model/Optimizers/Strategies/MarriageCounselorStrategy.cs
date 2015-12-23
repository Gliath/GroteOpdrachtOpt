using System;
using System.Collections.Generic;

using GOO.Utilities;

namespace GOO.Model.Optimizers.Strategies
{
    public class MarriageCounselorStrategy : Strategy
    {
        public MarriageCounselorStrategy()
            : base()
        {

        }

        public override Solution executeStrategy(Solution toStartFrom)
        {

            return toStartFrom;
        }

        public override Solution undoStrategy(Solution toStartFrom)
        {


            return toStartFrom;
        }
    }
}