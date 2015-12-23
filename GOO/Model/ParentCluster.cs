using System;
using System.Collections.Generic;
using System.Windows;

using GOO.Utilities;

namespace GOO.Model
{
    public class ParentCluster : AbstractCluster
    {
        public int NumberOfQuadrants { get { return Quadrants.Length; } }
        public Cluster[] Quadrants { get; private set; }

        public Point CentroidPoint { get; private set; }
        public override List<Order> OrdersInCluster { get; set; }
        public override Days DaysPlannedFor { get; set; } // available days
        public List<Days> DaysAvailable { get; set; } // available days

        public override List<Route> Routes { get { return null; } set { return; } } // Not implemented in parent cluster

        public ParentCluster(Point CentroidPoint, List<Order> OrdersInCluster, Cluster[] Quadrants)
        {
            this.CentroidPoint = CentroidPoint;
            this.OrdersInCluster = OrdersInCluster;
            this.Quadrants = Quadrants;


            DaysAvailable = new List<Days>();

            foreach (Order order in OrdersInCluster)
                foreach (Days restriction in Data.Orders[order.OrderNumber].DayRestrictions)
                    if (!DaysAvailable.Contains(restriction))
                        DaysAvailable.Add(restriction);
        }

        public bool CanSetDaysPlanned(Days DayPlanned)
        {
            foreach (Days dayAvailable in DaysAvailable)
                if (dayAvailable.HasFlag(DayPlanned))
                    return true;

            return false;
        }

        public void SetDaysPlannedForQuadrant(Days DayPlanned, int QuadrantNumber)
        {
            if (!CanSetDaysPlanned(DayPlanned))
                return;

            Quadrants[QuadrantNumber].DaysPlannedFor = DaysPlannedFor;

            for (int i = 0; i < DaysAvailable.Count; i++)
                if (!DaysAvailable[i].HasFlag(DayPlanned))
                    DaysAvailable.Remove(DaysAvailable[i]);
                else
                    DaysAvailable[i] ^= DayPlanned; // Removes the day from availability

            if (DaysAvailable.Count == 1)
            {
                int numOfUnassignedClusters = 0;
                foreach (Cluster quadrant in Quadrants)
                    if (quadrant.DaysPlannedFor == Days.None)
                        numOfUnassignedClusters++;

                if (numOfUnassignedClusters == 1)
                    foreach (Cluster quadrant in Quadrants)
                        if (quadrant.DaysPlannedFor == Days.None)
                            quadrant.DaysPlannedFor = DaysAvailable[0]; // Only one item left so assign it
            }
        }

        public override string ToString()
        {
            System.Text.StringBuilder builder = new System.Text.StringBuilder();

            builder.AppendLine(String.Format("Parent Cluster:"));
            builder.AppendLine(String.Format("Days planned: {0}", DaysPlannedFor));
            builder.AppendLine(String.Format("Centroid Point: {0}", CentroidPoint.ToString()));
            builder.AppendLine(String.Format("Number of Quadrants: {0}", NumberOfQuadrants));
            builder.AppendLine(String.Format("Number of Orders: {0}", OrdersInCluster.Count));
            for (int i = 0; i < Quadrants.Length; i++)
                builder.AppendLine(String.Format("Quadrant {0} has {1} orders", i, Quadrants[i].OrdersInCluster.Count));

            builder.AppendLine(String.Format(""));
            return builder.ToString();
        }
    }
}