using System;
using System.Collections.Generic;

namespace GOO.Model.Optimizers.Strategies
{
    public class GeneticRouteStrategy : Strategy
    {
        private Route firstRouteToModify;
        private Route secondRouteToModify;
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
            int secondRouteIndex = -1;

            int numOfMaximumTriesFindingACluster = 128;
            while (targetCluster == null && numOfMaximumTriesFindingACluster > 0)
            {
                targetCluster = toStartFrom.getRandomCluster();

                if (targetCluster.Routes.Count > 1)
                {
                    List<int> visitedIndices = new List<int>();
                    int numOfMaximumTriesFindingAMatchingRoute = 256;
                    while ((firstRouteIndex == -1 || secondRouteIndex == -1) && numOfMaximumTriesFindingAMatchingRoute > 0)
                    {
                        int randomIndex = random.Next(targetCluster.Routes.Count);
                        if (targetCluster.Routes[randomIndex].Orders.Count < 3 || visitedIndices.Contains(randomIndex))
                        {
                            numOfMaximumTriesFindingAMatchingRoute--;
                            continue;
                        }

                        if (firstRouteIndex != -1)
                        {
                            firstRouteIndex = randomIndex;
                            visitedIndices.Add(randomIndex);
                        }
                        else if (targetCluster.Routes[firstRouteIndex].Day == targetCluster.Routes[randomIndex].Day)
                            secondRouteIndex = randomIndex;
                        else
                            firstRouteIndex = -1;

                        numOfMaximumTriesFindingAMatchingRoute--;
                    }
                }
                else
                    targetCluster = null;

                numOfMaximumTriesFindingACluster--;
            }

            if (targetCluster == null && numOfMaximumTriesFindingACluster == 0)
                return null; // Solution does not have (is very hard to find) 

            firstRouteToModify = targetCluster.Routes[firstRouteIndex];
            secondRouteToModify = targetCluster.Routes[secondRouteIndex];



            // if route is 1 long, don't cut just add
            // remember the correctness of orderfrequency/daysrestriction
            // Initial Cut & Splice whereby the new spliced childs are of an even length
            // Uniform Crossover for #number of times where by you transfer 2-4 (random?) routes per iteration
            // return new spliced abomination to test if it is better

            return toStartFrom;
        }

        public override Solution undoStrategy(Solution toStartFrom)
        {
            // revert abomination to patient zero
            return null;
        }
    }
}