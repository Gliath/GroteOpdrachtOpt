using System;
using System.Collections.Generic;

using GOO.Utilities;

namespace GOO.Model.Optimizers.Strategies
{
    public class GeneticRouteStrategy : Strategy
    {
        private Route originalRoute;
        private Route firstAbominationRoute;
        private Route bestAbominationOffspringRoute;
        private Tuple<Days, int, List<Route>> planningForSelectedRoute;

        public GeneticRouteStrategy()
            : base()
        {
            originalRoute = null;
            firstAbominationRoute = null;
            bestAbominationOffspringRoute = null;
            planningForSelectedRoute = null;
        }

        public override Solution executeStrategy(Solution toStartFrom)
        {
            AbstractCluster targetCluster = null;
            int firstRouteIndex = -1;

            int numOfMaximumTriesFindingACluster = 32;
            while (targetCluster == null && numOfMaximumTriesFindingACluster > 0)
            {
                targetCluster = toStartFrom.getRandomCluster();

                if (targetCluster.Routes.Count > 0)
                {
                    for (int i = 0; i < targetCluster.Routes.Count; i++)
                        if (targetCluster.Routes[i].Orders.Count > 2)
                            firstRouteIndex = i; // TODO: Randomize route selection?
                }
                else
                    targetCluster = null;

                numOfMaximumTriesFindingACluster--;
            }

            if (targetCluster == null)
                return null;

            originalRoute = targetCluster.Routes[firstRouteIndex];
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
            // OPERATION CUT & SPLICE AN ABOMINATION IS ACTIVE!

            List<Order>[] orderSlices = new List<Order>[sliceIndices.Count];
            for (int i = 0; i < sliceIndices.Count; i++)
            {
                orderSlices[i] = new List<Order>();
                int startIndex = i == 0 ? 0 : sliceIndices[i - 1];

                for (int j = startIndex; j < sliceIndices[i]; j++)
                    orderSlices[i].Add(originalRoute.Orders[j]);
            }

            // Now that you've sliced up the orders in the selected route, it is time for randomizing the slices and putting it back together

            firstAbominationRoute = new Route(originalRoute.Day);
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

            bestAbominationOffspringRoute = boyAbominationIsTheBest ? bestBoyAbominationOffspring : bestGirlAbominationOffspring;

            Console.WriteLine("Original Travel Time:                     {0}", originalTravelTime);
            Console.WriteLine("The First Abomination Travel Time:        {0}", firstAbominationRoute.TravelTime);
            Console.WriteLine("Best Abomination Offspring Travel Time:   {0}", bestAbominationOffspringRoute.TravelTime);

            planningForSelectedRoute = toStartFrom.getRoute(targetCluster.Routes);

            if (planningForSelectedRoute.Item1 != originalRoute.Day)
                Console.WriteLine("ERROR, found the wrong route!");

            planningForSelectedRoute.Item3.Remove(originalRoute);
            planningForSelectedRoute.Item3.Add(bestAbominationOffspringRoute);

            toStartFrom.RemoveItemFromPlanning(planningForSelectedRoute.Item1, planningForSelectedRoute.Item2);
            toStartFrom.AddNewItemToPlanning(planningForSelectedRoute.Item1, planningForSelectedRoute.Item2, planningForSelectedRoute.Item3);

            return toStartFrom;
        }

        public override Solution undoStrategy(Solution toStartFrom)
        {
            // Revert abomination to Patient Zero...
            planningForSelectedRoute.Item3.Remove(bestAbominationOffspringRoute);
            planningForSelectedRoute.Item3.Add(originalRoute);

            toStartFrom.RemoveItemFromPlanning(planningForSelectedRoute.Item1, planningForSelectedRoute.Item2);
            toStartFrom.AddNewItemToPlanning(planningForSelectedRoute.Item1, planningForSelectedRoute.Item2, planningForSelectedRoute.Item3);

            return toStartFrom;
        }
    }
}