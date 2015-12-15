using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GOO.Model.Optimizers.SimulatedAnnealing
{
    public class SimulatedAnnealingOptimizer
    {
        public SimulatedAnnealingOptimizer() { }

        // Annealing schedule 

        public Solution runOptimizer(Solution startSolution)
        {
            // For all iterations until annealing temperature is 0

                // Create random other solution by utilizing a random strategy

                // Compare the scores of current solution and random strategy

                    // If new solution better, accept it and continue

                    // Else roll if accepted
                        // Accepted, continue

                        // Rejected, re-do iteration.


            /*
             * function HILL-CLIMBING(problem) returns a state that is a local maximum
                current ← MAKE-NODE(problem.INITIAL-STATE)
                loop do
                neighbor ← a highest-valued successor of current
                if neighbor.VALUE ≤ current.VALUE then return current.STATE
                current ← neighbor
             * 
             * function SIMULATED-ANNEALING(problem, schedule) returns a solution state
                inputs: problem, a problem
                schedule, a mapping from time to “temperature”
                current ← MAKE-NODE(problem.INITIAL-STATE)
                for t = 1 to ∞ do
                T ← schedule(t)
                if T = 0 then return current
                next ← a randomly selected successor of current
                ΔE ← next.VALUE – current.VALUE
                if ΔE > 0 then current ← next
                else current ← next only with probability eΔE/T
             * 
             */

            return startSolution;
        }
    }
}
