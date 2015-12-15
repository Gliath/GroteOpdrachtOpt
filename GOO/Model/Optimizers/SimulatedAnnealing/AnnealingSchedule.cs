using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GOO.Model.Optimizers.SimulatedAnnealing
{
    public class AnnealingSchedule
    {
        public AnnealingSchedule()
        {
            this.AnnealingTemperatureStep = standardAnnealingTemperatureStep;
            this.MaxIterationsBeforeAnnealing = standardMaxIterationsBeforeAnnealing;

            this.temperature = standardStartingTemperature;
            this.iterations = 0;
        }

        public AnnealingSchedule(double initialAnnealingTemperature, double AnnealingTemperatureStep, int MaxIterationsBeforeAnnealing)
        {
            this.AnnealingTemperatureStep = AnnealingTemperatureStep;
            this.MaxIterationsBeforeAnnealing = MaxIterationsBeforeAnnealing;

            this.temperature = 0;
            this.iterations = 0;
        }

        public double AnnealingTemperature { 
            get {return temperature;} 
            set {temperature = value;} 
        }

        public int AnnealingIterations
        {
            get { return iterations; } 
            
            set{ 
                int toWork = value;
                iterations = toWork > MaxIterationsBeforeAnnealing ? 0 : toWork;
            }
 
        }

        public readonly double standardStartingTemperature = 1.0d;
        public readonly double standardAnnealingTemperatureStep = 0.0000001d;
        public readonly int standardMaxIterationsBeforeAnnealing = 100;

        private readonly double AnnealingTemperatureStep;
        private readonly int MaxIterationsBeforeAnnealing;

        private double temperature;
        private int iterations;

    }
}
