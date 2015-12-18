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

        private List<Order> Orders { get; set; } // List of orders for this route
        private double traveltime { get; set; } // the total travel time
        private int weight { get; set; } //the filled weight of the route
        static Random random = new Random(); // TODO: <- a new random doesnt have to be made for every route

        private Route(Route toCopy)
        {
            this.traveltime = toCopy.traveltime;
            this.weight = toCopy.weight;
            this._SC_Orders(toCopy.Orders);
        }

        public Route()
        {
            Orders = new List<Order>();
            traveltime = 0.0d + 30.0d * 60.0d;
            //nothing to do here init
        }

        private void _SC_Orders(List<Order> listToCopy)
        {
            this.Orders = new List<Order>(listToCopy);
        }

        public Route GetShallowCopy()
        {
            return new Route(this);
        }

        public void printRouteToFile()
        {
            string[] orders = new string[Orders.Count + 1];
            for (int i = 0; i < Orders.Count; i++)
                orders[i] = "1;1;" + (i + 1) + ";" + Orders[i].OrderNumber.ToString() + ";";

            orders[Orders.Count] = "1;1;" + Orders.Count + ";0;";
            System.IO.File.WriteAllLines(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + "/Route_orders.txt", orders);
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
            while (weight < maxWeight && traveltime < maxTravelTime && maxSteps > steps) //nog een max step count
            {
                int randomInt = random.Next(1, OrderArray.Count); //-1????
                int bestOrder = OrderArray[randomInt].MatrixID;
                //make 10 points and get the nearest one
                for (int i = 0; i < 10; i++)
                {
                    int randomInt2 = random.Next(1, OrderArray.Count);
                    int travelLocation1 = FilesInitializer._DistanceMatrix.Matrix[matrixA, OrderArray[randomInt].MatrixID].TravelTime;
                    int travelLocation2 = FilesInitializer._DistanceMatrix.Matrix[matrixA, OrderArray[randomInt2].MatrixID].TravelTime;
                    if (travelLocation2 < travelLocation1)
                    {
                        bestOrder = OrderArray[randomInt2].MatrixID;
                        randomInt = randomInt2;
                    }
                }
                matrixB = bestOrder;

                if (Orders.Contains(OrderArray[randomInt]) == false) //check if order is already in the order list
                {
                    //check if the weight and travel time does not exceed thier max values
                    if (weight + (OrderArray[randomInt].NumberOfContainers * OrderArray[randomInt].VolumePerContainer) <= maxWeight &&
                        traveltime +
                        FilesInitializer._DistanceMatrix.Matrix[matrixA, matrixB].TravelTime +
                        FilesInitializer._DistanceMatrix.Matrix[matrixB, depositPoint].TravelTime -
                        FilesInitializer._DistanceMatrix.Matrix[matrixA, depositPoint].TravelTime +
                        OrderArray[randomInt].EmptyingTimeInSeconds <=
                        maxTravelTime)
                    {
                        //add the order to the orderlist and update the matrix check value for the next run
                        Console.WriteLine(OrderArray[randomInt].OrderNumber + "  Current travel time :" + traveltime + " Added route time: " + FilesInitializer._DistanceMatrix.Matrix[matrixA, matrixB].TravelTime + "/" + OrderArray[randomInt].EmptyingTimeInSeconds);
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
            Console.WriteLine("DONE Creating Route: Truck weight[" + weight + "/" + maxWeight + "] traveltime[" + traveltime + "/" + maxTravelTime + "] steps[" + steps + "/" + maxSteps + "].");
            printRouteToFile();
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
            traveltime -= FilesInitializer._DistanceMatrix.Matrix[previousOrder, currentOrder].TravelTime;
            traveltime -= FilesInitializer._DistanceMatrix.Matrix[currentOrder, nextOrder].TravelTime;
            traveltime += FilesInitializer._DistanceMatrix.Matrix[previousOrder, nextOrder].TravelTime;
            traveltime -= order.EmptyingTimeInSeconds; //remove empty time
            weight -= order.VolumePerContainer * order.NumberOfContainers; //adds the weight of this order
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

            weight += order.VolumePerContainer * order.NumberOfContainers; //adds the weight of this order
            traveltime += order.EmptyingTimeInSeconds; //adds empty time
            //now we need to calculate back drive times aswel
            traveltime -= FilesInitializer._DistanceMatrix.Matrix[matrixIDA, matrixIDC].TravelTime; //removes the old travel time to base // if midA = 0 it will remove 0 so no extra if check needed
            traveltime += FilesInitializer._DistanceMatrix.Matrix[matrixIDA, matrixIDB].TravelTime; //adds the travel time from last node to this node
            traveltime += FilesInitializer._DistanceMatrix.Matrix[matrixIDB, matrixIDC].TravelTime; //adds the new travel time to base from the added node

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
                traveltime -= FilesInitializer._DistanceMatrix.Matrix[matrixIDA, matrixIDC].TravelTime; //remove travel time to end if was last item on list
                traveltime += FilesInitializer._DistanceMatrix.Matrix[matrixIDB, matrixIDC].TravelTime; //adds the new travel time to base from the added node
                traveltime += FilesInitializer._DistanceMatrix.Matrix[matrixIDA, matrixIDB].TravelTime; //adds the new travel time from the prev to the added node
                traveltime += newOrder.EmptyingTimeInSeconds; //adds empty time
            }
            else
            {
                int Ordernext = Orders.FindIndex(o => o.OrderNumber == afterOrder.OrderNumber) + 1; //gets the orderid in the orderlist
                matrixIDC = Orders[Ordernext].MatrixID; // gets the matric id of the obtained orderid
                traveltime -= FilesInitializer._DistanceMatrix.Matrix[matrixIDA, matrixIDC].TravelTime; //remove travel time to next node from previous node
                traveltime += FilesInitializer._DistanceMatrix.Matrix[matrixIDA, matrixIDB].TravelTime; //adds the new travel time from previous to the new node
                traveltime += FilesInitializer._DistanceMatrix.Matrix[matrixIDB, matrixIDC].TravelTime; //adds the new travel time from the new node to the next node
                traveltime += newOrder.EmptyingTimeInSeconds; //adds empty time
            }

            weight += newOrder.VolumePerContainer * newOrder.NumberOfContainers; //adds the weight of this order
            Orders.Add(newOrder); // adds the order to the order list
        }
    }
}