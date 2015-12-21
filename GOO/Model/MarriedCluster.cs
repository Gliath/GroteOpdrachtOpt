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

        public override List<Days> DaysRestrictions
        {
            get
            {
                List<Days> UnifiedDaysRestrictions = new List<Days>();
                UnifiedDaysRestrictions.AddRange(Groom.DaysRestrictions);

                foreach (Days restriction in UnifiedDaysRestrictions)
                    if (!UnifiedDaysRestrictions.Contains(restriction))
                        UnifiedDaysRestrictions.Add(restriction);

                return UnifiedDaysRestrictions;
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
    }
}