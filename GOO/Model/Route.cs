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

        public List<Order> Orders { get; set; } // List of orders for this route
        public double traveltime { get; set; } // the total travel time
        public int weight { get; set; } //the filled weight of the route
        static Random rnd = new Random(); // TODO: <- a new random doesnt have to be made for every route

        private Route(Route toCopy)
        {
            this.traveltime = toCopy.traveltime;
            this.weight = toCopy.weight;
            this._SC_Orders(toCopy.Orders);
        }

        public Route()
        {
            Orders = new List<Order>();
            traveltime = 0f+30f*60f;
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
            string[] orders = new string[Orders.Count+1];
            int i= 0;
            foreach(Order ord in Orders)
            {
                orders[i] = "1;1;" + (i + 1) + ";" + ord.OrderNumber.ToString()+";";
                i++;
            }
            orders[i] = "1;1;" + (i + 1) + ";0;";
            System.IO.File.WriteAllLines(@"C:\Routes\Route_orders.txt", orders);
        }

        /// <summary>
        /// This function creates a randomized route for the route object
        /// </summary>
        /// <param name="maxWeight">The max weght for this route</param>
        /// <param name="maxTravelTime">The max travel time for this route</param>
        /// <param name="maxSteps">The max itterations for this route to create</param>
        public void CreateRouteList(int maxWeight, float maxTravelTime, int maxSteps)
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
                int r = rnd.Next(1, OrderArray.Count); //-1????
                int bestOrder = OrderArray[r].MatrixID;
                //make 5 points and get the nearest one
                for (int i = 0; i < 10; i++)
                {
                    int r2 = rnd.Next(1, OrderArray.Count);
                    int travel1 = FilesInitializer._DistanceMatrix.Matrix[matrixA, OrderArray[r].MatrixID].TravelTime;
                    int travel2 = FilesInitializer._DistanceMatrix.Matrix[matrixA, OrderArray[r2].MatrixID].TravelTime;
                    if(travel2 < travel1)
                    {
                        bestOrder = OrderArray[r2].MatrixID;
                        r = r2;
                    }
                }
                matrixB = bestOrder;

                if (Orders.Contains(OrderArray[r]) == false) //check if order is already in the order list
                {
                    //check if the weight and travel time does not exceed thier max values
                    if (weight + (OrderArray[r].NumberOfContainers * OrderArray[r].VolumePerContainer) <= maxWeight &&
                        traveltime + 
                        FilesInitializer._DistanceMatrix.Matrix[matrixA, matrixB].TravelTime +
                        FilesInitializer._DistanceMatrix.Matrix[matrixB, depositPoint].TravelTime - 
                        FilesInitializer._DistanceMatrix.Matrix[matrixA, depositPoint].TravelTime +
                        OrderArray[r].EmptyingTimeInSeconds <= 
                        maxTravelTime)
                    {
                        //add the order to the orderlist and update the matrix check value for the next run
                        Console.WriteLine(OrderArray[r].OrderNumber + "  Current travel time :" + traveltime + " Added route time: "+FilesInitializer._DistanceMatrix.Matrix[matrixA, matrixB].TravelTime+"/"+OrderArray[r].EmptyingTimeInSeconds);
                        matrixA = OrderArray[r].MatrixID;
                        AddOrder(OrderArray[r]);
                            
                    }
                    else
                    {
                        steps += 10;
                    }
                }
                steps++;
            }
            Console.WriteLine("DONE Creating Route: Truck weight["+weight+"/"+maxWeight+"] traveltime["+traveltime+"/"+maxTravelTime+ "] steps["+steps+"/"+maxSteps+"].");
            printRouteToFile();
        }


        /// <summary>
        /// This function removes the specified order from the order list and updates the traveltime and weight
        /// </summary>
        /// <param name="ord"> Removes the given order from the list </param>
        public void RemoveOrder(Order ord)
        {
            int prevO = 287; //node before the removed node
            int currO = ord.MatrixID; // the removed node
            int nextO = 287; //node after the removed node

            int indexnr = Orders.FindIndex(o => o.OrderNumber == ord.OrderNumber);

            if (indexnr > 0) //if the removed node is not the first node
            {
                prevO = Orders[indexnr - 1].MatrixID;
            }
            if (indexnr < Orders.Count-1) //if the removed node is not the last node
            {
                nextO = Orders[indexnr + 1].MatrixID;
            }

            //update travel time
            traveltime -= FilesInitializer._DistanceMatrix.Matrix[prevO, currO].TravelTime;
            traveltime -= FilesInitializer._DistanceMatrix.Matrix[currO, nextO].TravelTime;
            traveltime += FilesInitializer._DistanceMatrix.Matrix[prevO, nextO].TravelTime;
            traveltime -= ord.EmptyingTimeInSeconds; //remove empty time
            weight -= ord.VolumePerContainer * ord.NumberOfContainers; //adds the weight of this order
            Orders.Remove(ord);
        }

        /// <summary>
        /// This function will add an order at the end of the current Order list.
        /// </summary>
        /// <param name="ord">The order id to be added at the end of the list </param>
        public void AddOrder(Order ord)
        {
            int midA = 287; // the previous coordinate
            int midB = ord.MatrixID; // new added coordinate
            int midC = 287; // the dropping coordinate
            if (Orders.Count < 1)
            {
                midA = midC; // dropping coordinate 
            }
            else
            {
                midA = Orders[Orders.Count - 1].MatrixID; // last coordinate
            }        
            weight += ord.VolumePerContainer * ord.NumberOfContainers; //adds the weight of this order
            traveltime += ord.EmptyingTimeInSeconds; //adds empty time
            //now we need to calculate back drive times aswel
            traveltime -= FilesInitializer._DistanceMatrix.Matrix[midA, midC].TravelTime; //removes the old travel time to base // if midA = 0 it will remove 0 so no extra if check needed
            traveltime += FilesInitializer._DistanceMatrix.Matrix[midA, midB].TravelTime; //adds the travel time from last node to this node
            traveltime += FilesInitializer._DistanceMatrix.Matrix[midB, midC].TravelTime; //adds the new travel time to base from the added node

            Orders.Add(ord); // adds the order to the order list
        }

        /// <summary>
        /// This function will add a new order afther the specified order in the orderlist.
        /// </summary>
        /// <param name="ordnew"> The new Order to be added </param>
        /// <param name="afterord"> The Order the new it will be placed after </param>
        public void AddOrder(Order ordnew, Order afterord)
        {
            int midA = afterord.MatrixID; //the coord this order will be placed afhter
            int midB = ordnew.MatrixID; // new added coordinate
            int midC = 287; // the dropping coordinate
            if (Orders.FindIndex(o => o.OrderNumber == afterord.OrderNumber) == Orders.Count-1) //if order is added on the end of the list
            {
                traveltime -= FilesInitializer._DistanceMatrix.Matrix[midA, midC].TravelTime; //remove travel time to end if was last item on list
                traveltime += FilesInitializer._DistanceMatrix.Matrix[midB, midC].TravelTime; //adds the new travel time to base from the added node
                traveltime += FilesInitializer._DistanceMatrix.Matrix[midA, midB].TravelTime; //adds the new travel time from the prev to the added node
                traveltime += ordnew.EmptyingTimeInSeconds; //adds empty time
            }
            else
            {
                int Ordernext = Orders.FindIndex(o => o.OrderNumber == afterord.OrderNumber)+1; //gets the orderid in the orderlist
                midC = Orders[Ordernext].MatrixID; // gets the matric id of the obtained orderid
                traveltime -= FilesInitializer._DistanceMatrix.Matrix[midA, midC].TravelTime; //remove travel time to next node from previous node
                traveltime += FilesInitializer._DistanceMatrix.Matrix[midA, midB].TravelTime; //adds the new travel time from previous to the new node
                traveltime += FilesInitializer._DistanceMatrix.Matrix[midB, midC].TravelTime; //adds the new travel time from the new node to the next node
                traveltime += ordnew.EmptyingTimeInSeconds; //adds empty time
            }
            weight += ordnew.VolumePerContainer * ordnew.NumberOfContainers; //adds the weight of this order
            Orders.Add(ordnew); // adds the order to the order list
        }


    }
}