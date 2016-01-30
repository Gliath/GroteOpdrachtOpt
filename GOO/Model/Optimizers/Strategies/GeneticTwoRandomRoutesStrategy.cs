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
            secondOriginalRoute = planningForSelectedRoute.Item3[random.Next(planningForSelectedRoute.Item3.Count)];

            for (int index = 0; index < 5 && firstOriginalRoute != secondOriginalRoute; index++)
                secondOriginalRoute = planningForSelectedRoute.Item3[random.Next(planningForSelectedRoute.Item3.Count)];

            double firstOriginalTravelTime = firstOriginalRoute.TravelTime;
            double secondOriginalTravelTime = secondOriginalRoute.TravelTime;
            int[] numOfSlices = new int[2] {random.Next(1, firstOriginalRoute.Orders.Count - 2), random.Next(1, secondOriginalRoute.Orders.Count - 2)};
            List<int>[] sliceIndices = new List<int>[2];

            for (int i = 0; i < sliceIndices.Length; i++)
            {
                sliceIndices[i] = new List<int>();
                for (int j = 0; j < numOfSlices[i]; j++)
                {
                    bool hasAddedAnIndex = false;
                    do
                    {
                        int sliceIndex = random.Next(1, numOfSlices[i]);
                        if (!sliceIndices[i].Contains(sliceIndex))
                        {
                            sliceIndices[i].Add(sliceIndex);
                            hasAddedAnIndex = true;
                        }
                    } while (!hasAddedAnIndex);
                }

                sliceIndices[i].Sort();
            }

            // COMMENCE THE GENETIC EXPERIMENTS!
            // OPERATION SLICE & SPLICE AN ABOMINATION IS ACTIVE!

            List<Order>[][] orderSlices = new List<Order>[sliceIndices.Length][];
            for (int index = 0; index < sliceIndices.Length; index++)
            {
                orderSlices[index] = new List<Order>[sliceIndices[index].Count];
                for (int i = 0; i < sliceIndices[index].Count; i++)
                {
                    orderSlices[index][i] = new List<Order>();
                    int startIndex = i == 0 ? 0 : sliceIndices[index][i - 1];

                    for (int j = startIndex; j < sliceIndices[index][i]; j++)
                        if(index == 0)
                            orderSlices[0][i].Add(firstOriginalRoute.Orders[j]);
                        else
                            orderSlices[1][i].Add(secondOriginalRoute.Orders[j]);
                }
            }

            // Now that you've sliced up the orders in the selected route, it is time for randomizing the slices and putting it back together

            firstAbominationRoute = new Route(planningForSelectedRoute.Item1);
            secondAbominationRoute = new Route(planningForSelectedRoute.Item1);
            List<int>[] slicesIndexPutBack = new List<int>[2];
            for (int index = 0; index < sliceIndices.Length; index++)
            {
                slicesIndexPutBack[index] = new List<int>();
                for (int i = 0; i < orderSlices[index].Length - 2; i++)
                {
                    bool hasPutBackASlice = false;
                    do
                    {
                        int sliceIndex = random.Next(1, orderSlices[index].Length - 1);
                        if (!slicesIndexPutBack[index].Contains(sliceIndex))
                        {
                            for (int j = 0; j < orderSlices[index][sliceIndex].Count; j++)
                                if (orderSlices[index][sliceIndex][j].OrderNumber != 0)

                                    if (index == 0)
                                        firstAbominationRoute.AddOrder(orderSlices[index][sliceIndex][j]);
                                    else
                                        secondAbominationRoute.AddOrder(orderSlices[index][sliceIndex][j]);

                            slicesIndexPutBack[index].Add(sliceIndex);
                            hasPutBackASlice = true;
                        }
                    } while (!hasPutBackASlice);
                }
            }

            // COMMENCE THE GENETIC EXPERIMENTS PHASE TWO!
            // OPERATION DARWIN'S ABOMINATION REPRODUCTION IS NOW OPERATIONAL!

            int numOfGenerationsToMake = 64;
            Route bestBoyAbominationOffspring = firstAbominationRoute;
            Route bestGirlAbominationOffspring = secondAbominationRoute;
            int[] numberOfGenesThatAreTransferred = new int[2] { random.Next(firstAbominationRoute.Orders.Count / 4, // 25%
                                                                    firstAbominationRoute.Orders.Count / 2), // 50%
                                                                random.Next(secondAbominationRoute.Orders.Count / 4, // 25%
                                                                    secondAbominationRoute.Orders.Count / 2)}; // 50%

            List<Order>[] ordersSelectedForGeneticSwap = new List<Order>[2] { new List<Order>(), new List<Order>() };

            while (numOfGenerationsToMake > 0)
            {
                for (int i = 0; i < 2; i++)
                {
                    ordersSelectedForGeneticSwap[i].Clear();

                    int orderIndex = -1;
                    do
                    {
                        orderIndex = random.Next(orderSlices[i].Length - 1);
                        Order randomOrder = null;

                        if (i == 0)
                            randomOrder = bestBoyAbominationOffspring.Orders[orderIndex];
                        else
                            randomOrder = bestGirlAbominationOffspring.Orders[orderIndex];

                        if (ordersSelectedForGeneticSwap[i].Contains(randomOrder))
                            continue;

                        ordersSelectedForGeneticSwap[i].Add(randomOrder);

                    } while (ordersSelectedForGeneticSwap[i].Count * 2 < numberOfGenesThatAreTransferred[i] - 1);

                    List<int> indicesToBeSwaped = new List<int>(); // FIX MEE
                    foreach (Order order in ordersSelectedForGeneticSwap[i]) // TODO, not (exclusive) simularities, but just 'random' orders
                    {
                        indicesToBeSwaped.Add(bestBoyAbominationOffspring.Orders.FindIndex(o => o == order));
                        indicesToBeSwaped.Add(bestGirlAbominationOffspring.Orders.FindIndex(o => o == order));
                    }
                    indicesToBeSwaped.Sort();

                Route newBoyAbominationOffspring = new Route(planningForSelectedRoute.Item1);
                Route newGirlAbominationOffspring = new Route(planningForSelectedRoute.Item1);

                for (int index = 0; index < firstOriginalRoute.Orders.Count - 1; index++) // FIX MEE TOOO!!
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
                if (true) // if (boyAbominationIsTheBest) Find alternative!
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
                }

                numOfGenerationsToMake--;
            }

            // PHASE 3 - INSERT THE ABOMINATION INTO THE PUBLIC!

            firstBestAbominationOffspringRoute = bestBoyAbominationOffspring;
            secondBestAbominationOffspringRoute = bestGirlAbominationOffspring;
            // secondBest?

            Console.WriteLine("First Route: Original Travel Time:                     {0}", firstOriginalTravelTime); // NEEDS FIXING
            Console.WriteLine("First Route: The First Abomination Travel Time:        {0}", firstAbominationRoute.TravelTime); // NEEDS FIXING
            Console.WriteLine("First Route: Best Abomination Offspring Travel Time:   {0}", firstBestAbominationOffspringRoute.TravelTime); // NEEDS FIXING

            Console.WriteLine("Second Route: Original Travel Time:                     {0}", secondOriginalTravelTime); // NEEDS FIXING
            Console.WriteLine("Second Route: The First Abomination Travel Time:        {0}", secondAbominationRoute.TravelTime); // NEEDS FIXING
            Console.WriteLine("Second Route: Best Abomination Offspring Travel Time:   {0}", secondBestAbominationOffspringRoute.TravelTime); // NEEDS FIXING

            planningForSelectedRoute.Item3.Remove(firstOriginalRoute); // NEEDS FIXING?
            planningForSelectedRoute.Item3.Add(firstBestAbominationOffspringRoute); // NEEDS FIXING?
            planningForSelectedRoute.Item3.Remove(secondOriginalRoute); // NEEDS FIXING?
            planningForSelectedRoute.Item3.Add(secondBestAbominationOffspringRoute); // NEEDS FIXING?

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