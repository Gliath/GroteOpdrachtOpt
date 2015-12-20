using System;

using GOO.Model.Optimizers.Strategies;

namespace GOO.Model.Optimizers
{
    public class SimulatedAnnealingOptimizer
    {
        private AnnealingSchedule annealingSchedule;
        private Strategy[] strategies;
        private Random random;
        private static double theX = 2d;

        public SimulatedAnnealingOptimizer()
        {
            annealingSchedule = new AnnealingSchedule();
            strategies = StrategyFactory.GetAllStrategies();
            random = new Random();
        }

        public Solution runOptimizer(Solution startSolution)
        {
            Solution currentSolution = startSolution;

            for (annealingSchedule.AnnealingIterations = 0; annealingSchedule.AnnealingTemperature > 0.0d; annealingSchedule.AnnealingIterations++)
            {
                Solution newSolution = createNewSolution(startSolution);
                double currentSolutionScore = currentSolution.GetSolutionScore();
                double newSolutionScore = newSolution.GetSolutionScore();
                double deltaScore = currentSolutionScore - newSolutionScore;
                double chanceToBeAccepted =  Math.Pow(theX, (deltaScore/annealingSchedule.AnnealingTemperature));

                if (deltaScore > 0)
                {
                    currentSolution = newSolution;
                }
                    
                else if (random.NextDouble() <= chanceToBeAccepted)
                {
                    currentSolution = newSolution;
                } // No new solution accepted.
            }

            return currentSolution;
        }

        private Solution createNewSolution(Solution currentSolution) // TODO : Change
        {
            return strategies[(random.Next(strategies.Length - 1 <= 0 ? 0 : strategies.Length - 1))].executeStrategy(currentSolution);
        }
    }
}
