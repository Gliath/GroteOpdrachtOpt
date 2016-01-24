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

        private bool CanAddOrderCheck(Order order, double timeLimit)
        {
            if (order.OrderNumber == 0)
                return false;

            if (Orders.Contains(order))
                return false;

            if (Weight + (order.VolumePerContainer * order.NumberOfContainers) > 100000)
                return false;

            return order.CanBeAddedOnDay(this.Day);
        }

        public bool CanAddOrder(Order order, double timeLimit = 43200.0d)
        {
            if (CanAddOrderCheck(order, timeLimit))
            {
                double tempTT = TravelTime;
                int PreviousMatrixID = Orders.Count == 1 ? 287 : Orders[Orders.Count - 2].MatrixID;

                tempTT -= Data.DistanceMatrix[PreviousMatrixID, 287].TravelTime;
                tempTT += Data.DistanceMatrix[PreviousMatrixID, order.MatrixID].TravelTime;
                tempTT += Data.DistanceMatrix[order.MatrixID, 287].TravelTime;
                if (tempTT > timeLimit)
                    return false;

                return true;
            }   
            else
                return false;
        }

        public bool CanAddOrderAtStart(Order order, double timeLimit = 43200.0d)
        {
            if (CanAddOrderCheck(order, timeLimit))
            {
                double tempTT = TravelTime;

                tempTT -= Data.DistanceMatrix[287, Orders[1].MatrixID].TravelTime;
                tempTT += Data.DistanceMatrix[287, order.MatrixID].TravelTime;
                tempTT += Data.DistanceMatrix[order.MatrixID, Orders[1].MatrixID].TravelTime;
                if (tempTT > timeLimit)
                    return false;

                return true;
            }
            else
                return false;
        }

        public bool CanAddOrderAfter(Order order, Order orderToInsertAfter, double timeLimit = 43200.0d)
        {
            if (orderToInsertAfter.OrderNumber == 0)
                return false;

            if (CanAddOrderCheck(order, timeLimit))
            {
                for (int i = 0; i < Orders.Count; i++)
                    if (Orders[i] == orderToInsertAfter)
                    {
                        double tempTT = TravelTime;
                        int PreviousMatrixID = Orders[i].MatrixID;
                        int NextMatrixID = Orders[i + 1].MatrixID;

                        tempTT -= Data.DistanceMatrix[PreviousMatrixID, NextMatrixID].TravelTime;
                        tempTT += Data.DistanceMatrix[PreviousMatrixID, order.MatrixID].TravelTime;
                        tempTT += Data.DistanceMatrix[order.MatrixID, NextMatrixID].TravelTime;
                        if (tempTT > timeLimit)
                            return false;

                        return true;
                    }

                return false;
            }
            else
                return false;
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
            order.AddedToRoute(this);
        }

        public void AddOrderAtStart(Order order)
        {
            int FirstMatrixID = 287;
            int NewMatrixID = order.MatrixID;
            int NextMatrixID = Orders[1].MatrixID;

            TravelTime -= Data.DistanceMatrix[FirstMatrixID, NextMatrixID].TravelTime;
            TravelTime += Data.DistanceMatrix[FirstMatrixID, NewMatrixID].TravelTime;
            TravelTime += Data.DistanceMatrix[NewMatrixID, NextMatrixID].TravelTime;

            TravelTime += order.EmptyingTimeInSeconds;
            Weight += order.VolumePerContainer * order.NumberOfContainers;
            Orders.Insert(1, order);
            order.AddedToRoute(this);
        }

        public void AddOrderAt(Order newOrder, Order orderToInsertAfter)
        {
            int IndexOfOrderToInsertAfter = Orders.FindIndex(o => o.OrderNumber == orderToInsertAfter.OrderNumber);

            if (IndexOfOrderToInsertAfter == -1)
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
            newOrder.AddedToRoute(this);
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
            order.RemoveFromRoute(this);
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