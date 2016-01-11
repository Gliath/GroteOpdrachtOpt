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

            Orders.Add(Data.GetOrder0());
        }

        public bool CanAddOrder(Order order)
        { // TODO: ? -> Only check if order is already on this route, better check from solution
            return true;
        }

        public void AddOrder(Order order)
        {
            int LastMatrixID = 287;
            int NewMatrixID = order.MatrixID;
            int PreviousMatrixID = Orders.Count == 1 ? LastMatrixID : Orders[Orders.Count - 2].MatrixID;

            TravelTime -= Data.DistanceMatrix[PreviousMatrixID, LastMatrixID].TravelTime;
            TravelTime += Data.DistanceMatrix[PreviousMatrixID, NewMatrixID].TravelTime;
            TravelTime += Data.DistanceMatrix[NewMatrixID, LastMatrixID].TravelTime;

            TravelTime += order.EmptyingTimeInSeconds;
            Weight += order.VolumePerContainer * order.NumberOfContainers;
            Orders.Insert(Orders.Count - 1, order);
        }

        public void AddOrderAt(Order newOrder, Order orderToInsertAfter)
        {
            int IndexOfOrderToInsertAfter = Orders.FindIndex(o => o.OrderNumber == orderToInsertAfter.OrderNumber);

            if (IndexOfOrderToInsertAfter == -1) // Could not find the order or insert the new order after the 0 order
                return;

            if (IndexOfOrderToInsertAfter >= Orders.Count - 2)
            {
                AddOrder(newOrder);
                return;
            }

            int NextMatrixID = Orders[IndexOfOrderToInsertAfter + 1].MatrixID;
            int NewMatrixID = newOrder.MatrixID;
            int PreviousMatrixID = orderToInsertAfter.MatrixID;

            TravelTime -= Data.DistanceMatrix[PreviousMatrixID, NextMatrixID].TravelTime;
            TravelTime += Data.DistanceMatrix[PreviousMatrixID, NewMatrixID].TravelTime;
            TravelTime += Data.DistanceMatrix[NewMatrixID, NextMatrixID].TravelTime;

            TravelTime += newOrder.EmptyingTimeInSeconds;
            Weight += newOrder.VolumePerContainer * newOrder.NumberOfContainers;
            Orders.Insert(IndexOfOrderToInsertAfter + 1, newOrder);
        }

        public void RemoveOrder(Order order)
        {
            int OldMatrixID = order.MatrixID;
            int indexOfOldOrder = Orders.FindIndex(o => o.OrderNumber == order.OrderNumber);
            int PreviousMatrixID = (indexOfOldOrder - 1) >= 0 ? Orders[indexOfOldOrder - 1].MatrixID : 287;
            int NextMatrixID = indexOfOldOrder < Orders.Count - 2 ? Orders[indexOfOldOrder + 1].MatrixID : 287;

            TravelTime -= Data.DistanceMatrix[OldMatrixID, NextMatrixID].TravelTime;
            TravelTime -= Data.DistanceMatrix[PreviousMatrixID, OldMatrixID].TravelTime;
            TravelTime += Data.DistanceMatrix[PreviousMatrixID, NextMatrixID].TravelTime;

            TravelTime -= order.EmptyingTimeInSeconds;
            Weight -= order.VolumePerContainer * order.NumberOfContainers;
            Orders.Remove(order);
        }

        public void SwapOrders(Order A, Order B)
        {
            int indexOfA = Orders.FindIndex(o => o.OrderNumber == A.OrderNumber);
            int indexOfB = Orders.FindIndex(o => o.OrderNumber == B.OrderNumber);

            if (indexOfA == -1 || indexOfB == -1)
                return;

            if (indexOfA > indexOfB)
            {
                AddOrderAt(B, A);
                RemoveOrder(A);
                AddOrderAt(A, B);
                RemoveOrder(B);
            }
            else
            {
                AddOrderAt(A, B);
                RemoveOrder(B);
                AddOrderAt(B, A);
                RemoveOrder(A);
            }
        }
    }
}