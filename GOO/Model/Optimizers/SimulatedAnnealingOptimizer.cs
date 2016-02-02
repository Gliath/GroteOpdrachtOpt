using System;
using System.Collections.Generic;

using GOO.Model;
using GOO.Model.Optimizers.Strategies;
using GOO.Utilities;
using GOO.ViewModel;

namespace GOO.Model.Optimizers
{
    public class SimulatedAnnealingOptimizer
    {
        private AnnealingSchedule annealingSchedule;
        private Random random;
        private double oldSolutionScore;
        private double newSolutionScore;

        public SimulatedAnnealingOptimizer()
        {
            annealingSchedule = new AnnealingSchedule();
            random = new Random();
            oldSolutionScore = Double.MaxValue;
            newSolutionScore = Double.MaxValue;
        }

        public Solution runOptimizer(Solution solution, GOO.ViewModel.MainViewModel reportProgress)
        {
            oldSolutionScore = solution.SolutionScore;
            int annealingCounter = 0;
            int AllRoutesCount = -1;
            int AvailableCount = -1;
            Strategy strategyUsed = null;

            for (annealingSchedule.AnnealingIterations = 0; annealingSchedule.AnnealingTemperature > 0.0d; annealingSchedule.AnnealingIterations++)
            {
                AllRoutesCount = solution.AllRoutes.Count;
                AvailableCount = solution.AvailableRoutes.Count;

                Strategy usedStrategy = SelectAndExecuteMove(solution);
                newSolutionScore = solution.SolutionScore;
                // Possible effects:
                // OrderAdd                 (None)
                // OrderRemove              (None)
                // OrderSwap!               (+2 Available)
                // OrderShift!              (+2 Available)
                // RouteDestroyAvailable!   (-1 All (vaak?), -1 Available)
                // RouteDestroyPlanned      (-1 All)
                // RoutePlan                (-1 Available)
                // RouteRemove              (+1 Available)
                // RouteSwap                (None)
                // RouteAdd                 (None)
                // Opt2Half!                (+1 All)
                // Opt2                     (None)

                // Accept or reject new solution
                if (AcceptOrReject(solution)) // New Solution accepted
                    oldSolutionScore = newSolutionScore;
                else // New solution rejected
                    usedStrategy.undoStrategy(solution);

                if (reportProgress != null)
                    reportProgress.ProgressValue++;

                annealingCounter++;
                strategyUsed = usedStrategy;
            }

            return solution;
        }

        //TODO : Update this method to make the calculation of what chance a move has to be executed clearer and easier to manage
        private Strategy SelectAndExecuteMove(Solution toStartFrom) // Deal with Routes
        {
            int[] chances = whatAreTheChances();
            int totalPercentages = 0;
            int randomSelectedNumber = random.Next(100);
            Strategy strategy = null;
            for (int i = 0; i < chances.Length; i++)
            {
                totalPercentages += chances[i];
                if (totalPercentages > randomSelectedNumber) // randomSelectionNumber is 0-based, meaning +1 if comparing with whatAreTheChances()
                {
                    strategy = StrategyFactory.GetAllStrategies()[i];
                    break;
                }
            }
            strategy.executeStrategy(toStartFrom);
            return strategy;
        }

        private int[] whatAreTheChances()
        {
            if (annealingSchedule.AnnealingTemperature < 43750.0d) // after 37.5% progression
                return new int[] { 
                    1, // new AddRouteStrategy()
                    2, // new SwapRouteStrategy()
                    1, // new DestroyPlannedRouteStrategy()
                    1, // new DestroyPoolRouteStrategy()
                    1, // new RemoveRouteStrategy()
                    2, // new PlanRouteStrategy()

                    6, // new RandomOrderAddStrategy()
                    6, // new RandomOrderRemoveStrategy()
                    6, // new RandomOrderShiftStrategy()
                    6, // new RandomOrderSwapStrategy()

                    18, // new RandomStepOpt2Strategy()
                    17, // new RandomStepOpt2HalfStrategy()
                    18, // new RandomStepOpt3Strategy()
                    17, // new RandomStepOpt3HalfStrategy()

                    0, // new RandomRouteOpt2Strategy()
                    0, // new RandomRouteOpt2HalfStrategy()
                    0, // new RandomRouteOpt3Strategy()
                    0, // new RandomRouteOpt3HalfStrategy()

                    0, // new GeneticOneRandomRouteStrategy()
                    //0, // new GeneticTwoRandomRouteStrategy(), currently not implemented
                };
            else if (annealingSchedule.AnnealingTemperature < 48500.0d) // after 3% progression
                return new int[] { 
                    8, // new AddRouteStrategy()
                    8, // new SwapRouteStrategy()
                    4, // new DestroyPlannedRouteStrategy()
                    4, // new DestroyPoolRouteStrategy()
                    8, // new RemoveRouteStrategy()
                    8, // new PlanRouteStrategy()

                    11, // new RandomOrderAddStrategy()
                    11, // new RandomOrderRemoveStrategy()
                    11, // new RandomOrderShiftStrategy()
                    11, // new RandomOrderSwapStrategy()

                    8, // new RandomStepOpt2Strategy()
                    8, // new RandomStepOpt2HalfStrategy()
                    0, // new RandomStepOpt3Strategy()
                    0, // new RandomStepOpt3HalfStrategy()

                    0, // new RandomRouteOpt2Strategy()
                    0, // new RandomRouteOpt2HalfStrategy()
                    0, // new RandomRouteOpt3Strategy()
                    0, // new RandomRouteOpt3HalfStrategy()

                    0, // new GeneticOneRandomRouteStrategy()
                    //0, // new GeneticTwoRandomRouteStrategy(), currently not implemented
                };
            else if (annealingSchedule.AnnealingTemperature < 49000.0d) // after 2% progression
                return new int[] { 
                    0, // new AddRouteStrategy()
                    0, // new SwapRouteStrategy()
                    100, // new DestroyPlannedRouteStrategy()
                    0, // new DestroyPoolRouteStrategy()
                    0, // new RemoveRouteStrategy()
                    0, // new PlanRouteStrategy()
                    
                    0, // new RandomOrderAddStrategy()
                    0, // new RandomOrderRemoveStrategy()
                    0, // new RandomOrderShiftStrategy()
                    0, // new RandomOrderSwapStrategy()
                    
                    0, // new RandomStepOpt2Strategy()
                    0, // new RandomStepOpt2HalfStrategy()
                    0, // new RandomStepOpt3Strategy()
                    0, // new RandomStepOpt3HalfStrategy()

                    0, // new RandomRouteOpt2Strategy()
                    0, // new RandomRouteOpt2HalfStrategy()
                    0, // new RandomRouteOpt3Strategy()
                    0, // new RandomRouteOpt3HalfStrategy()

                    0, // new GeneticOneRandomRouteStrategy()
                    //0, // new GeneticTwoRandomRouteStrategy(), currently not implemented
                };
            else if (annealingSchedule.AnnealingTemperature < 49500.0d) // after 1% progression
                return new int[] { 
                    0, // new AddRouteStrategy()
                    0, // new SwapRouteStrategy()
                    0, // new DestroyPlannedRouteStrategy()
                    0, // new DestroyPoolRouteStrategy()
                    0, // new RemoveRouteStrategy()
                    100, // new PlanRouteStrategy()
                    
                    0, // new RandomOrderAddStrategy()
                    0, // new RandomOrderRemoveStrategy()
                    0, // new RandomOrderShiftStrategy()
                    0, // new RandomOrderSwapStrategy()
                    
                    0, // new RandomStepOpt2Strategy()
                    0, // new RandomStepOpt2HalfStrategy()
                    0, // new RandomStepOpt3Strategy()
                    0, // new RandomStepOpt3HalfStrategy()

                    0, // new RandomRouteOpt2Strategy()
                    0, // new RandomRouteOpt2HalfStrategy()
                    0, // new RandomRouteOpt3Strategy()
                    0, // new RandomRouteOpt3HalfStrategy()

                    0, // new GeneticOneRandomRouteStrategy()
                    //0, // new GeneticTwoRandomRouteStrategy(), currently not implemented
                };
            else // before 1% progression
                return new int[] { 
                    100, // new AddRouteStrategy()
                    0, // new SwapRouteStrategy()
                    0, // new DestroyPlannedRouteStrategy()
                    0, // new DestroyPoolRouteStrategy()
                    0, // new RemoveRouteStrategy()
                    0, // new PlanRouteStrategy()

                    0, // new RandomOrderAddStrategy()
                    0, // new RandomOrderRemoveStrategy()
                    0, // new RandomOrderShiftStrategy()
                    0, // new RandomOrderSwapStrategy()

                    0, // new RandomStepOpt2Strategy()
                    0, // new RandomStepOpt2HalfStrategy()
                    0, // new RandomStepOpt3Strategy()
                    0, // new RandomStepOpt3HalfStrategy()

                    0, // new RandomRouteOpt2Strategy()
                    0, // new RandomRouteOpt2HalfStrategy()
                    0, // new RandomRouteOpt3Strategy()
                    0, // new RandomRouteOpt3HalfStrategy()

                    0, // new GeneticOneRandomRouteStrategy()
                    //0, // new GeneticTwoRandomRouteStrategy(), currently not implemented
                };
        }

        private bool AcceptOrReject(Solution toAcceptOrReject) // Accept Solution or not
        {
            double deltaScore = oldSolutionScore - newSolutionScore;
            double chanceToBeAccepted = Math.Exp(deltaScore / annealingSchedule.AnnealingTemperature);

            return deltaScore >= 0 || random.NextDouble() <= chanceToBeAccepted;
        }

        public AnnealingSchedule getAnnealingSchedule()
        {
            return annealingSchedule;
        }
    }
}