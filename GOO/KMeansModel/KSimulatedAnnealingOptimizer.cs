using System;
using System.Collections.Generic;
using System.Text;

using GOO.Model.Optimizers.SimulatedAnnealing;

namespace GOO.KMeansModel
{
    public class KSimulatedAnnealingOptimizer
    {
        private AnnealingSchedule annealingSchedule;
        private KStrategy[] strategies;
        private Random random;
        private static double theX = 2d;

        public KSimulatedAnnealingOptimizer()
        {
            annealingSchedule = new AnnealingSchedule();
            strategies = KStrategyFactory.GetAllStrategies();
            random = new Random();
        }

        public KSolution runOptimizer(KSolution startSolution)
        {
            KSolution currentSolution = startSolution;

            for (annealingSchedule.AnnealingIterations = 0; annealingSchedule.AnnealingTemperature > 0.0d; annealingSchedule.AnnealingIterations++)
            {
                KSolution newSolution = createNewSolution(startSolution);
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

        private KSolution createNewSolution(KSolution currentSolution) // TODO : Change
        {
            return strategies[(random.Next(strategies.Length - 1 <= 0 ? 0 : strategies.Length - 1))].executeStrategy(currentSolution);
        }
    }
}
