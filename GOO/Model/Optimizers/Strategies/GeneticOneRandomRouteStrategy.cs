using System;
using System.Collections.Generic;

using GOO.Utilities;

namespace GOO.Model.Optimizers.Strategies
{
    public class GeneticOneRandomRouteStrategy : Strategy
    {
        private Route originalRoute;
        private Route bestAbominationOffspringRoute;
        private Tuple<Days, int, List<Route>> planningForSelectedRoute;

        public GeneticOneRandomRouteStrategy()
            : base()
        {
            originalRoute = null;
            bestAbominationOffspringRoute = null;
            planningForSelectedRoute = null;
        }

        public override Solution executeStrategy(Solution toStartFrom)
        {
            planningForSelectedRoute = toStartFrom.GetRandomPlanning();
            originalRoute = planningForSelectedRoute.Item3[random.Next(planningForSelectedRoute.Item3.Count)];
            double originalTravelTime = originalRoute.TravelTime;
            int numOfSlices = random.Next(1, originalRoute.Orders.Count - 2);

            List<int> sliceIndices = new List<int>();
            for (int i = 0; i < numOfSlices; i++)
            {
                bool hasAddedAnIndex = false;
                do
                {
                    int sliceIndex = random.Next(1, numOfSlices);
                    if (!sliceIndices.Contains(sliceIndex))
                    {
                        sliceIndices.Add(sliceIndex);
                        hasAddedAnIndex = true;
                    }
                } while (!hasAddedAnIndex);
            }
            sliceIndices.Sort();

            // COMMENCE THE GENETIC EXPERIMENTS!
            // OPERATION SLICE & SPLICE AN ABOMINATION IS ACTIVE!

            List<Order>[] orderSlices = new List<Order>[sliceIndices.Count];
            for (int i = 0; i < sliceIndices.Count; i++)
            {
                orderSlices[i] = new List<Order>();
                int startIndex = i == 0 ? 0 : sliceIndices[i - 1];

                for (int j = startIndex; j < sliceIndices[i]; j++)
                    orderSlices[i].Add(originalRoute.Orders[j]);
            }

            // Now that you've sliced up the orders in the selected route, it is time for randomizing the slices and putting it back together

            Route firstAbominationRoute = new Route(originalRoute.Day);
            List<int> slicesIndexPutBack = new List<int>();
            for (int i = 0; i < orderSlices.Length - 2; i++)
            {
                bool hasPutBackASlice = false;
                do
                {
                    int sliceIndex = random.Next(1, orderSlices.Length - 1);
                    if (!slicesIndexPutBack.Contains(sliceIndex))
                    {
                        for (int j = 0; j < orderSlices[sliceIndex].Count; j++)
                            if (orderSlices[sliceIndex][j].OrderNumber != 0)
                                firstAbominationRoute.AddOrder(orderSlices[sliceIndex][j]);

                        slicesIndexPutBack.Add(sliceIndex);
                        hasPutBackASlice = true;
                    }
                } while (!hasPutBackASlice);
            }

            // COMMENCE THE GENETIC EXPERIMENTS PHASE TWO!
            // OPERATION DARWIN'S ABOMINATION REPRODUCTION IS NOW OPERATIONAL!

            int numOfGenerationsToMake = 64;
            Route bestBoyAbominationOffspring = originalRoute;
            Route bestGirlAbominationOffspring = firstAbominationRoute;
            Boolean boyAbominationIsTheBest = bestBoyAbominationOffspring.TravelTime >= bestGirlAbominationOffspring.TravelTime;
            int numberOfGenesThatAreTransferred = originalRoute.Orders.Count / 2;
            List<Order> ordersSelectedForGeneticSwap = new List<Order>();

            while (numOfGenerationsToMake > 0)
            {
                ordersSelectedForGeneticSwap.Clear();

                int orderIndex = -1;
                do
                {
                    orderIndex = random.Next(orderSlices.Length - 1);
                    Order randomOrder = null;

                    if (boyAbominationIsTheBest)
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

                Route newBoyAbominationOffspring = new Route(originalRoute.Day);
                Route newGirlAbominationOffspring = new Route(originalRoute.Day);

                for (int index = 0; index < originalRoute.Orders.Count - 1; index++)
                {
                    if (indicesToBeSwaped.Contains(index))
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

                if (newBoyAbominationOffspring.isValid() && !newGirlAbominationOffspring.isValid()) // One is allowed
                    if (bestBoyAbominationOffspring.TravelTime < bestGirlAbominationOffspring.TravelTime)
                        bestGirlAbominationOffspring = newBoyAbominationOffspring;
                    else
                        bestBoyAbominationOffspring = newBoyAbominationOffspring;
                else if (!newBoyAbominationOffspring.isValid() && newGirlAbominationOffspring.isValid()) // Other one allowed
                    if (bestBoyAbominationOffspring.TravelTime < bestGirlAbominationOffspring.TravelTime)
                        bestGirlAbominationOffspring = newGirlAbominationOffspring;
                    else
                        bestBoyAbominationOffspring = newGirlAbominationOffspring;
                else if (!newBoyAbominationOffspring.isValid() && !newGirlAbominationOffspring.isValid()) // Neither is allowed
                {
                    numOfGenerationsToMake--;
                    continue;
                }

                bool newGirlAbominationOffspringIstheBest = newBoyAbominationOffspring.TravelTime <= newGirlAbominationOffspring.TravelTime;
                if (boyAbominationIsTheBest)
                {
                    if (newGirlAbominationOffspringIstheBest)
                    {
                        if (bestGirlAbominationOffspring.TravelTime < newGirlAbominationOffspring.TravelTime)
                        {
                            bestGirlAbominationOffspring = newGirlAbominationOffspring;

                            if (bestBoyAbominationOffspring.TravelTime < newBoyAbominationOffspring.TravelTime)
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

            bestAbominationOffspringRoute = boyAbominationIsTheBest ? bestBoyAbominationOffspring : bestGirlAbominationOffspring;

            Console.WriteLine("Original Travel Time:                     {0}", originalTravelTime); // These three lines are temporarily here
            Console.WriteLine("The First Abomination Travel Time:        {0}", firstAbominationRoute.TravelTime);
            Console.WriteLine("Best Abomination Offspring Travel Time:   {0}", bestAbominationOffspringRoute.TravelTime);

            toStartFrom.RemoveRouteFromPlanning(planningForSelectedRoute.Item1, planningForSelectedRoute.Item2, originalRoute);
            toStartFrom.AddRouteToPlanning(planningForSelectedRoute.Item1, planningForSelectedRoute.Item2, bestAbominationOffspringRoute);

            return toStartFrom;
        }

        public override Solution undoStrategy(Solution toStartFrom)
        {
            // Revert abomination to Patient Zero...
            toStartFrom.RemoveRouteFromPlanning(planningForSelectedRoute.Item1, planningForSelectedRoute.Item2, bestAbominationOffspringRoute);
            toStartFrom.AddRouteToPlanning(planningForSelectedRoute.Item1, planningForSelectedRoute.Item2, originalRoute);

            return toStartFrom;
        }
    }
}