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
            // Let op dat de clusters die je gaat trouwen die dezelfde dagrestrictions hebben
            // Kijk of de marriage de routes kan verbeteren (of een cluster een route heeft die niet vullend is (op tijd en weight) die gevuld kan worden door een andere clusters niet vullende route)

            // If this fails to encapsulate all clusters, Las Vegas marry clusters with each other (randomly marry them)

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

        /*
            // First try to marry smart - OLD METHOD
            for (int firstClusterIndex = 0; firstClusterIndex < Clusters.Count; firstClusterIndex++)
            {
                for (int secondClusterIndex = 0; secondClusterIndex < Clusters.Count; secondClusterIndex++)
                {
                    if (firstClusterIndex == secondClusterIndex)
                        continue;

                    for (int firstQuadrantIndex = 0; firstQuadrantIndex < Clusters[firstClusterIndex].NumberOfQuadrants; firstQuadrantIndex++)
                    {
                        for (int secondQuadrantIndex = 0; secondQuadrantIndex < Clusters[secondClusterIndex].NumberOfQuadrants; secondQuadrantIndex++)
                        {
                            if (Clusters[firstClusterIndex].Quadrants[firstQuadrantIndex].DaysPlannedFor != Clusters[secondClusterIndex].Quadrants[secondQuadrantIndex].DaysPlannedFor)
                                continue; // Do not have matching DaysPlannedFor, searching for a better one

                            Days dayPlanning = Clusters[firstClusterIndex].Quadrants[firstQuadrantIndex].DaysPlannedFor;

                            for (int firstRouteIndex = 0; firstRouteIndex < Clusters[firstClusterIndex].Quadrants[firstQuadrantIndex].Routes.Count; firstRouteIndex++)
                            {
                                for (int secondRouteIndex = 0; secondRouteIndex < Clusters[secondClusterIndex].Quadrants[secondQuadrantIndex].Routes.Count; secondRouteIndex++)
                                {
                                    Route firstRoute = Clusters[firstClusterIndex].Quadrants[firstQuadrantIndex].Routes[firstRouteIndex];
                                    Route secondRoute = Clusters[secondClusterIndex].Quadrants[secondQuadrantIndex].Routes[secondRouteIndex];

                                    if (firstRoute.TravelTime + secondRoute.TravelTime <= 43200.0d && firstRoute.Weight + secondRoute.Weight <= 100000)
                                    {
                                        // Found a viable marriage! nou check if there is a harem already available
                                        bool bothAreAlreadyMarried = false;
                                        bool bothAreNotMarried = true;

                                        for (int coupleIndex = 0; coupleIndex < Couples.Count; coupleIndex++)
                                        { // Beware Clusters can only be part of one harem
                                            for (int haremIndex = 0; haremIndex < Couples[coupleIndex].Harem.Count; haremIndex++)
                                            {

                                                if (Couples[coupleIndex].Harem[haremIndex] == Clusters[firstClusterIndex].Quadrants[firstQuadrantIndex])
                                                    if (Couples[coupleIndex].Harem[haremIndex] != Clusters[secondClusterIndex].Quadrants[secondQuadrantIndex]) // The first is already in a harem, but second is not, let him join
                                                        Couples[coupleIndex].Harem.Add(Clusters[secondClusterIndex].Quadrants[secondQuadrantIndex]);
                                                    else // Both clusters are already married with each other
                                                        bothAreAlreadyMarried = true; // Too early?
                                                else
                                                    if (Couples[coupleIndex].Harem[haremIndex] == Clusters[secondClusterIndex].Quadrants[secondQuadrantIndex]) // The second is already in the harem, but second is not, let him join
                                                        Couples[coupleIndex].Harem.Add(Clusters[firstClusterIndex].Quadrants[firstQuadrantIndex]);
                                                    else // Both are not part of the harem, if they are not part of any harem, make one 
                                                        bothAreNotMarried = false; // Too early?
                                            }

                                            if (bothAreAlreadyMarried)
                                                break;
                                        }

                                        if(!bothAreAlreadyMarried && bothAreNotMarried)
                                            Couples.Add(new MarriedCluster(new List<Cluster> { Clusters[firstClusterIndex].Quadrants[firstQuadrantIndex], Clusters[secondClusterIndex].Quadrants[secondQuadrantIndex] }));
                                    }
                                }
                            }
                        }
                    }
                }
            }
        */
    }
}