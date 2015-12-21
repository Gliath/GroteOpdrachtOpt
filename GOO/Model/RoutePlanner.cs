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
            List<Order> AvailableClusterOrders = cluster.OrdersInCluster;
            List<Order> OrdersInRoute = toFill.Orders;

            OrdersCounter ClusterCounter = cluster.OrdersCounter;

            // Build for Monday - Friday
                // Do i need to build for <day>? 
                    // Yes -> Make new route and update available orders
                    // No -> Skip iteration

            foreach (Days Day in Enum.GetValues(typeof(Days)))
            {
                if(cluster.DaysRestrictions.Find(d => d.HasFlag(Day)) == null)
                    continue;

                List<Order> AvailableOrders = createAvailableOrdersForDay(Day, ClusterCounter, AvailableClusterOrders);

                while (toFill.Weight < maxWeight && toFill.TravelTime < maxTravelTime && maxSteps > steps)
                {
                    int randomInt = random.Next(0, AvailableClusterOrders.Count);
                    int bestOrder = AvailableClusterOrders[randomInt].MatrixID;
                    int randomAmountOfLoops = random.Next(5, 50);

                    //make 5 ~ 50 points and get the best one
                    for (int i = 0; i < randomAmountOfLoops; i++)
                    {
                        int randomInt2 = random.Next(1, AvailableClusterOrders.Count);
                        int travelLocation1 = FilesInitializer._DistanceMatrix.Matrix[matrixA, AvailableClusterOrders[randomInt].MatrixID].TravelTime;
                        int travelLocation2 = FilesInitializer._DistanceMatrix.Matrix[matrixA, AvailableClusterOrders[randomInt2].MatrixID].TravelTime;
                        if (travelLocation2 < travelLocation1)
                        {
                            if (!OrdersInRoute.Contains(AvailableClusterOrders[randomInt2]))
                            {
                                bestOrder = AvailableClusterOrders[randomInt2].MatrixID;
                                randomInt = randomInt2;
                            }
                        }
                    }
                    matrixB = bestOrder;

                    if (!OrdersInRoute.Contains(AvailableClusterOrders[randomInt]) && !AvailableOrders.Contains(AvailableClusterOrders[randomInt]) && !ClusterCounter.IsOrderCompleted(AvailableClusterOrders[randomInt].OrderNumber)) //check if order is already in the order list
                    {
                        //check if the weight and travel time does not exceed thier max values
                        if (toFill.Weight + (AvailableClusterOrders[randomInt].NumberOfContainers * AvailableClusterOrders[randomInt].VolumePerContainer) <= maxWeight &&
                            toFill.TravelTime +
                            FilesInitializer._DistanceMatrix.Matrix[matrixA, matrixB].TravelTime +
                            FilesInitializer._DistanceMatrix.Matrix[matrixB, depositPoint].TravelTime -
                            FilesInitializer._DistanceMatrix.Matrix[matrixA, depositPoint].TravelTime +
                            AvailableClusterOrders[randomInt].EmptyingTimeInSeconds <=
                            maxTravelTime)
                        {
                            //add the order to the orderlist and update the matrix check value for the next run
                            //Console.WriteLine(OrderArray[randomInt].OrderNumber + "  Current travel time :" + TravelTime + " Added route time: " + FilesInitializer._DistanceMatrix.Matrix[matrixA, matrixB].TravelTime + "/" + OrderArray[randomInt].EmptyingTimeInSeconds);
                            matrixA = AvailableClusterOrders[randomInt].MatrixID;
                            toFill.AddOrder(AvailableClusterOrders[randomInt]);
                        }
                        else
                        {
                            steps += 10;
                        }
                    }
                    steps++;
                }

            }
            return toReturn;
        }

        private static List<Order> createAvailableOrdersForDay(Days day, OrdersCounter orderCounter, List<Order> ordersToUse)
        {
            List<Order> toReturn = new List<Order>();
            foreach (Order order in ordersToUse)
            {
                if (!orderCounter.HasOccurence(day, order.OrderNumber) && !orderCounter.IsOrderCompleted(order.OrderNumber))
                {
                    toReturn.Add(order);
                }
            }
            return toReturn;
        }
    }
}