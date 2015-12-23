using System;
using System.Collections.Generic;

using GOO.Utilities;

namespace GOO.Model
{
    public class MarriedCluster : AbstractCluster
    {
        public Cluster[] Harem { get; private set; }

        public MarriedCluster(Cluster[] Harem)
        {
            this.Harem = Harem;
        }

        public override List<Order> OrdersInCluster
        {
            get
            {
                List<Order> UnifiedOrdersInCluster = new List<Order>();

                foreach (Cluster Concubine in Harem)
                    UnifiedOrdersInCluster.AddRange(Concubine.OrdersInCluster);

                return UnifiedOrdersInCluster;
            }

            set { return; }
        }

        public override Days DaysPlannedFor
        {
            get
            {
                return Harem[0].DaysPlannedFor; // Every member of the Harem is planned for the same day
            }

            set { return; }
        }

        public override List<Route> Routes
        {
            get
            {
                List<Route> UnifiedRoutes = new List<Route>();

                foreach (Cluster Concubine in Harem)
                    UnifiedRoutes.AddRange(Concubine.Routes);

                return UnifiedRoutes;
            }

            set { return; }
        }

        public override string ToString()
        {
            System.Text.StringBuilder builder = new System.Text.StringBuilder();

            builder.AppendLine(String.Format("Married Cluster:"));
            builder.AppendLine(String.Format("Days planned: {0}", DaysPlannedFor));
            builder.AppendLine(String.Format("Number of Clusters in this marriage: {0}", Harem.Length));
            builder.AppendLine(String.Format("Number of Orders: {0}", OrdersInCluster.Count));
            builder.AppendLine(String.Format("Number of Routes: {0}", Routes.Count));
            for (int i = 0; i < Harem.Length; i++)
                builder.AppendLine(String.Format("Cluster {0} in marriage has {1} orders", i, Harem[i].OrdersInCluster.Count));

            builder.AppendLine(String.Format(""));
            return builder.ToString();
        }
    }
}