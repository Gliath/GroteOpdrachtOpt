using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

using GOO.Utilities;

namespace GOO.Model
{
    public class MarriedCluster : AbstractCluster
    {
        public Cluster Groom { get; set; }
        public Cluster Bride { get; set; }

        public override List<Order> OrdersInCluster
        {
            get
            {
                return null;
            }

            set { return; }
        }

        public override List<Days> DaysRestrictions
        {
            get
            {
                return null;
            }

            set { return; }
        }

        public override Days DaysPlannedFor
        {
            get
            {
                return Days.None;
            }

            set { return; }
        }

        public override OrdersCounter OrdersCounter
        {
            get
            {
                return null;
            }

            set { return; }
        }

        public MarriedCluster(Cluster Groom, Cluster Bride)
        {
            this.Groom = Groom;
            this.Bride = Bride;
        }
    }
}