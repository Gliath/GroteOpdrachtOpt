using System;

namespace GOO.Model.Optimizers
{
    public class AnnealingSchedule
    {
        public readonly double standardStartingTemperature = 1000.0d;
        public readonly double standardAnnealingTemperatureStep = 0.1d; // 0.0000001d
        public readonly int standardMaxIterationsBeforeAnnealing = 2; // 100

        private readonly double AnnealingTemperatureStep;
        private readonly int MaxIterationsBeforeAnnealing;

        private int iterations;

        public AnnealingSchedule()
        {
            this.AnnealingTemperatureStep = standardAnnealingTemperatureStep;
            this.MaxIterationsBeforeAnnealing = standardMaxIterationsBeforeAnnealing;

            AnnealingTemperature = standardStartingTemperature;
            this.iterations = 0;
        }

        public AnnealingSchedule(double initialAnnealingTemperature, double AnnealingTemperatureStep, int MaxIterationsBeforeAnnealing)
        {
            this.AnnealingTemperatureStep = AnnealingTemperatureStep;
            this.MaxIterationsBeforeAnnealing = MaxIterationsBeforeAnnealing;

            AnnealingTemperature = initialAnnealingTemperature;
            this.iterations = 0;
        }

        public double AnnealingTemperature { get; set; }

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
    }
}

/*
 * standardStartingTemperature = 1.0d + standardAnnealingTemperatureStep = 0.001d + standardMaxIterationsBeforeAnnealing = 2 ~= 50000ms processingtime
 * standardStartingTemperature = 1.0d + standardAnnealingTemperatureStep = 0.0000001d + standardMaxIterationsBeforeAnnealing = 100 ==(estimated) 350 days processingtime
*/
