using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GOO.KMeansModel
{
    public class KStrategy
    {
        // TODO : Figure out how to undo strategies
        public virtual KSolution executeStrategy(KSolution toStartFrom)
        {
            return toStartFrom;
        }

        public virtual KSolution undoStrategy(KSolution toStartFrom)
        {
            return toStartFrom;
        }
    }
}
