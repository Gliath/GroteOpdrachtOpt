using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GOO.Utilities
{
    public class Route
    {

        double traveltime;
        List<Object> orders;
        int weight;
        int forced_day = 0;  //0 = vrij , 1 = ma, 2 = di

        public Route()
        {

        }
    }
}