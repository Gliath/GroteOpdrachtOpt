using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GOO.Model.Optimizers.SimulatedAnnealing
{
    public abstract class Strategy
    {
        public Strategy()
        {

        }

        public virtual Solution executeStrategy(Solution toStartFrom);
    }
}
