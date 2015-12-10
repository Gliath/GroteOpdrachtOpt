using GOO.Utilities;

using System;

namespace GOO.Model
{
    public class Order
    {
        public int OrderNumber { get; private set; }
        public String Place { get; private set; }
        public OrderFrequency Frequency { get; private set; }
        public int NumberOfContainers { get; private set; }
        public int VolumePerContainer { get; private set; }
        public float EmptyingTimeInMinutes { get; private set; }
        public int MatrixID { get; private set; }
        public int X { get; private set; }
        public int Y { get; private set; }

        public Order(int OrderNumber, String Place, OrderFrequency Frequency, int NumberOfContainers, int VolumePerContainer, float EmptyingTimeInMinutes, int MatrixID, int X, int Y)
        {
            this.OrderNumber = OrderNumber;
            this.Place = Place;
            this.Frequency = Frequency;
            this.NumberOfContainers = NumberOfContainers;
            this.VolumePerContainer = VolumePerContainer;
            this.EmptyingTimeInMinutes = EmptyingTimeInMinutes;
            this.MatrixID = MatrixID;
            this.X = X;
            this.Y = Y; 
        }
    }
}