using System;

using GOO.Model.Optimizers.Strategies;

namespace GOO.Model.Optimizers
{
    public class StrategyFactory
    {
        public static Strategy[] GetAllStrategies()
        {
            return new Strategy[] {
                new AddRouteStrategy(),
                new SwapRouteStrategy(),
                new DestroyPlannedRouteStrategy(),
                new RemoveRouteStrategy(),
                new PlanRouteStrategy(),

                new RandomOrderAddStrategy(),
                new RandomOrderRemoveStrategy(),
                new RandomOrderShiftStrategy(),
                new RandomOrderSwapStrategy(),

                //new MarriageCounselorStrategy(), both not yet implemented as strategies
                //new DivorceAttourneyStrategy(),
                
                new RandomStepOpt2Strategy(),
                new RandomStepOpt2HalfStrategy(),
                new RandomStepOpt3Strategy(),
                new RandomStepOpt3HalfStrategy(),

                // Not working correctly?
                new RandomRouteOpt2Strategy(),
                new RandomRouteOpt2HalfStrategy(),
                new RandomRouteOpt3Strategy(),
                new RandomRouteOpt3HalfStrategy(),

                new GeneticOneRandomRouteStrategy(),
                //new GeneticTwoRandomRouteStrategy(), // currently not implemented
            };
        }
    }
}