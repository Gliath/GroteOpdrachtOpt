using System;
using System.Collections.Generic;

using GOO.Utilities;

namespace GOO.Model
{
    public class Solver
    {
        private static Clusterer clusterer;
        private static List<Cluster> clusters;

        public static Solution generateSolution()
        {
            System.Diagnostics.Stopwatch sw = System.Diagnostics.Stopwatch.StartNew();
            for (int i = 10; i <= 100; i += 5)
            {
                sw.Restart();
                clusterer = new Clusterer(FilesInitializer._Orders, i);
                clusters = clusterer.createKClusters();

                int numOfEmptyClusters = 0;
                for (int k = 0; k < clusters.Count; k++)
                {
                    if (clusters[k].OrdersInCluster.Count == 0)
                        numOfEmptyClusters++;
                }
                sw.Stop();

                Console.WriteLine("Generated a k-cluster solution with: {0} clusters with {1} not empty ones.", i, clusters.Count);
                //Console.WriteLine("This solution had {0} empty clusters.", numOfEmptyClusters);
                Console.WriteLine("It took {0}ms to generate this solution.", sw.ElapsedMilliseconds);

            }

            foreach (Cluster cluster in clusters)
                Console.WriteLine(cluster);

            return new Solution(clusters);
        }

        public static string generateRouteSolution()
        {
            List<System.Text.StringBuilder> sbList = new List<System.Text.StringBuilder>();
            System.Text.StringBuilder sb = null;


            int truckNr = 1;
            int dayNr = 1;

            foreach (Cluster cluster in clusters)
            {
                if (sb == null)
                    sb = new System.Text.StringBuilder();

                sb.Append(cluster.ToRouteString(truckNr++, dayNr));

                if (truckNr > 2)
                {
                    truckNr = 1;
                    dayNr++;
                }
                if (dayNr > 5)
                {
                    dayNr = 1;

                    sbList.Add(sb);
                    sb = null;
                }
            }

            sb = new System.Text.StringBuilder();
            for (int i = 0; i < sbList.Count; i++)
            {
                string solution = sbList[i].ToString();
                System.IO.File.WriteAllText(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + "/ClusterSolution" + (i + 1) + ".txt", solution);
                sb.AppendLine(solution);
            }

            return sb.ToString();
        }
    }
}