using System;
using System.Collections.Generic;

namespace GOO.Model.Optimizers.Strategies
{
    public class DivorceAttourneyStrategy
    {
        private List<AbstractCluster> OriginalClusters;

        public DivorceAttourneyStrategy()
        {
            OriginalClusters = new List<AbstractCluster>();
        }

        public List<AbstractCluster> executeStrategy(List<AbstractCluster> clustersToDivorce)
        {
            // Randomly divorce clusters or divorce them because it can make filled clusters?


            return clustersToDivorce;
        }

        public List<AbstractCluster> undoStrategy(List<AbstractCluster> clustersToRemarry)
        {


            return clustersToRemarry;
        }
    }
}