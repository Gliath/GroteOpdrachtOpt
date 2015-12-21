using System;
using System.Collections.Generic;

namespace GOO.Model.Optimizers.Strategies
{
    public class GeneticRouteStrategy : Strategy
    {
        private Route firstRouteToModify;
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

            firstRouteToModify = targetCluster.Routes[firstRouteIndex];



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