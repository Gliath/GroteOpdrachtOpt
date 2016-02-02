using System;
using System.Collections.Generic;

using GOO.Utilities;

namespace GOO.Model
{
    public class Order
    {
        public OrderTracker OrderTracker { get; private set; }
        public String Place { get; private set; }
        public List<Days> DayRestrictions { get; private set; }
        public Cluster ClusterOrderIsLocatedIn { get; private set; }

        public OrderFrequency Frequency { get; private set; }

        public int OrderNumber { get; private set; }
        public int NumberOfContainers { get; private set; }
        public int VolumePerContainer { get; private set; }
        public int MatrixID { get; private set; }
        public int X { get; private set; }
        public int Y { get; private set; }
        public double EmptyingTimeInSeconds { get; private set; }
        public double PenaltyTime { get; private set; }

        public int FrequencyNumber
        {
            get
            {
                switch (Frequency)
                {
                    case OrderFrequency.PWK1:
                        return 1;
                    case OrderFrequency.PWK2:
                        return 2;
                    case OrderFrequency.PWK3:
                        return 3;
                    case OrderFrequency.PWK4:
                        return 4;
                    case OrderFrequency.PWK5:
                        return 5;
                }

                return 0;
            }
        }

        public Order(int OrderNumber, String Place, OrderFrequency Frequency, int NumberOfContainers, int VolumePerContainer, double EmptyingTimeInSeconds, int MatrixID, int X, int Y)
        {
            this.OrderNumber = OrderNumber;
            this.Place = Place;
            this.Frequency = Frequency;
            this.NumberOfContainers = NumberOfContainers;
            this.VolumePerContainer = VolumePerContainer;
            this.EmptyingTimeInSeconds = EmptyingTimeInSeconds;
            this.MatrixID = MatrixID;
            this.X = X;
            this.Y = Y;

            ClusterOrderIsLocatedIn = null;
            PenaltyTime = Convert.ToDouble(FrequencyNumber) * Convert.ToDouble(EmptyingTimeInSeconds) * 3.0d;
            DayRestrictions = DayRestrictionFactory.GetDayRestrictions(Frequency);
            OrderTracker = new OrderTracker(this, DayRestrictions);
        }

        public bool PutInCluster(Cluster ClusterOrderIsLocatedIn)
        {
            if (this.ClusterOrderIsLocatedIn != null)
                return false;

            this.ClusterOrderIsLocatedIn = ClusterOrderIsLocatedIn;
            return true;
        }

        public void RemoveCluster()
        {
            this.ClusterOrderIsLocatedIn = null;
        }

        public void AddOrderOccurrence(Route OccurredIn)
        {
            OrderTracker.AddOrderOccurrence(OccurredIn);
        }

        public void RemoveOrderOccurrence(Route OccurredIn)
        {
            OrderTracker.RemoveOrderOccurrence(OccurredIn);
        }

        public void AddAvailableOrderBackToCluster()
        {
            if(OrderNumber != 0)
                this.ClusterOrderIsLocatedIn.AvailableOrdersInCluster.Add(this);
        }

        public void RemoveAvailableOrderFromCluster()
        {
            if (OrderNumber != 0)
                this.ClusterOrderIsLocatedIn.AvailableOrdersInCluster.Remove(this);
        }

        public bool CanBeAddedOnDay(Days day)
        {
            return this.OrderTracker.CanAddOrder(day);
        }
    }
}