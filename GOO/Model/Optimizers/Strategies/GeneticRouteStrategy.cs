using System;
using System.Collections.Generic;

using GOO.Utilities;

namespace GOO.Model.Optimizers.Strategies
{
    public class GeneticRouteStrategy : Strategy
    {
        private Route originalRoute;
        private Route newAbominationRoute;
        private Tuple<Days, int, List<Route>> planningForSelectedRoute;
        private Random random;

        public GeneticRouteStrategy()
            : base()
        {
            random = new Random();
        }

        public override Solution executeStrategy(Solution toStartFrom)
        {
            Cluster targetCluster = null;
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
                return null; // Just in case something weird happens

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
            sliceIndices.Sort(); // So that the indices are going for low to high

            // COMMENCE THE GENETIC EXPERIMENTS! 
            // OPERATION CUT & SPLICE AN ABOMINATION IS ACTIVE!

            List<Order>[] orderSlices = new List<Order>[sliceIndices.Count];
            for (int i = 0; i < sliceIndices.Count; i++)
            {
                orderSlices[i] = new List<Order>();
                int startIndex = i == 0 ? 0 : sliceIndices[i - 1]; // Slices on 0 twice, on 0 and 1 - 1

                for (int j = startIndex; j < sliceIndices[i]; j++)
                    orderSlices[i].Add(originalRoute.Orders[j]);
            }

            // No that you've sliced up the orders in the selected route, it is time for randomizing the slices and putting it back together

            newAbominationRoute = new Route(originalRoute.Day);
            List<int> slicesIndexPutBack = new List<int>();
            for (int i = 0; i < orderSlices.Length; i++)
            {
                bool hasPutBackASlice = false;
                do
                {
                    int sliceIndex = random.Next(1, orderSlices.Length); // And index 0?
                    if (!slicesIndexPutBack.Contains(sliceIndex))
                    {
                        for (int j = 0; j < orderSlices[sliceIndex].Count; j++)
                            if (orderSlices[sliceIndex][j].OrderNumber != 0)
                                newAbominationRoute.AddOrder(orderSlices[sliceIndex][j]);

                        slicesIndexPutBack.Add(sliceIndex);
                        hasPutBackASlice = true;
                    }
                } while (!hasPutBackASlice);
            }

            Console.WriteLine("Original Travel Time : {0}", originalTravelTime);
            Console.WriteLine("Abomination Travel Time : {0}", newAbominationRoute.TravelTime);

            planningForSelectedRoute = toStartFrom.getRoute(targetCluster.Routes);

            if (planningForSelectedRoute.Item1 != originalRoute.Day)
                Console.WriteLine("ERROR, found the wrong route!");

            planningForSelectedRoute.Item3.Remove(originalRoute);
            planningForSelectedRoute.Item3.Add(newAbominationRoute);

            toStartFrom.RemoveItemFromPlanning(planningForSelectedRoute.Item1, planningForSelectedRoute.Item2);
            toStartFrom.AddNewItemToPlanning(planningForSelectedRoute.Item1, planningForSelectedRoute.Item2, planningForSelectedRoute.Item3);
            // Where is the generation of the population and then the selection based on fitness?
            return toStartFrom;
        }

        public override Solution undoStrategy(Solution toStartFrom)
        {
            // Revert abomination to Patient Zero
            planningForSelectedRoute.Item3.Remove(newAbominationRoute);
            planningForSelectedRoute.Item3.Add(originalRoute);

            toStartFrom.RemoveItemFromPlanning(planningForSelectedRoute.Item1, planningForSelectedRoute.Item2);
            toStartFrom.AddNewItemToPlanning(planningForSelectedRoute.Item1, planningForSelectedRoute.Item2, planningForSelectedRoute.Item3);

            return toStartFrom;
        }
    }
}