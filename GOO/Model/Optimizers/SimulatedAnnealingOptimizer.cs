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
        private int[] chances;

        public SimulatedAnnealingOptimizer()
        {
            annealingSchedule = new AnnealingSchedule();
            random = new Random();
            oldSolutionScore = Double.MaxValue;
            newSolutionScore = Double.MaxValue;
            chances = new int[] { 
                    2, // new AddRouteStrategy()               
                    2, // new SwapRouteStrategy()               
                    1, // new DestroyPlannedRouteStrategy()     
                    1, // new DestroyPoolRouteStrategy()        
                    2, // new RemoveRouteStrategy()             
                    30, // new PlanRouteStrategy()              

                    20, // new RandomOrderAddStrategy()         
                    10, // new RandomOrderRemoveStrategy()       
                    2, // new RandomOrderShiftStrategy()        
                    10, // new RandomOrderSwapStrategy()         

                    10, // new RandomStepOpt2Strategy()
                    10, // new RandomStepOpt2HalfStrategy()
                    5, // new RandomStepOpt3Strategy()
                    5, // new RandomStepOpt3HalfStrategy()

                    0, // new RandomRouteOpt2Strategy()
                    0, // new RandomRouteOpt2HalfStrategy()
                    0, // new RandomRouteOpt3Strategy()
                    0, // new RandomRouteOpt3HalfStrategy()

                    0, // new GeneticOneRandomRouteStrategy()
                    //0, // new GeneticTwoRandomRouteStrategy(), currently not implemented
                };
        }

        public Solution runOptimizer(Solution solution, GOO.ViewModel.MainViewModel reportProgress)
        {
            oldSolutionScore = solution.SolutionScore;
            int annealingCounter = 0;
            int lastMinuteMark = 0;
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();

            for (annealingSchedule.AnnealingIterations = 0; annealingSchedule.AnnealingTemperature > 0.0d; annealingSchedule.AnnealingIterations++)
            {
                Strategy usedStrategy = SelectAndExecuteMove(solution);
                newSolutionScore = solution.SolutionScore;

                // Accept or reject new solution
                bool AcceptedorRejected = AcceptOrReject(solution);
                if (AcceptedorRejected) // New Solution accepted
                    oldSolutionScore = newSolutionScore;
                else // New solution rejected
                    usedStrategy.undoStrategy(solution);

                if (reportProgress != null)
                    reportProgress.ProgressValue++;

                bool printCurrentSolution = false;
                if (printCurrentSolution)
                    System.IO.File.WriteAllText(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + "/Solution" + annealingCounter + ".txt", solution.ToString());

                annealingCounter++;

                sw.Stop();
                if (sw.ElapsedMilliseconds % 60000 <= 1)
                {
                    int minuteMark = (int)(sw.ElapsedMilliseconds / (long)60000);
                    if (lastMinuteMark < minuteMark)
                    {
                        lastMinuteMark = minuteMark;
                        Console.WriteLine("Elapsed {0}ms and made {1} iterations", sw.ElapsedMilliseconds, annealingCounter);
                    }
                }
                sw.Start();
            }
            sw.Stop();
            Console.WriteLine("Finished running in {0}ms and with {1} iterations", sw.ElapsedMilliseconds, annealingCounter);

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
            //if (annealingSchedule.AnnealingTemperature < 4000.0d) // after 2% progression
            //    return new int[] { 
            //        5, // new AddRouteStrategy()
            //        10, // new SwapRouteStrategy()
            //        0, // new DestroyPlannedRouteStrategy()
            //        0, // new DestroyPoolRouteStrategy()
            //        5, // new RemoveRouteStrategy()
            //        5, // new PlanRouteStrategy()
                    
            //        5, // new RandomOrderAddStrategy()
            //        5, // new RandomOrderRemoveStrategy()
            //        5, // new RandomOrderShiftStrategy()
            //        5, // new RandomOrderSwapStrategy()
                    
            //        18, // new RandomStepOpt2Strategy()
            //        18, // new RandomStepOpt2HalfStrategy()
            //        9, // new RandomStepOpt3Strategy()
            //        10, // new RandomStepOpt3HalfStrategy()

            //        0, // new RandomRouteOpt2Strategy()
            //        0, // new RandomRouteOpt2HalfStrategy()
            //        0, // new RandomRouteOpt3Strategy()
            //        0, // new RandomRouteOpt3HalfStrategy()

            //        0, // new GeneticOneRandomRouteStrategy()
            //        //0, // new GeneticTwoRandomRouteStrategy(), currently not implemented
            //    };
            //else if (annealingSchedule.AnnealingTemperature < 9500.0d) // after 1% progression
            //    return new int[] { 
            //        10, // new AddRouteStrategy()
            //        10, // new SwapRouteStrategy()
            //        0, // new DestroyPlannedRouteStrategy()
            //        0, // new DestroyPoolRouteStrategy()
            //        2, // new RemoveRouteStrategy()
            //        20, // new PlanRouteStrategy()
                    
            //        23, // new RandomOrderAddStrategy()
            //        5, // new RandomOrderRemoveStrategy()
            //        10, // new RandomOrderShiftStrategy()
            //        10, // new RandomOrderSwapStrategy()
                    
            //        5, // new RandomStepOpt2Strategy()
            //        5, // new RandomStepOpt2HalfStrategy()
            //        0, // new RandomStepOpt3Strategy()
            //        0, // new RandomStepOpt3HalfStrategy()

            //        0, // new RandomRouteOpt2Strategy()
            //        0, // new RandomRouteOpt2HalfStrategy()
            //        0, // new RandomRouteOpt3Strategy()
            //        0, // new RandomRouteOpt3HalfStrategy()

            //        0, // new GeneticOneRandomRouteStrategy()
            //        //0, // new GeneticTwoRandomRouteStrategy(), currently not implemented
            //    };
            //else // before 1% progression
                return chances;
        }

        private bool AcceptOrReject(Solution toAcceptOrReject) // Accept Solution or not
        {
            double deltaScore = newSolutionScore - oldSolutionScore;
            double chanceToBeAccepted = 1.0 / ( 1+ Math.Exp(deltaScore / annealingSchedule.AnnealingTemperature) );

            return deltaScore <= 0 || random.NextDouble() <= chanceToBeAccepted;
        }

        public AnnealingSchedule getAnnealingSchedule()
        {
            return annealingSchedule;
        }
    }
}