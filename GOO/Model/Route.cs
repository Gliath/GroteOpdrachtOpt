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

        private bool CanAddOrderCheck(Order order)
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
            if (CanAddOrderCheck(order))
            {
                double tempTT = TravelTime;
                int PreviousMatrixID = Orders.Count == 1 ? 287 : Orders[Orders.Count - 2].MatrixID;

                tempTT -= Data.DistanceMatrix[PreviousMatrixID, 287].TravelTime;
                tempTT += Data.DistanceMatrix[PreviousMatrixID, order.MatrixID].TravelTime;
                tempTT += Data.DistanceMatrix[order.MatrixID, 287].TravelTime;

                return tempTT < timeLimit;
            }
            else
                return false;
        }

        public bool CanAddOrderAtStart(Order order, double timeLimit = 43200.0d)
        {
            if (CanAddOrderCheck(order))
            {
                double tempTT = TravelTime;

                tempTT -= Data.DistanceMatrix[287, Orders[0].MatrixID].TravelTime;
                tempTT += Data.DistanceMatrix[287, order.MatrixID].TravelTime;
                tempTT += Data.DistanceMatrix[order.MatrixID, Orders[0].MatrixID].TravelTime;

                return tempTT < timeLimit;
            }
            else
                return false;
        }

        public bool CanAddOrderAfter(Order order, Order orderToInsertAfter, double timeLimit = 43200.0d)
        {
            if (orderToInsertAfter.OrderNumber == 0)
                return false;

            if (CanAddOrderCheck(order))
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

                        return tempTT < timeLimit;
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

        public bool CanSwapOrder(Order firstOrder, Order secondOrder, double timeLimit = 43200.0d)
        {
            if (firstOrder.OrderNumber == 0 || secondOrder.OrderNumber == 0)
                return false;

            int firstOrderIndex = Orders.FindIndex(o => o.OrderNumber == firstOrder.OrderNumber);
            int preFirstOrderIndex = firstOrderIndex == 0 ? 287 : firstOrderIndex - 1;
            int secondOrderIndex = Orders.FindIndex(o => o.OrderNumber == secondOrder.OrderNumber);
            int preSecondOrderIndex = secondOrderIndex == 0 ? 287 : secondOrderIndex - 1;

            int preFirstMatrixID = Orders[preFirstOrderIndex].MatrixID;
            int firstOrderMatrixID = firstOrder.MatrixID;
            int postFirstMatrixID = Orders[firstOrderIndex + 1].MatrixID;
            int preSecondMatrixID = Orders[preSecondOrderIndex].MatrixID;
            int secondOrderMatrixID = secondOrder.MatrixID;
            int postSecondMatrixID = Orders[secondOrderIndex + 1].MatrixID;

            double tempTT = TravelTime;
            tempTT -= Data.DistanceMatrix[preFirstMatrixID, firstOrderMatrixID].TravelTime;
            tempTT -= Data.DistanceMatrix[firstOrderMatrixID, postFirstMatrixID].TravelTime;
            tempTT += Data.DistanceMatrix[preFirstMatrixID, secondOrderMatrixID].TravelTime;
            tempTT += Data.DistanceMatrix[secondOrderMatrixID, postFirstMatrixID].TravelTime;

            tempTT -= Data.DistanceMatrix[preSecondMatrixID, secondOrderMatrixID].TravelTime;
            tempTT -= Data.DistanceMatrix[secondOrderMatrixID, postSecondMatrixID].TravelTime;
            tempTT += Data.DistanceMatrix[preSecondMatrixID, firstOrderMatrixID].TravelTime;
            tempTT += Data.DistanceMatrix[firstOrderMatrixID, postSecondMatrixID].TravelTime;

            return tempTT < timeLimit;
        }

        public bool CanSwapOrder(Order firstOrder, Order secondOrder, Order thirdOrder, double timeLimit = 43200.0d)
        {
            if (firstOrder.OrderNumber == 0 || secondOrder.OrderNumber == 0 || secondOrder.OrderNumber == 0)
                return false;

            int firstOrderIndex = Orders.FindIndex(o => o.OrderNumber == firstOrder.OrderNumber);
            int preFirstOrderIndex = firstOrderIndex == 0 ? 287 : firstOrderIndex - 1;
            int secondOrderIndex = Orders.FindIndex(o => o.OrderNumber == secondOrder.OrderNumber);
            int preSecondOrderIndex = secondOrderIndex == 0 ? 287 : secondOrderIndex - 1;
            int thirdOrderIndex = Orders.FindIndex(o => o.OrderNumber == thirdOrder.OrderNumber);
            int preThirdOrderIndex = thirdOrderIndex == 0 ? 287 : thirdOrderIndex - 1;

            int preFirstMatrixID = Orders[preFirstOrderIndex].MatrixID;
            int firstOrderMatrixID = firstOrder.MatrixID;
            int postFirstMatrixID = Orders[firstOrderIndex + 1].MatrixID;
            int preSecondMatrixID = Orders[preSecondOrderIndex].MatrixID;
            int secondOrderMatrixID = secondOrder.MatrixID;
            int postSecondMatrixID = Orders[secondOrderIndex + 1].MatrixID;
            int preThirdMatrixID = Orders[preThirdOrderIndex].MatrixID;
            int thirdOrderMatrixID = thirdOrder.MatrixID;
            int postThirdMatrixID = Orders[thirdOrderIndex + 1].MatrixID;

            double tempTT = TravelTime;
            tempTT -= Data.DistanceMatrix[preFirstMatrixID, firstOrderMatrixID].TravelTime;
            tempTT -= Data.DistanceMatrix[firstOrderMatrixID, postFirstMatrixID].TravelTime;
            tempTT += Data.DistanceMatrix[preFirstMatrixID, thirdOrderMatrixID].TravelTime;
            tempTT += Data.DistanceMatrix[thirdOrderMatrixID, postFirstMatrixID].TravelTime;

            tempTT -= Data.DistanceMatrix[preSecondMatrixID, secondOrderMatrixID].TravelTime;
            tempTT -= Data.DistanceMatrix[secondOrderMatrixID, postSecondMatrixID].TravelTime;
            tempTT += Data.DistanceMatrix[preSecondMatrixID, firstOrderMatrixID].TravelTime;
            tempTT += Data.DistanceMatrix[firstOrderMatrixID, postSecondMatrixID].TravelTime;

            tempTT -= Data.DistanceMatrix[preThirdMatrixID, thirdOrderMatrixID].TravelTime;
            tempTT -= Data.DistanceMatrix[thirdOrderMatrixID, postThirdMatrixID].TravelTime;
            tempTT += Data.DistanceMatrix[preThirdMatrixID, secondOrderMatrixID].TravelTime;
            tempTT += Data.DistanceMatrix[secondOrderMatrixID, postThirdMatrixID].TravelTime;

            return tempTT < timeLimit;
        }

        public bool CanHalfSwapOrder(Order firstOrder, Order secondOrder, double timeLimit = 43200.0d)
        {
            if (firstOrder.OrderNumber == 0 || secondOrder.OrderNumber == 0)
                return false;

            int firstOrderIndex = Orders.FindIndex(o => o.OrderNumber == firstOrder.OrderNumber);
            int secondOrderIndex = Orders.FindIndex(o => o.OrderNumber == secondOrder.OrderNumber);
            int preSecondOrderIndex = secondOrderIndex == 0 ? 287 : secondOrderIndex - 1;

            int firstOrderMatrixID = firstOrder.MatrixID;
            int postFirstMatrixID = Orders[firstOrderIndex + 1].MatrixID;
            int preSecondMatrixID = Orders[preSecondOrderIndex].MatrixID;
            int secondOrderMatrixID = secondOrder.MatrixID;
            int postSecondMatrixID = Orders[secondOrderIndex + 1].MatrixID;

            double tempTT = TravelTime;
            tempTT -= Data.DistanceMatrix[firstOrderMatrixID, postFirstMatrixID].TravelTime;
            tempTT += Data.DistanceMatrix[firstOrderMatrixID, secondOrderMatrixID].TravelTime;
            tempTT += Data.DistanceMatrix[secondOrderMatrixID, postFirstMatrixID].TravelTime;

            tempTT -= Data.DistanceMatrix[preSecondMatrixID, secondOrderMatrixID].TravelTime;
            tempTT -= Data.DistanceMatrix[secondOrderMatrixID, postSecondMatrixID].TravelTime;
            tempTT += Data.DistanceMatrix[preSecondMatrixID, postSecondMatrixID].TravelTime;

            return tempTT < timeLimit;
        }

        public bool CanHalfSwapOrder(Order firstOrder, Order secondOrder, Order thirdOrder, double timeLimit = 43200.0d)
        {
            if (firstOrder.OrderNumber == 0 || secondOrder.OrderNumber == 0 || secondOrder.OrderNumber == 0)
                return false;

            int firstOrderIndex = Orders.FindIndex(o => o.OrderNumber == firstOrder.OrderNumber);
            int secondOrderIndex = Orders.FindIndex(o => o.OrderNumber == secondOrder.OrderNumber);
            int preSecondOrderIndex = secondOrderIndex == 0 ? 287 : secondOrderIndex - 1;
            int thirdOrderIndex = Orders.FindIndex(o => o.OrderNumber == thirdOrder.OrderNumber);
            int preThirdOrderIndex = thirdOrderIndex == 0 ? 287 : thirdOrderIndex - 1;

            int firstOrderMatrixID = firstOrder.MatrixID;
            int postFirstMatrixID = Orders[firstOrderIndex + 1].MatrixID;
            int preSecondMatrixID = Orders[preSecondOrderIndex].MatrixID;
            int secondOrderMatrixID = secondOrder.MatrixID;
            int postSecondMatrixID = Orders[secondOrderIndex + 1].MatrixID;
            int preThirdMatrixID = Orders[preThirdOrderIndex].MatrixID;
            int thirdOrderMatrixID = thirdOrder.MatrixID;
            int postThirdMatrixID = Orders[thirdOrderIndex + 1].MatrixID;

            double tempTT = TravelTime;
            tempTT -= Data.DistanceMatrix[firstOrderMatrixID, postFirstMatrixID].TravelTime;
            tempTT += Data.DistanceMatrix[firstOrderMatrixID, secondOrderMatrixID].TravelTime;
            tempTT += Data.DistanceMatrix[secondOrderMatrixID, postFirstMatrixID].TravelTime;

            tempTT -= Data.DistanceMatrix[preSecondMatrixID, secondOrderMatrixID].TravelTime;
            tempTT -= Data.DistanceMatrix[secondOrderMatrixID, postSecondMatrixID].TravelTime;
            tempTT += Data.DistanceMatrix[preSecondMatrixID, thirdOrderMatrixID].TravelTime;
            tempTT += Data.DistanceMatrix[thirdOrderMatrixID, postSecondMatrixID].TravelTime;

            tempTT -= Data.DistanceMatrix[preThirdMatrixID, thirdOrderMatrixID].TravelTime;
            tempTT -= Data.DistanceMatrix[thirdOrderMatrixID, postThirdMatrixID].TravelTime;
            tempTT += Data.DistanceMatrix[preThirdMatrixID, postThirdMatrixID].TravelTime;

            return tempTT < timeLimit;
        }

        public void SwapOrders(Order firstOrder, Order secondOrder)
        {
            int indexOfTheFirstOrder = Orders.FindIndex(o => o.OrderNumber == firstOrder.OrderNumber);
            int indexOfTheSecondOrder = Orders.FindIndex(o => o.OrderNumber == secondOrder.OrderNumber);

            if (indexOfTheFirstOrder == -1 || indexOfTheSecondOrder == -1)
                return;

            if (indexOfTheFirstOrder > indexOfTheSecondOrder)
            {
                AddOrderAt(secondOrder, firstOrder);
                RemoveOrder(firstOrder);
                AddOrderAt(firstOrder, secondOrder);
                RemoveOrder(secondOrder);
            }
            else
            {
                AddOrderAt(firstOrder, secondOrder);
                RemoveOrder(secondOrder);
                AddOrderAt(secondOrder, firstOrder);
                RemoveOrder(firstOrder);
            }
        }

        public void SwapOrders(Order firstOrder, Order secondOrder, Order thirdOrder)
        {
            int indexOfTheFirstOrder = Orders.FindIndex(o => o.OrderNumber == firstOrder.OrderNumber);
            int indexOfTheSecondOrder = Orders.FindIndex(o => o.OrderNumber == secondOrder.OrderNumber);
            int indexOfTheThirdOrder = Orders.FindIndex(o => o.OrderNumber == thirdOrder.OrderNumber);

            if (indexOfTheFirstOrder == -1 || indexOfTheSecondOrder == -1 || indexOfTheThirdOrder == -1)
                return;

            if (indexOfTheFirstOrder > indexOfTheSecondOrder)
            {
                if (indexOfTheFirstOrder > indexOfTheThirdOrder)
                {
                    if (indexOfTheSecondOrder > indexOfTheThirdOrder)
                    {
                        AddOrderAt(secondOrder, firstOrder);
                        RemoveOrder(firstOrder);
                        AddOrderAt(thirdOrder, secondOrder);
                        RemoveOrder(secondOrder);
                        AddOrderAt(firstOrder, thirdOrder);
                        RemoveOrder(thirdOrder);
                    }
                    else
                    {
                        AddOrderAt(secondOrder, firstOrder);
                        RemoveOrder(firstOrder);
                        AddOrderAt(firstOrder, thirdOrder);
                        RemoveOrder(thirdOrder);
                        AddOrderAt(thirdOrder, secondOrder);
                        RemoveOrder(secondOrder);
                    }
                }
                else
                {
                    AddOrderAt(firstOrder, thirdOrder);
                    RemoveOrder(thirdOrder);
                    AddOrderAt(secondOrder, firstOrder);
                    RemoveOrder(firstOrder);
                    AddOrderAt(thirdOrder, secondOrder);
                    RemoveOrder(secondOrder);
                }
            }
            else
            {
                if (indexOfTheSecondOrder > indexOfTheThirdOrder)
                {
                    if (indexOfTheFirstOrder > indexOfTheThirdOrder)
                    {
                        AddOrderAt(thirdOrder, secondOrder);
                        RemoveOrder(secondOrder);
                        AddOrderAt(secondOrder, firstOrder);
                        RemoveOrder(firstOrder);
                        AddOrderAt(firstOrder, thirdOrder);
                        RemoveOrder(thirdOrder);
                    }
                    else
                    {
                        AddOrderAt(thirdOrder, secondOrder);
                        RemoveOrder(secondOrder);
                        AddOrderAt(firstOrder, thirdOrder);
                        RemoveOrder(thirdOrder);
                        AddOrderAt(secondOrder, firstOrder);
                        RemoveOrder(firstOrder);
                    }
                }
                else
                {
                    AddOrderAt(firstOrder, thirdOrder);
                    RemoveOrder(thirdOrder);
                    AddOrderAt(thirdOrder, secondOrder);
                    RemoveOrder(secondOrder);
                    AddOrderAt(secondOrder, firstOrder);
                    RemoveOrder(firstOrder);
                }
            }
        }

        public void HalfSwapOrders(Order firstOrder, Order secondOrder)
        {
            RemoveOrder(secondOrder);
            AddOrderAt(secondOrder, firstOrder);
        }

        public void HalfSwapOrders(Order firstOrder, Order secondOrder, Order thirdOrder)
        {
            RemoveOrder(thirdOrder);
            AddOrderAt(thirdOrder, secondOrder);
            RemoveOrder(secondOrder);
            AddOrderAt(secondOrder, firstOrder);
        }

        public bool isValid(double timeLimit = 43200.0d)
        {
            return TravelTime < timeLimit && Weight < 100000;
        }
    }
}