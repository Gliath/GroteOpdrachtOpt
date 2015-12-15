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


        public void CreateRouteList(int maxWeight, double maxTravelTime, int maxSteps)
        {
            int steps = 0;
            while(weight < maxWeight && traveltime < maxTravelTime && maxSteps > steps) //nog een max step count
            {
                //add nodes
                //get current node location
                //select one of the nearest <?number> locations from the current node

                steps++;
            }
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
            weight -= ord.VolumePerContainer * ord.NumberOfContainers; //adds the weight of this order
            Orders.Remove(ord);
        }

        /// <summary>
        /// This function will add an order at the end of the current Order list.
        /// </summary>
        /// <param name="ord">The order id to be added at the end of the list </param>
        public void AddOrder(Order ord)
        {
            int midA = 0; // the previous coordinate
            int midB = ord.MatrixID; // new added coordinate
            int midC = 287; // the dropping coordinate
            if (Orders.Count == 0)
            {
                midA = midC; // dropping coordinate 
            }
            else
            {
                midA = Orders[Orders.Count - 1].MatrixID; // last coordinate
            }
            traveltime += FilesInitializer._DistanceMatrix.Matrix[midA, midB].TravelTime; //adds the travel time from last node to this node
            weight += ord.VolumePerContainer * ord.NumberOfContainers; //adds the weight of this order

            //now we need to calculate back drive times aswel
            traveltime -= FilesInitializer._DistanceMatrix.Matrix[midA, midC].TravelTime; //removes the old travel time to base // if midA = 0 it will remove 0 so no extra if check needed
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
            }
            else
            {
                int Ordernext = Orders.FindIndex(o => o.OrderNumber == afterord.OrderNumber)+1; //gets the orderid in the orderlist
                midC = Orders[Ordernext].MatrixID; // gets the matric id of the obtained orderid
                traveltime -= FilesInitializer._DistanceMatrix.Matrix[midA, midC].TravelTime; //remove travel time to next node from previous node
                traveltime += FilesInitializer._DistanceMatrix.Matrix[midA, midB].TravelTime; //adds the new travel time from previous to the new node
                traveltime += FilesInitializer._DistanceMatrix.Matrix[midB, midC].TravelTime; //adds the new travel time from the new node to the next node
            }
            weight += ordnew.VolumePerContainer * ordnew.NumberOfContainers; //adds the weight of this order
            Orders.Add(ordnew); // adds the order to the order list
        }


    }
}