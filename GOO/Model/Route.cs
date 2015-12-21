using System;
using System.Collections.Generic;

using GOO.Utilities;

namespace GOO.Model
{
    public class Route
    {
        public List<Order> Orders { get; private set; }
        public double TravelTime { get; private set; }
        public Days Day { get; private set; }
        public int Weight { get; private set; }

        public Route(Days Day)
        {
            Orders = new List<Order>();
            TravelTime = 1800.0d;
            Weight = 0;
            this.Day = Day;

            Orders.Add(FilesInitializer.GetOrder0());
        }

        public bool CanAddOrder(Order order)
        { // TODO: ?
            return true;
        }

        public void AddOrder(Order order)
        {
            int LastMatrixID = 287;
            int NewMatrixID = order.MatrixID;
            int PreviousMatrixID = Orders.Count == 1 ? LastMatrixID : Orders[Orders.Count - 2].MatrixID;

            TravelTime -= FilesInitializer._DistanceMatrix.Matrix[PreviousMatrixID, LastMatrixID].TravelTime;
            TravelTime += FilesInitializer._DistanceMatrix.Matrix[PreviousMatrixID, NewMatrixID].TravelTime;
            TravelTime += FilesInitializer._DistanceMatrix.Matrix[NewMatrixID, LastMatrixID].TravelTime;

            TravelTime += order.EmptyingTimeInSeconds;
            Weight += order.VolumePerContainer * order.NumberOfContainers;
            Orders.Insert(Orders.Count - 1, order);
        }

        public void AddOrderAt(Order newOrder, Order orderToInsertAfter)
        {
            int NextMatrixID = 287;
            int NewMatrixID = newOrder.MatrixID;
            int PreviousMatrixID = orderToInsertAfter.MatrixID;
            int IndexOfMatrixToInsertAfter = Orders.FindIndex(o => o.MatrixID == PreviousMatrixID);

            if (IndexOfMatrixToInsertAfter != Orders.Count - 2)
                NextMatrixID = Orders[IndexOfMatrixToInsertAfter + 1].MatrixID;

            TravelTime -= FilesInitializer._DistanceMatrix.Matrix[PreviousMatrixID, NextMatrixID].TravelTime;
            TravelTime += FilesInitializer._DistanceMatrix.Matrix[PreviousMatrixID, NewMatrixID].TravelTime;
            TravelTime += FilesInitializer._DistanceMatrix.Matrix[NewMatrixID, NextMatrixID].TravelTime;

            TravelTime += newOrder.EmptyingTimeInSeconds;
            Weight += newOrder.VolumePerContainer * newOrder.NumberOfContainers;
            Orders.Insert(IndexOfMatrixToInsertAfter + 1, newOrder);
        }

        public void RemoveOrder(Order order)
        {
            int OldMatrixID = order.MatrixID;
            int indexOfOldOrder = Orders.FindIndex(o => o.MatrixID == OldMatrixID);
            int PreviousMatrixID = (indexOfOldOrder - 1) > 0 ? Orders[indexOfOldOrder - 1].MatrixID : 287;
            int NextMatrixID = indexOfOldOrder < Orders.Count - 2 ? Orders[indexOfOldOrder + 1].MatrixID : 287;

            TravelTime -= FilesInitializer._DistanceMatrix.Matrix[OldMatrixID, NextMatrixID].TravelTime;
            TravelTime -= FilesInitializer._DistanceMatrix.Matrix[PreviousMatrixID, OldMatrixID].TravelTime;
            TravelTime += FilesInitializer._DistanceMatrix.Matrix[PreviousMatrixID, NextMatrixID].TravelTime;

            TravelTime -= order.EmptyingTimeInSeconds;
            Weight -= order.VolumePerContainer * order.NumberOfContainers;
            Orders.Remove(order);
        }
    }
}