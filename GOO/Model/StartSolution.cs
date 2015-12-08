using System;
using System.Collections.Generic;

namespace GOO.Model
{
    public class StartSolution
    {
        /// <summary>
        /// Returns the generated solution
        /// </summary>
        /// <returns>The solution according to the format</returns>
        /// <Solution Format>
        ///     day;truck;sequence number;node;
        ///     Example:
        ///     1;1;1;114;  // go to node 114
        ///     1;1;2;287;  // always go back to depot node at the end of the day
        /// </Solution Format>
        public List<string> Solution { get; private set; }
        private int depot_node = 287; // Maarheeze == end_node

        public StartSolution()
        {
            Solution = new List<string>();
        }

        public void Solve()
        {

        }
    }
}
