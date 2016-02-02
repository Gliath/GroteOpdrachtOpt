using System;
using System.Collections.Generic;

using GOO.Utilities;

namespace GOO.Model
{
    public class MarriedCluster : AbstractCluster
    {
        public List<Cluster> Harem { get; private set; }

        public override List<Order> AvailableOrdersInCluster
        {
            get
            {
                List<Order> UnifiedOrdersInCluster = new List<Order>();

                foreach (Cluster Concubine in Harem)
                    UnifiedOrdersInCluster.AddRange(Concubine.AvailableOrdersInCluster);

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

        public MarriedCluster(List<Cluster> Harem)
        {
            this.Harem = Harem;
        }

        public bool ContainsCluster(Cluster toCompareWith)
        {
            foreach (Cluster Concubine in Harem)
                if (Concubine == toCompareWith)
                    return true;

            return false;
        }

        public override string ToString()
        {
            System.Text.StringBuilder builder = new System.Text.StringBuilder();

            builder.AppendLine(String.Format("Married Cluster:"));
            builder.AppendLine(String.Format("Days planned: {0}", DaysPlannedFor));
            builder.AppendLine(String.Format("Number of Clusters in this marriage: {0}", Harem.Count));
            builder.AppendLine(String.Format("Number of Orders: {0}", AvailableOrdersInCluster.Count));
            builder.AppendLine(String.Format("Number of Routes: {0}", Routes.Count));
            for (int i = 0; i < Harem.Count; i++)
                builder.AppendLine(String.Format("Cluster {0} in marriage has {1} orders", i, Harem[i].AvailableOrdersInCluster.Count));

            builder.AppendLine(String.Format(""));
            return builder.ToString();
        }
    }
}