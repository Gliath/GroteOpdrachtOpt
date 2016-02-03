using System;

namespace GOO.Model.Optimizers
{
    public class AnnealingSchedule
    {
        private readonly double standardStartingTemperature = 10000.0d;
        private readonly double standardAnnealingTemperatureStep = 0.0003d;
        private readonly int standardMaxIterationsBeforeAnnealing = 1;

        public double AnnealingTemperature { get; private set; }
        public double AnnealingTemperatureStep { get; private set; }
        public int MaxIterationsBeforeAnnealing { get; private set; }

        public AnnealingSchedule()
        {
            this.AnnealingTemperature = standardStartingTemperature;
            this.AnnealingTemperatureStep = standardAnnealingTemperatureStep;
            this.MaxIterationsBeforeAnnealing = standardMaxIterationsBeforeAnnealing;

            this.iterations = 0;
        }

        public AnnealingSchedule(double initialAnnealingTemperature, double AnnealingTemperatureStep, int MaxIterationsBeforeAnnealing)
        {
            this.AnnealingTemperature = initialAnnealingTemperature;
            this.AnnealingTemperatureStep = AnnealingTemperatureStep;
            this.MaxIterationsBeforeAnnealing = MaxIterationsBeforeAnnealing;

            this.iterations = 0;
        }

        private int iterations;
        public int AnnealingIterations
        {
            get { return iterations; }

            set
            {
                if(value >= MaxIterationsBeforeAnnealing)
                {
                    iterations = 0;
                    AnnealingTemperature -= AnnealingTemperatureStep;
                }
                else
                    iterations = value;
            }
        }

        public double getMaximumNumberOfIterations()
        {
            return AnnealingTemperature * AnnealingTemperatureStep * MaxIterationsBeforeAnnealing;
        }
    }
}

/* New Cluster model:
 * standardStartingTemperature = 50000.0d + standardAnnealingTemperatureStep = 0.003d + standardMaxIterationsBeforeAnnealing = 1 ~= UNKNOWNms processingtime
 * 
 * Old Clusters model:
 * standardStartingTemperature = 1000.0d + standardAnnealingTemperatureStep = 0.1d + standardMaxIterationsBeforeAnnealing = 2 ~= UNKNOWNms processingtime
 * 
 * Obsolete model:
 * standardStartingTemperature = 1.0d + standardAnnealingTemperatureStep = 0.001d + standardMaxIterationsBeforeAnnealing = 2 ~= 50000ms processingtime
 * standardStartingTemperature = 1.0d + standardAnnealingTemperatureStep = 0.0000001d + standardMaxIterationsBeforeAnnealing = 100 ==(estimated) 350 days processingtime
*/