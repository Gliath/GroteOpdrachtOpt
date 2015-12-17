using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GOO.Model.Optimizers.SimulatedAnnealing
{
    public class StrategyFactory
    {
        public static Strategy[] getAllStrategies()
        {
            return new Strategy[] {new RandomSwitchOrdersInRouteStrategy()};
        }
    }
}
