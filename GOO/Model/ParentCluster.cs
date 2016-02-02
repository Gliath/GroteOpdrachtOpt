using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

using GOO.Utilities;

namespace GOO.Model
{
    public class ParentCluster : AbstractCluster
    {
        public int NumberOfQuadrants { get { return Quadrants.Length; } }
        public Cluster[] Quadrants { get; private set; }

        public Point CentroidPoint { get; private set; }
        public override List<Order> AvailableOrdersInCluster { get; set; }
        public override Days DaysPlannedFor { get; set; } // available days

        public Tuple<Days, Days, Days>[] PerQuadrantDaysInformation { get; set; }
        // First Days: Initial Days available
        // Second Days: Current Days available
        // Third Days: Selected Day

        public override List<Route> Routes { get { return null; } set { return; } } // Not implemented in parent cluster

        public ParentCluster(Point CentroidPoint, List<Order> OrdersInCluster, Cluster[] Quadrants)
        {
            this.CentroidPoint = CentroidPoint;
            this.AvailableOrdersInCluster = OrdersInCluster;
            this.Quadrants = Quadrants;
            PerQuadrantDaysInformation = new Tuple<Days, Days, Days>[NumberOfQuadrants];

            for (int QuadrantIndex = 0; QuadrantIndex < NumberOfQuadrants; QuadrantIndex++)
			    PerQuadrantDaysInformation[QuadrantIndex] = new Tuple<Days,Days,Days>(Quadrants[QuadrantIndex].initialRestrictions, Quadrants[QuadrantIndex].initialRestrictions, Days.None);
        }

        public bool CanSetDaysPlanned(Cluster Quadrant, Days DayPlanned)
        {
            for (int QuadrantIndex = 0; QuadrantIndex < NumberOfQuadrants; QuadrantIndex++)
                if (Quadrant == Quadrants[QuadrantIndex])
                    return CanSetDaysPlanned(QuadrantIndex, DayPlanned);

            return false;
        }

        public bool CanSetDaysPlanned(int QuadrantIndex, Days DayPlanned)
        {
            return PerQuadrantDaysInformation[QuadrantIndex].Item2.HasFlag(DayPlanned);
        }

        public bool SetDaysPlannedForQuadrant(Cluster Quadrant, Days DayPlanned)
        {
            for (int QuadrantIndex = 0; QuadrantIndex < NumberOfQuadrants; QuadrantIndex++)
                if (Quadrant == Quadrants[QuadrantIndex])
                {
                    if (!CanSetDaysPlanned(QuadrantIndex, DayPlanned) || PerQuadrantDaysInformation[QuadrantIndex].Item3 != Days.None)
                        return false;

                    PerQuadrantDaysInformation[QuadrantIndex] = new Tuple<Days, Days, Days>(PerQuadrantDaysInformation[QuadrantIndex].Item1, Days.None, DayPlanned);
                    Quadrants[QuadrantIndex].DaysPlannedFor = DayPlanned;

                    for (int EachQuadrantIndex = 0; EachQuadrantIndex < NumberOfQuadrants; EachQuadrantIndex++)
                    {
                        if (EachQuadrantIndex == QuadrantIndex || PerQuadrantDaysInformation[EachQuadrantIndex].Item3 != Days.None)
                            continue;

                        if (PerQuadrantDaysInformation[EachQuadrantIndex].Item2.HasFlag(DayPlanned))
                        {
                            Days QuadrantRestriction = PerQuadrantDaysInformation[EachQuadrantIndex].Item2;

                            QuadrantRestriction ^= DayPlanned;
                            PerQuadrantDaysInformation[EachQuadrantIndex] = new Tuple<Days, Days, Days>(PerQuadrantDaysInformation[EachQuadrantIndex].Item1, QuadrantRestriction, Days.None);

                            if (QuadrantRestriction != Days.None && ((int)QuadrantRestriction & ((int)QuadrantRestriction - 1)) == 0)
                                SetDaysPlannedForQuadrant(Quadrants[EachQuadrantIndex], QuadrantRestriction);
                        }
                    }

                    break;
                }

            return true;
        }

        public bool RemoveDaysPlannedForQuadrant(int QuadrantNumber)
        {
            for (int QuadrantIndex = 0; QuadrantIndex < NumberOfQuadrants; QuadrantIndex++)
                if (QuadrantNumber == QuadrantIndex)
                {
                    if (PerQuadrantDaysInformation[QuadrantIndex].Item3 == Days.None)
                        return false;

                    Days DayItWasPlannedFor = PerQuadrantDaysInformation[QuadrantIndex].Item3;
                    Days DaysRestriction = PerQuadrantDaysInformation[QuadrantIndex].Item1;

                    for (int EachQuadrantIndex = 0; EachQuadrantIndex < NumberOfQuadrants; EachQuadrantIndex++)
                    {
                        if (EachQuadrantIndex == QuadrantIndex || PerQuadrantDaysInformation[EachQuadrantIndex].Item3 == Days.None)
                            continue;

                        DaysRestriction ^= PerQuadrantDaysInformation[EachQuadrantIndex].Item3;
                    }

                    PerQuadrantDaysInformation[QuadrantIndex] = new Tuple<Days, Days, Days>(PerQuadrantDaysInformation[QuadrantIndex].Item1, DaysRestriction, Days.None);
                    Quadrants[QuadrantIndex].DaysPlannedFor = Days.None;

                    for (int EachQuadrantIndex = 0; EachQuadrantIndex < NumberOfQuadrants; EachQuadrantIndex++)
                    {
                        if (EachQuadrantIndex == QuadrantIndex || PerQuadrantDaysInformation[EachQuadrantIndex].Item3 != Days.None)
                            continue;

                        if (PerQuadrantDaysInformation[EachQuadrantIndex].Item1.HasFlag(DayItWasPlannedFor))
                        {
                            Days QuadrantRestriction = PerQuadrantDaysInformation[EachQuadrantIndex].Item2;
                            QuadrantRestriction |= DayItWasPlannedFor;
                            PerQuadrantDaysInformation[EachQuadrantIndex] = new Tuple<Days, Days, Days>(PerQuadrantDaysInformation[EachQuadrantIndex].Item1, QuadrantRestriction, Days.None);
                        }
                    }

                    break;
                }

            return true;
        }

        public override string ToString()
        {
            System.Text.StringBuilder builder = new System.Text.StringBuilder();

            builder.AppendLine(String.Format("Parent Cluster:"));
            builder.AppendLine(String.Format("Days planned: {0}", DaysPlannedFor));
            builder.AppendLine(String.Format("Centroid Point: {0}", CentroidPoint.ToString()));
            builder.AppendLine(String.Format("Number of Quadrants: {0}", NumberOfQuadrants));
            builder.AppendLine(String.Format("Number of Orders: {0}", AvailableOrdersInCluster.Count));
            for (int i = 0; i < Quadrants.Length; i++)
                builder.AppendLine(String.Format("Quadrant {0} has {1} orders", i, Quadrants[i].AvailableOrdersInCluster.Count));

            builder.AppendLine(String.Format(""));
            return builder.ToString();
        }
    }
}