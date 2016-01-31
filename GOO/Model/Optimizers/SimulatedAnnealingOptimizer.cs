using System;
using System.Collections.Generic;

using GOO.Model;
using GOO.Model.Optimizers.Strategies;
using GOO.Utilities;

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

        public Solution runOptimizer(Solution startSolution, GOO.ViewModel.MainViewModel reportProgress)
        {
            Solution currentSolution = new Solution(startSolution.GetAllClusters());

            foreach (Tuple<Days, int, List<Route>> t in startSolution.GetEntirePlanning())
            {
                List<Route> copyRoute = new List<Route>();
                foreach (Route r in t.Item3)
                    copyRoute.Add(r);

                currentSolution.AddNewItemToPlanning(t.Item1, t.Item2, copyRoute);
            }

            oldSolutionScore = startSolution.GetSolutionScore();

            for (annealingSchedule.AnnealingIterations = 0; annealingSchedule.AnnealingTemperature > 0.0d; annealingSchedule.AnnealingIterations++)
            {
                currentSolution = SelectAndExecuteMove(currentSolution);

                // TODO: Add Planning for routes after a move has been done  -> for instance
                // Check if there are still available routes. Plan those
                // And use a strategy to either destroy the unused ones or remove used ones so that unused ones can be planned

                // Accept or reject new solution
                if (AcceptOrReject(currentSolution)) // New Solution accepted
                {
                    startSolution = currentSolution;
                    oldSolutionScore = newSolutionScore;
                    currentSolution = new Solution(startSolution.GetAllClusters());
                    //Console.WriteLine("The solution is better");

                    foreach (Tuple<Days, int, List<Route>> t in startSolution.GetEntirePlanning())
                    {
                        List<Route> copyRoute = new List<Route>();
                        foreach (Route r in t.Item3)
                            copyRoute.Add(r);

                        currentSolution.AddNewItemToPlanning(t.Item1, t.Item2, copyRoute);
                    }
                }
                else // New solution rejected
                {
                    //Console.WriteLine("The solution is not better");
                    currentSolution = startSolution;
                }

                if (reportProgress != null)
                    reportProgress.ProgressValue++;
            }

            //Console.WriteLine("SimulatedAnnealingOptimizer : Done optimizing solution!");

            return currentSolution;
        }

        //TODO : Update this method to make the calculation of what chance a move has to be executed clearer and easier to manage
        private Solution SelectAndExecuteMove(Solution toStartFrom) // Deal with Routes
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

            return strategy.executeStrategy(toStartFrom);
        }

        private int[] whatAreTheChances()
        {
            if (annealingSchedule.AnnealingTemperature < 43750.0d) // after 37.5% progression
                return new int[] { 
                    1, // new AddRouteStrategy()
                    1, // new SwapRouteStrategy()
                    1, // new DestroyRouteStrategy()
                    1, // new RemoveRouteStrategy()
                    1, // new PlanRouteStrategy()

                    5, // new RandomOrderAddStrategy()
                    5, // new RandomOrderRemoveStrategy()
                    5, // new RandomOrderShiftStrategy()
                    5, // new RandomOrderSwapStrategy()

                    //0, // new MarriageCounselorStrategy(), both not yet implemented as strategies
                    //0, // new DivorceAttourneyStrategy()

                    15, // new RandomStepOpt2Strategy()
                    15, // new RandomStepOpt2HalfStrategy()
                    15, // new RandomStepOpt3Strategy()
                    15, // new RandomStepOpt3HalfStrategy()

                    0, // new RandomRouteOpt2Strategy()
                    0, // new RandomRouteOpt2HalfStrategy()
                    0, // new RandomRouteOpt3Strategy()
                    0, // new RandomRouteOpt3HalfStrategy()

                    15, // new GeneticOneRandomRouteStrategy()
                    //0, // new GeneticTwoRandomRouteStrategy(), currently not implemented
                };
            else if (annealingSchedule.AnnealingTemperature < 49500.0d) // after 1% progression
                return new int[] { 
                    8, // new AddRouteStrategy()
                    8, // new SwapRouteStrategy()
                    8, // new DestroyRouteStrategy()
                    8, // new RemoveRouteStrategy()
                    8, // new PlanRouteStrategy()

                    10, // new RandomOrderAddStrategy()
                    10, // new RandomOrderRemoveStrategy()
                    10, // new RandomOrderShiftStrategy()
                    10, // new RandomOrderSwapStrategy()

                    //0, // new MarriageCounselorStrategy(), both not yet implemented as strategies
                    //0, // new DivorceAttourneyStrategy()

                    8, // new RandomStepOpt2Strategy()
                    7, // new RandomStepOpt2HalfStrategy()
                    0, // new RandomStepOpt3Strategy()
                    0, // new RandomStepOpt3HalfStrategy()

                    0, // new RandomRouteOpt2Strategy()
                    0, // new RandomRouteOpt2HalfStrategy()
                    0, // new RandomRouteOpt3Strategy()
                    0, // new RandomRouteOpt3HalfStrategy()

                    5, // new GeneticOneRandomRouteStrategy()
                    //0, // new GeneticTwoRandomRouteStrategy(), currently not implemented
                };
            else // before 1% progression
                return new int[] { 
                    16, // new AddRouteStrategy()
                    16, // new SwapRouteStrategy()
                    16, // new DestroyRouteStrategy()
                    16, // new RemoveRouteStrategy()
                    16, // new PlanRouteStrategy()

                    5, // new RandomOrderAddStrategy()
                    5, // new RandomOrderRemoveStrategy()
                    5, // new RandomOrderShiftStrategy()
                    5, // new RandomOrderSwapStrategy()

                    //0, // new MarriageCounselorStrategy(), both not yet implemented as strategies
                    //0, // new DivorceAttourneyStrategy()

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

        // TODO : Use or not?
        private Solution Phase3(Solution toStartFrom, List<AbstractCluster> clustersToPlan) // Distribute routes to truckers
        {
            toStartFrom.ClearTruckPlanning();
            return RoutePlanner.PlanRoutesFromClustersIntoSolution(toStartFrom, clustersToPlan);
        }

        private bool AcceptOrReject(Solution toAcceptOrReject) // Accept Solution or not
        {
            double deltaScore = oldSolutionScore - newSolutionScore;
            double chanceToBeAccepted = Math.Exp(deltaScore / annealingSchedule.AnnealingTemperature);

            return deltaScore > 0 || random.NextDouble() <= chanceToBeAccepted;
        }

        public AnnealingSchedule getAnnealingSchedule()
        {
            return annealingSchedule;
        }
    }
}