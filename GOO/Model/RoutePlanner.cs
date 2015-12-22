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
            Order depositPoint = FilesInitializer.GetOrder0();
            Order A = depositPoint;
            Order B = depositPoint;

            // Order array to choose orders from with a random
            List<Order> AvailableClusterOrders = cluster.OrdersInCluster;
            List<Order> OrdersInRoute = toFill.Orders;

            OrdersCounter ClusterCounter = cluster.OrdersCounter;

            foreach (Days Day in Enum.GetValues(typeof(Days)))
            {
                if(cluster.DaysRestrictions.Find(d => d.HasFlag(Day)) == Days.None)
                    continue;

                List<Order> AvailableOrders = createAvailableOrdersForDay(Day, ClusterCounter, AvailableClusterOrders);

                while (toFill.Weight < maxWeight && toFill.TravelTime < maxTravelTime && maxSteps > steps)
                {
                    int randomOrderToSelect = random.Next(0, AvailableClusterOrders.Count);
                    Order bestOrder = AvailableClusterOrders[randomOrderToSelect];
                    int randomAmountOfLoops = random.Next(5, 50);

                    //make 5 ~ 50 points and get the best one
                    for (int i = 0; i < randomAmountOfLoops; i++)
                    {
                        int randomOrderToSelect2 = random.Next(1, AvailableClusterOrders.Count);
                        int travelLocation1 = FilesInitializer._DistanceMatrix.Matrix[A.MatrixID, AvailableClusterOrders[randomOrderToSelect].MatrixID].TravelTime;
                        int travelLocation2 = FilesInitializer._DistanceMatrix.Matrix[A.MatrixID, AvailableClusterOrders[randomOrderToSelect2].MatrixID].TravelTime;
                        if (travelLocation2 < travelLocation1)
                        {
                            if (!OrdersInRoute.Contains(AvailableClusterOrders[randomOrderToSelect2]))
                            {
                                bestOrder = AvailableClusterOrders[randomOrderToSelect2];
                                randomOrderToSelect = randomOrderToSelect2;
                            }
                        }
                    }
                    B = bestOrder;

                    if (!OrdersInRoute.Contains(AvailableClusterOrders[randomOrderToSelect]) && !AvailableOrders.Contains(AvailableClusterOrders[randomOrderToSelect]) && !ClusterCounter.IsOrderCompleted(AvailableClusterOrders[randomOrderToSelect].OrderNumber)) //check if order is already in the order list
                    {
                        //check if the weight and travel time does not exceed their max values
                        if (toFill.Weight + (AvailableClusterOrders[randomOrderToSelect].NumberOfContainers * AvailableClusterOrders[randomOrderToSelect].VolumePerContainer) <= maxWeight &&
                            toFill.TravelTime +
                            FilesInitializer._DistanceMatrix.Matrix[A.MatrixID, B.MatrixID].TravelTime +
                            FilesInitializer._DistanceMatrix.Matrix[B.MatrixID, depositPoint.MatrixID].TravelTime -
                            FilesInitializer._DistanceMatrix.Matrix[A.MatrixID, depositPoint.MatrixID].TravelTime +
                            AvailableClusterOrders[randomOrderToSelect].EmptyingTimeInSeconds <=
                            maxTravelTime)
                        {
                            //add the order to the orderlist and update the matrix check value for the next run
                            //Console.WriteLine(OrderArray[randomInt].OrderNumber + "  Current travel time :" + TravelTime + " Added route time: " + FilesInitializer._DistanceMatrix.Matrix[matrixA, matrixB].TravelTime + "/" + OrderArray[randomInt].EmptyingTimeInSeconds);
                            A = AvailableClusterOrders[randomOrderToSelect];
                            toFill.AddOrder(AvailableClusterOrders[randomOrderToSelect]);
                        }
                        else
                        {
                            steps += 10;
                        }
                    }
                    steps++;
                }
                toReturn.Add(toFill);
                toFill = new Route(Days.None);
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