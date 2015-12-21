using System;
using System.Collections.Generic;

using GOO.Utilities;

namespace GOO.Model
{
    public class RoutePlanner
    {
        private static Random random = new Random();

        public static Dictionary<Days, Order> PlanRoute(List<Cluster> clusters)
        {
            Dictionary<Days, Order> planRoutes = null;

            // Step 1. Generate Routes for each cluster dependent on the orderfrequency within

            // Step 2. Attempt to assign new days to clusters 

            // Step 3. Attempt to plan the routes based on the day restrictions of their clusters 
            
            return planRoutes;
        }

        private static List<Route> generateRoutes(Cluster cluster) // WIP
        {
            List<Route> toReturn = new List<Route>();
            // Retrieve all orders

            // Create amount of routes based on frequency
            // Greedy pattern

            Route toFill = new Route(Days.None);

            // Max Steps and matrix values for travel time checks
            int maxSteps = 10;
            double maxTravelTime = 0.0d;
            double maxWeight = 0.0d;

            int steps = 0;
            int depositPoint = 287;
            int matrixA = 287;
            int matrixB = 287;
            
            // Order array to choose orders from with a random
            List<Order> OrderArray = cluster.OrdersInCluster;
            List<Order> OrdersInRoute = toFill.Orders;
            List<Order> AvailableOrders = new List<Order>(); // TODO : <--- Make this based on the OrdersCounter
            OrdersCounter clusterCounter = cluster.OrdersCounter;

            while (toFill.Weight < maxWeight && toFill.TravelTime < maxTravelTime && maxSteps > steps)
            {
                int randomInt = random.Next(0, OrderArray.Count);
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
                        if (!OrdersInRoute.Contains(OrderArray[randomInt2]))
                        {
                            bestOrder = OrderArray[randomInt2].MatrixID;
                            randomInt = randomInt2;
                        }
                    }
                }
                matrixB = bestOrder;

                if (!OrdersInRoute.Contains(OrderArray[randomInt]) && !AvailableOrders.Contains(OrderArray[randomInt]) && !clusterCounter.IsOrderCompleted(OrderArray[randomInt].OrderNumber)) //check if order is already in the order list
                {
                    //check if the weight and travel time does not exceed thier max values
                    if (toFill.Weight + (OrderArray[randomInt].NumberOfContainers * OrderArray[randomInt].VolumePerContainer) <= maxWeight &&
                        toFill.TravelTime +
                        FilesInitializer._DistanceMatrix.Matrix[matrixA, matrixB].TravelTime +
                        FilesInitializer._DistanceMatrix.Matrix[matrixB, depositPoint].TravelTime -
                        FilesInitializer._DistanceMatrix.Matrix[matrixA, depositPoint].TravelTime +
                        OrderArray[randomInt].EmptyingTimeInSeconds <=
                        maxTravelTime)
                    {
                        //add the order to the orderlist and update the matrix check value for the next run
                        //Console.WriteLine(OrderArray[randomInt].OrderNumber + "  Current travel time :" + TravelTime + " Added route time: " + FilesInitializer._DistanceMatrix.Matrix[matrixA, matrixB].TravelTime + "/" + OrderArray[randomInt].EmptyingTimeInSeconds);
                        matrixA = OrderArray[randomInt].MatrixID;
                        toFill.AddOrder(OrderArray[randomInt]);
                    }
                    else
                    {
                        steps += 10;
                    }
                }
                steps++;
            }


            return toReturn;
        }
    }
}