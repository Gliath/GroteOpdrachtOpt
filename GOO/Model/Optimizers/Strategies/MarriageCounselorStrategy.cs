using System;
using System.Collections.Generic;

using GOO.Utilities;

namespace GOO.Model.Optimizers.Strategies
{
    public class MarriageCounselorStrategy
    {
        private List<AbstractCluster> OriginalClusters;

        public MarriageCounselorStrategy()
        {
            OriginalClusters = new List<AbstractCluster>();
        }

        public List<AbstractCluster> executeStrategy(Solution toStartFrom)
        {
            List<AbstractCluster> Couples = new List<AbstractCluster>();
            List<ParentCluster> Clusters = toStartFrom.getAllClusters();
            List<Cluster> SingleClusters = new List<Cluster>();

            OriginalClusters.Clear();
            foreach (ParentCluster cluster in Clusters)
                OriginalClusters.Add(cluster);

            foreach (ParentCluster parentCluster in Clusters)
                SingleClusters.AddRange(parentCluster.Quadrants);

            for (int singleClusterIndex = 0; singleClusterIndex < SingleClusters.Count; singleClusterIndex++)
            {
                bool isInAHaremAlready = false;
                foreach (MarriedCluster Couple in Couples)
	            {
                    if (!Couple.ContainsCluster(SingleClusters[singleClusterIndex]))
                    {
                        Couple.Harem.Add(SingleClusters[singleClusterIndex]);
                        isInAHaremAlready = true;
                    }
	            }

                if (!isInAHaremAlready)
                    Couples.Add(new MarriedCluster(new List<Cluster>() { SingleClusters[singleClusterIndex] }));
            }

            return Couples;
        }

        public List<AbstractCluster> undoStrategy(Solution toStartFrom)
        {
            return OriginalClusters;
        }
    }
}