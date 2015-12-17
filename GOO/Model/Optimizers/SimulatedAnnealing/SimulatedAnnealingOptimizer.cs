using System;

namespace GOO.Model.Optimizers.SimulatedAnnealing
{
    public class SimulatedAnnealingOptimizer
    {
        private AnnealingSchedule annealingSchedule;
        private object strategy;
        private Random random;

        public SimulatedAnnealingOptimizer()
        {
            annealingSchedule = new AnnealingSchedule();
            strategy = new object();
            random = new Random();
        }

        public Solution runOptimizer(Solution startSolution)
        {
            Solution currentSolution = startSolution;

            for (annealingSchedule.AnnealingIterations = 0; annealingSchedule.AnnealingTemperature > 0.0d; annealingSchedule.AnnealingIterations++)
            {
                Solution newSolution = createNewSolution(startSolution);
                if (newSolution.GetSolutionScore() > currentSolution.GetSolutionScore())
                {
                    currentSolution = newSolution;
                }
                else if (random.NextDouble() <= annealingSchedule.AnnealingTemperature)
                {
                    currentSolution = newSolution;
                } // No new solution accepted.
            }

            return startSolution;
        }

        private Solution createNewSolution(Solution currentSolution)
        {
            // TODO use strategy!
            return currentSolution;
        }

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
    }
}
