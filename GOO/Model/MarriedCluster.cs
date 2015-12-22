using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

using GOO.Utilities;

namespace GOO.Model
{
    public class MarriedCluster : AbstractCluster
    {
        public Cluster Groom { get; private set; }
        public Cluster Bride { get; private set; }

        public MarriedCluster(Cluster Groom, Cluster Bride)
        {
            this.Groom = Groom;
            this.Bride = Bride;
        }

        public override List<Order> OrdersInCluster
        {
            get
            {
                List<Order> UnifiedOrdersInCluster = new List<Order>();
                UnifiedOrdersInCluster.AddRange(Groom.OrdersInCluster);
                UnifiedOrdersInCluster.AddRange(Bride.OrdersInCluster);

                return UnifiedOrdersInCluster;
            }

            set { return; }
        }

        private Days UnifiedDaysPlannedFor = Days.None;
        public override Days DaysPlannedFor
        {
            get
            {
                if (UnifiedDaysPlannedFor == Days.None)
                {
                    UnifiedDaysPlannedFor = Groom.DaysPlannedFor;
                    if (!UnifiedDaysPlannedFor.HasFlag(Bride.DaysPlannedFor))
                        UnifiedDaysPlannedFor |= Bride.DaysPlannedFor;
                }

                return UnifiedDaysPlannedFor;
            }

            set { UnifiedDaysPlannedFor = value; }
        }

        public override OrdersCounter OrdersCounter
        {
            get
            {
                OrdersCounter UnifiedOrdersCounter = new OrdersCounter();

                foreach (GOO.Model.OrdersCounter.OrderCounter orderCounter in Groom.OrdersCounter.CounterList)
                    UnifiedOrdersCounter.AddOccurrence(orderCounter.OrderNumber, orderCounter.OrderDayOccurrences);

                foreach (GOO.Model.OrdersCounter.OrderCounter orderCounter in Bride.OrdersCounter.CounterList)
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
                UnifiedRoutes.AddRange(Groom.Routes);
                UnifiedRoutes.AddRange(Bride.Routes);

                return UnifiedRoutes;
            }

            set { return; }
        }
    }
}