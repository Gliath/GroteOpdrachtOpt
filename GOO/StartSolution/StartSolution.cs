using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace GOO.StartSolution
{
    public class StartSolution
    {

        public ArrayList solutionitems = new ArrayList();
        public static int depot_node = 287; //Maarheeze
        public static int end_node = 0; //het punt in de checker voor return

        /// <Solution Format>
        /// day;truck;sequence number;node;
        /// example
        /// 1;1;1;114;  //go to node 114
        /// 1;1;2;0;    //always go back to depot node
        /// </Solution Format>

        public StartSolution()
        {

        }
    }
}
