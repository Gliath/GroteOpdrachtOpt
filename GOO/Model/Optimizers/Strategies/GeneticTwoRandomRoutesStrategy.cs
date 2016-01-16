using System;
using System.Collections.Generic;

using GOO.Utilities;

namespace GOO.Model.Optimizers.Strategies
{
    public class GeneticTwoRandomRoutesStrategy : Strategy
    {
        private Route firstOriginalRoute;
        private Route secondOriginalRoute;
        private Route firstAbominationRoute;
        private Route secondAbominationRoute;
        private Route firstBestAbominationOffspringRoute;
        private Route secondBestAbominationOffspringRoute;
        private Tuple<Days, int, List<Route>> planningForSelectedRoute;

        public GeneticTwoRandomRoutesStrategy()
            : base()
        {
            firstOriginalRoute = null;
            secondOriginalRoute = null;
            firstAbominationRoute = null;
            secondAbominationRoute = null;
            firstBestAbominationOffspringRoute = null;
            secondBestAbominationOffspringRoute = null;
            planningForSelectedRoute = null;
        }

        public override Solution executeStrategy(Solution toStartFrom)
        {
            planningForSelectedRoute = toStartFrom.getRandomPlanning();
            firstOriginalRoute = planningForSelectedRoute.Item3[random.Next(planningForSelectedRoute.Item3.Count)];

            do // Check dayrestrictions?
                secondOriginalRoute = planningForSelectedRoute.Item3[random.Next(planningForSelectedRoute.Item3.Count)];
            while (firstOriginalRoute != secondOriginalRoute);

            double firstOriginalTravelTime = firstOriginalRoute.TravelTime;
            double secondOriginalTravelTime = secondOriginalRoute.TravelTime;

            // COMMENCE THE GENETIC EXPERIMENTS!
            // OPERATION DARWIN'S ABOMINATION REPRODUCTION IS NOW OPERATIONAL!

            int numOfGenerationsToMake = 64;
            Route bestBoyAbominationOffspring = firstOriginalRoute;
            Route bestGirlAbominationOffspring = secondOriginalRoute;
            Boolean boyAbominationIsTheBest = bestBoyAbominationOffspring.TravelTime >= bestGirlAbominationOffspring.TravelTime;
            int numberOfGenesThatAreTransferred = firstOriginalRoute.Orders.Count / 2;
            List<Order> ordersSelectedForGeneticSwap = new List<Order>();

            while (numOfGenerationsToMake > 0)
            {
                ordersSelectedForGeneticSwap.Clear();

                int orderIndex = -1;
                do
                {
                    orderIndex = random.Next(numberOfGenesThatAreTransferred); // NEEDS FIXING
                    Order randomOrder = null;

                    if (boyAbominationIsTheBest) // NEEDS FIXING?
                        randomOrder = bestBoyAbominationOffspring.Orders[orderIndex];
                    else
                        randomOrder = bestGirlAbominationOffspring.Orders[orderIndex];

                    if (ordersSelectedForGeneticSwap.Contains(randomOrder))
                        continue;

                    ordersSelectedForGeneticSwap.Add(randomOrder);

                } while (ordersSelectedForGeneticSwap.Count * 2 < numberOfGenesThatAreTransferred - 1);

                List<int> indicesToBeSwaped = new List<int>();
                foreach (Order order in ordersSelectedForGeneticSwap)
                {
                    indicesToBeSwaped.Add(bestBoyAbominationOffspring.Orders.FindIndex(o => o == order));
                    indicesToBeSwaped.Add(bestGirlAbominationOffspring.Orders.FindIndex(o => o == order));
                }
                indicesToBeSwaped.Sort();

                Route newBoyAbominationOffspring = new Route(firstOriginalRoute.Day); // NEEDS FIXING
                Route newGirlAbominationOffspring = new Route(firstOriginalRoute.Day); // NEEDS FIXING

                for (int index = 0; index < firstOriginalRoute.Orders.Count - 1; index++)
                {
                    if (indicesToBeSwaped.Contains(index)) // What?
                    {
                        newBoyAbominationOffspring.AddOrder(bestGirlAbominationOffspring.Orders[index]);
                        newGirlAbominationOffspring.AddOrder(bestBoyAbominationOffspring.Orders[index]);
                    }
                    else
                    {
                        newBoyAbominationOffspring.AddOrder(bestBoyAbominationOffspring.Orders[index]);
                        newGirlAbominationOffspring.AddOrder(bestGirlAbominationOffspring.Orders[index]);
                    }
                }

                Boolean newGirlAbominationOffspringIstheBest = newBoyAbominationOffspring.TravelTime <= newGirlAbominationOffspring.TravelTime;
                if (boyAbominationIsTheBest)
                {
                    if (newGirlAbominationOffspringIstheBest)
                    {
                        if (bestGirlAbominationOffspring.TravelTime < newGirlAbominationOffspring.TravelTime)
                        {
                            bestGirlAbominationOffspring = newGirlAbominationOffspring;

                            if(bestBoyAbominationOffspring.TravelTime < newBoyAbominationOffspring.TravelTime)
                            {
                                bestBoyAbominationOffspring = newBoyAbominationOffspring;
                            }
                        }
                    }
                    else
                    {
                        if (bestGirlAbominationOffspring.TravelTime < newBoyAbominationOffspring.TravelTime)
                        {
                            bestGirlAbominationOffspring = newBoyAbominationOffspring;

                            if (bestBoyAbominationOffspring.TravelTime < newGirlAbominationOffspring.TravelTime)
                            {
                                bestBoyAbominationOffspring = newGirlAbominationOffspring;
                            }
                        }
                    }
                }
                else
                {
                    if (newGirlAbominationOffspringIstheBest)
                    {
                        if (bestBoyAbominationOffspring.TravelTime < newGirlAbominationOffspring.TravelTime)
                        {
                            bestBoyAbominationOffspring = newGirlAbominationOffspring;

                            if (bestGirlAbominationOffspring.TravelTime < newBoyAbominationOffspring.TravelTime)
                            {
                                bestGirlAbominationOffspring = newBoyAbominationOffspring;
                            }
                        }
                    }
                    else
                    {
                        if (bestBoyAbominationOffspring.TravelTime < newBoyAbominationOffspring.TravelTime)
                        {
                            bestBoyAbominationOffspring = newBoyAbominationOffspring;

                            if (bestGirlAbominationOffspring.TravelTime < newGirlAbominationOffspring.TravelTime)
                            {
                                bestGirlAbominationOffspring = newGirlAbominationOffspring;
                            }
                        }
                    }
                }

                boyAbominationIsTheBest = bestBoyAbominationOffspring.TravelTime >= bestGirlAbominationOffspring.TravelTime;
                numOfGenerationsToMake--;
            }

            // PHASE 3 - INSERT THE ABOMINATION INTO THE PUBLIC!

            firstBestAbominationOffspringRoute = boyAbominationIsTheBest ? bestBoyAbominationOffspring : bestGirlAbominationOffspring;
            // secondBest?

            Console.WriteLine("First Route: Original Travel Time:                     {0}", firstOriginalTravelTime); // NEEDS FIXING
            Console.WriteLine("First Route: The First Abomination Travel Time:        {0}", firstAbominationRoute.TravelTime); // NEEDS FIXING
            Console.WriteLine("First Route: Best Abomination Offspring Travel Time:   {0}", firstBestAbominationOffspringRoute.TravelTime); // NEEDS FIXING

            planningForSelectedRoute.Item3.Remove(firstOriginalRoute); // NEEDS FIXING
            planningForSelectedRoute.Item3.Add(firstBestAbominationOffspringRoute); // NEEDS FIXING

            toStartFrom.RemoveItemFromPlanning(planningForSelectedRoute.Item1, planningForSelectedRoute.Item2); // NEEDS FIXING
            toStartFrom.AddNewItemToPlanning(planningForSelectedRoute.Item1, planningForSelectedRoute.Item2, planningForSelectedRoute.Item3); // NEEDS FIXING

            return toStartFrom;
        }

        public override Solution undoStrategy(Solution toStartFrom)
        {
            // Revert abomination to Patient Zero...
            planningForSelectedRoute.Item3.Remove(firstBestAbominationOffspringRoute); // NEEDS FIXING
            planningForSelectedRoute.Item3.Add(firstOriginalRoute); // NEEDS FIXING

            toStartFrom.RemoveItemFromPlanning(planningForSelectedRoute.Item1, planningForSelectedRoute.Item2); // NEEDS FIXING
            toStartFrom.AddNewItemToPlanning(planningForSelectedRoute.Item1, planningForSelectedRoute.Item2, planningForSelectedRoute.Item3); // NEEDS FIXING

            return toStartFrom;
        }
    }
}