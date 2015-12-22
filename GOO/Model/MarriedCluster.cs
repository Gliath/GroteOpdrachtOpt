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

        public override OrdersCounter OrdersCounter
        {
            get
            {
                OrdersCounter UnifiedOrdersCounter = new OrdersCounter();

                foreach (Cluster Concubine in Harem)
                    foreach (GOO.Model.OrdersCounter.OrderCounter orderCounter in Concubine.OrdersCounter.CounterList)
                        UnifiedOrdersCounter.AddOccurrence(orderCounter.OrderNumber, orderCounter.OrderDayOccurrences);

                return UnifiedOrdersCounter;
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
    }
}