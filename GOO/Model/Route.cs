using GOO.Utilities;
using System;
using System.Collections.Generic;

namespace GOO.Model
{
    /// <summary>
    /// Part of a route for a day for a specific truck
    /// </summary>
    public class Route
    {
        // add variable to indicate for which day this route is meant for

        public List<Order> Orders { get; private set; } // List of orders for this route
        public double TravelTime { get; private set; } // the total travel time
        public int Weight { get; private set; } //the filled weight of the route
        static Random random = new Random();

        private Route(Route toCopy)
        {
            this.TravelTime = toCopy.TravelTime;
            this.Weight = toCopy.Weight;
            this.Orders = SC_Orders(toCopy.Orders);
        }

        public Route()
        {
            Orders = new List<Order>();
            TravelTime = 1800.0d;
        }

        private List<Order> SC_Orders(List<Order> listToCopy)
        {
            return new List<Order>(listToCopy);
        }

        public Route GetShallowCopy()
        {
            return new Route(this);
        }

        /// <summary>
        /// This function creates a randomized route for the route object
        /// </summary>
        /// <param name="maxWeight">The max weght for this route</param>
        /// <param name="maxTravelTime">The max travel time for this route</param>
        /// <param name="maxSteps">The max itterations for this route to create</param>
        public void CreateRouteList(int maxWeight, double maxTravelTime, int maxSteps)
        {
            //stepper and matrix values for travel time checks
            int steps = 0;
            int depositPoint = 287;
            int matrixA = 287;
            int matrixB = 287;
            //order array to choose orders from with a random
            List<Order> OrderArray = new List<Order>(FilesInitializer._Orders);
            while (Weight < maxWeight && TravelTime < maxTravelTime && maxSteps > steps) //nog een max step count
            {
                int randomInt = random.Next(1, OrderArray.Count); //-1????
                int bestOrder = OrderArray[randomInt].MatrixID;
                int randomAmountOfLoops = random.Next(5, 50);

                //make 5 ~ 50 points and get the best one
                for (int i = 0; i < randomAmountOfLoops; i++)
                {
                    int randomInt2 = random.Next(1, OrderArray.Count);
                    int travelLocation1 = FilesInitializer._DistanceMatrix.Matrix[matrixA, OrderArray[randomInt].MatrixID].TravelTime;
                    int travelLocation2 = FilesInitializer._DistanceMatrix.Matrix[matrixA, OrderArray[randomInt2].MatrixID].TravelTime;
                    if (travelLocation2 < travelLocation1)
                    {
                        if (!Orders.Contains(OrderArray[randomInt2]))
                        {
                            bestOrder = OrderArray[randomInt2].MatrixID;
                            randomInt = randomInt2;
                        }
                    }
                }
                matrixB = bestOrder;

                if (!Orders.Contains(OrderArray[randomInt])) //check if order is already in the order list
                {
                    //check if the weight and travel time does not exceed thier max values
                    if (Weight + (OrderArray[randomInt].NumberOfContainers * OrderArray[randomInt].VolumePerContainer) <= maxWeight &&
                        TravelTime +
                        FilesInitializer._DistanceMatrix.Matrix[matrixA, matrixB].TravelTime +
                        FilesInitializer._DistanceMatrix.Matrix[matrixB, depositPoint].TravelTime -
                        FilesInitializer._DistanceMatrix.Matrix[matrixA, depositPoint].TravelTime +
                        OrderArray[randomInt].EmptyingTimeInSeconds <=
                        maxTravelTime)
                    {
                        //add the order to the orderlist and update the matrix check value for the next run
                        //Console.WriteLine(OrderArray[randomInt].OrderNumber + "  Current travel time :" + TravelTime + " Added route time: " + FilesInitializer._DistanceMatrix.Matrix[matrixA, matrixB].TravelTime + "/" + OrderArray[randomInt].EmptyingTimeInSeconds);
                        matrixA = OrderArray[randomInt].MatrixID;
                        AddOrder(OrderArray[randomInt]);

                    }
                    else
                    {
                        steps += 10;
                    }
                }
                steps++;
            }

            Orders.Add(FilesInitializer.GetOrder0());
            //Console.WriteLine("DONE Creating Route: Truck weight[" + Weight + "/" + maxWeight + "] traveltime[" + TravelTime + "/" + maxTravelTime + "] steps[" + steps + "/" + maxSteps + "].");
        }

        /// <summary>
        /// This function removes the specified order from the order list and updates the traveltime and weight
        /// </summary>
        /// <param name="order"> Removes the given order from the list </param>
        public void RemoveOrder(Order order)
        {
            int previousOrder = 287; //node before the removed node
            int currentOrder = order.MatrixID; // the removed node
            int nextOrder = 287; //node after the removed node
            int indexNumber = Orders.FindIndex(o => o.OrderNumber == order.OrderNumber);

            if (indexNumber > 0) //if the removed node is not the first node
            {
                previousOrder = Orders[indexNumber - 1].MatrixID;
            }
            if (indexNumber < Orders.Count - 1) //if the removed node is not the last node
            {
                nextOrder = Orders[indexNumber + 1].MatrixID;
            }

            //update travel time
            TravelTime -= FilesInitializer._DistanceMatrix.Matrix[previousOrder, currentOrder].TravelTime;
            TravelTime -= FilesInitializer._DistanceMatrix.Matrix[currentOrder, nextOrder].TravelTime;
            TravelTime += FilesInitializer._DistanceMatrix.Matrix[previousOrder, nextOrder].TravelTime;
            TravelTime -= order.EmptyingTimeInSeconds; //remove empty time
            Weight -= order.VolumePerContainer * order.NumberOfContainers; //adds the weight of this order
            Orders.Remove(order);
        }

        /// <summary>
        /// This function will add an order at the end of the current Order list.
        /// </summary>
        /// <param name="order">The order id to be added at the end of the list </param>
        public void AddOrder(Order order)
        {
            int matrixIDA = 287; // the previous coordinate
            int matrixIDB = order.MatrixID; // new added coordinate
            int matrixIDC = 287; // the dropping coordinate

            if (Orders.Count < 1)
            {
                matrixIDA = matrixIDC; // dropping coordinate 
            }
            else
            {
                matrixIDA = Orders[Orders.Count - 1].MatrixID; // last coordinate
            }

            Weight += order.VolumePerContainer * order.NumberOfContainers; //adds the weight of this order
            TravelTime += order.EmptyingTimeInSeconds; //adds empty time
            //now we need to calculate back drive times as well
            TravelTime -= FilesInitializer._DistanceMatrix.Matrix[matrixIDA, matrixIDC].TravelTime; //removes the old travel time to base // if midA = 0 it will remove 0 so no extra if check needed
            TravelTime += FilesInitializer._DistanceMatrix.Matrix[matrixIDA, matrixIDB].TravelTime; //adds the travel time from last node to this node
            TravelTime += FilesInitializer._DistanceMatrix.Matrix[matrixIDB, matrixIDC].TravelTime; //adds the new travel time to base from the added node

            Orders.Add(order); // adds the order to the order list
        }

        /// <summary>
        /// This function will add a new order afther the specified order in the orderlist.
        /// </summary>
        /// <param name="newOrder"> The new Order to be added </param>
        /// <param name="afterOrder"> The Order the new it will be placed after </param>
        public void AddOrder(Order newOrder, Order afterOrder)
        {
            int matrixIDA = afterOrder.MatrixID; //the coord this order will be placed afhter
            int matrixIDB = newOrder.MatrixID; // new added coordinate
            int matrixIDC = 287; // the dropping coordinate

            if (Orders.FindIndex(o => o.OrderNumber == afterOrder.OrderNumber) == Orders.Count - 1) //if order is added on the end of the list
            {
                TravelTime -= FilesInitializer._DistanceMatrix.Matrix[matrixIDA, matrixIDC].TravelTime; //remove travel time to end if was last item on list
                TravelTime += FilesInitializer._DistanceMatrix.Matrix[matrixIDB, matrixIDC].TravelTime; //adds the new travel time to base from the added node
                TravelTime += FilesInitializer._DistanceMatrix.Matrix[matrixIDA, matrixIDB].TravelTime; //adds the new travel time from the prev to the added node
                TravelTime += newOrder.EmptyingTimeInSeconds; //adds empty time
            }
            else
            {
                int Ordernext = Orders.FindIndex(o => o.OrderNumber == afterOrder.OrderNumber) + 1; //gets the orderid in the orderlist
                matrixIDC = Orders[Ordernext].MatrixID; // gets the matric id of the obtained orderid
                TravelTime -= FilesInitializer._DistanceMatrix.Matrix[matrixIDA, matrixIDC].TravelTime; //remove travel time to next node from previous node
                TravelTime += FilesInitializer._DistanceMatrix.Matrix[matrixIDA, matrixIDB].TravelTime; //adds the new travel time from previous to the new node
                TravelTime += FilesInitializer._DistanceMatrix.Matrix[matrixIDB, matrixIDC].TravelTime; //adds the new travel time from the new node to the next node
                TravelTime += newOrder.EmptyingTimeInSeconds; //adds empty time
            }

            Weight += newOrder.VolumePerContainer * newOrder.NumberOfContainers; //adds the weight of this order
            Orders.Add(newOrder); // adds the order to the order list
        }
    }
}