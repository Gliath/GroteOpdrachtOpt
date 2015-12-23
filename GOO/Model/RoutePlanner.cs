using System;
using System.Collections.Generic;

using GOO.Utilities;
using GOO.Model.Optimizers.Strategies;

namespace GOO.Model
{
    public class RoutePlanner
    {
        private static Random random = new Random();

        public static Solution PlanRoutesFromClustersIntoSolution(Solution solution, List<ParentCluster> clusters)
        {
            Console.WriteLine("Planning routes into solution!");

            Dictionary<Days, List<Route>> DayRoutes = new Dictionary<Days, List<Route>>();
            foreach (Days Day in Enum.GetValues(typeof(Days)))
            {
                if (Day != Days.None)
                    DayRoutes.Add(Day, createAvailableRoutesForDayFromQuadrants(Day, clusters));
            }

            double maxTravelTimeOnDay = 43200.0d;
            double travelTimeOnDay = 0.0d;

            for (int i = 0; i < 2; i++)
            {
                foreach (Days Day in DayRoutes.Keys)
                {
                    List<Route> plannedRoutes = new List<Route>();
                    List<Route> toWorkWith = DayRoutes[Day];
                    bool canFitMore = true;
                    while (travelTimeOnDay < maxTravelTimeOnDay && canFitMore)
                    {
                        Route bestRoute = null;
                        foreach (Route route in toWorkWith)
                        {
                            if (bestRoute == null)
                                bestRoute = route;
                            else if (route.TravelTime < bestRoute.TravelTime)
                                bestRoute = route;
                        }
                        if (bestRoute == null)
                            canFitMore = false;
                        else
                        {
                            plannedRoutes.Add(bestRoute);
                            toWorkWith.Remove(bestRoute);
                            travelTimeOnDay += bestRoute.TravelTime;
                        }
                    }
                    if (plannedRoutes.Count > 0)
                        solution.AddNewItemToPlanning(Day, i, plannedRoutes);

                    travelTimeOnDay = 0.0d;
                }
            }

            Console.WriteLine("Done with planning routes into solution!");

            return solution;
        }

        public static List<ParentCluster> PlanStartClusters(List<ParentCluster> clusters)
        {
            // Step 1. Attempt to assign new days to clusters 
            // Based on weight and traveltime contained per day in a cluster instead?
            // Currently doing it randomly

            List<Days> days = new List<Days>();

            foreach (Days Day in Enum.GetValues(typeof(Days)))
            {
                if (Day == Days.None)
                    continue;
                days.Add(Day);
            }

            foreach (ParentCluster parent in clusters)
            {
                for (int i = 0; i < parent.Quadrants.Length; i++)
                {
                    bool assigned = false;
                    int numOfTries = 0;
                    do
                    {
                        Days dayToAttempt = days[random.Next(days.Count)];
                        if (parent.CanSetDaysPlanned(dayToAttempt))
                        {
                            parent.SetDaysPlannedForQuadrant(dayToAttempt, i);
                            assigned = true;
                        }
                    } while (!assigned && numOfTries < 100);
                }
            }

            // Step 2. Generate Routes for each quadrant within the parent cluster
            // Leveraging Opt-2 for creating a route in a quadrant

            RandomRouteOpt2Strategy opt2 = new RandomRouteOpt2Strategy();
            foreach (ParentCluster parent in clusters)
            {
                foreach (Cluster quadrant in parent.Quadrants)
                {
                    Route toAdd = new Route(quadrant.DaysPlannedFor);
                    foreach (Order order in quadrant.OrdersInCluster) // TODO : Check for max-weight
                    {
                        toAdd.AddOrder(order);
                    }
                    quadrant.AddRouteToCluster(opt2.opt2(toAdd)); // TODO : Check for max traveltime
                }
            }
            return clusters;
        }

        public static Dictionary<Days, Order> PlanRoute(List<Cluster> clusters)
        {
            Dictionary<Days, Order> planRoutes = new Dictionary<Days, Order>();


            return planRoutes;
        }

        private static List<Route> createAvailableRoutesForDayFromQuadrants(Days day, List<ParentCluster> parents)
        {
            List<Route> toReturn = new List<Route>();
            foreach (ParentCluster parent in parents)
            {
                foreach (Cluster quadrant in parent.Quadrants)
                {
                    if (quadrant.DaysPlannedFor == day)
                    {
                        toReturn.AddRange(quadrant.Routes); // TODO: See if this holds up with married clusters.
                    }
                }
            }
            return toReturn;
        }

        private static List<Order> createAvailableOrdersForDay(Days day, OrdersCounter orderCounter, List<Order> ordersToUse)
        {
            List<Order> toReturn = new List<Order>();
            foreach (Order order in ordersToUse)
            {
                if (orderCounter.CanAddOrder(order.OrderNumber, day))
                {
                    toReturn.Add(order);
                }
            }
            return toReturn;
        }
    }
}

//private static List<Route> generateRoutes(Cluster cluster) // WIP
//{
//    List<Route> toReturn = new List<Route>();
//    // Create amount of routes based on frequency
//    // Greedy pattern

//    // Max Steps and matrix values for travel time checks
//    int maxSteps = 10;
//    double maxTravelTime = 43200.0d; // TODO: Double-check values 
//    double maxWeight = 100000.0d;

//    int steps = 0;
//    Order depositPoint = Data.GetOrder0();
//    Order previousAddedOrder = depositPoint;
//    OrdersCounter ClusterCounter = null; // cluster.OrdersCounter -> get it from solution

//    List<Order> AvailableClusterOrders = cluster.OrdersInCluster; // Order array to choose orders from with a random

//    foreach (Days Day in Enum.GetValues(typeof(Days)))
//    {
//        if (Day == Days.None)
//            continue;

//        Route toFill = new Route(Day);
//        List<Order> OrdersInRoute = toFill.Orders;
//        List<Order> AvailableOrders = createAvailableOrdersForDay(Day, ClusterCounter, AvailableClusterOrders);

//        while (toFill.Weight < maxWeight && toFill.TravelTime < maxTravelTime && maxSteps > steps && AvailableOrders.Count > 0)
//        {
//            int randomAmountOfLoops = random.Next(5, 50);
//            int randomOrderToSelect = random.Next(0, AvailableOrders.Count - 1);

//            Order bestOrder = AvailableOrders[randomOrderToSelect];
//            Order randomOrder = AvailableOrders[randomOrderToSelect];

//            //make 5 ~ 50 points and get the best one
//            for (int i = 0; i < randomAmountOfLoops; i++)
//            {
//                randomOrder = AvailableOrders[random.Next(0, AvailableOrders.Count - 1)];

//                int travelLocation1 = Data.DistanceMatrix[previousAddedOrder.MatrixID, bestOrder.MatrixID].TravelTime;
//                int travelLocation2 = Data.DistanceMatrix[previousAddedOrder.MatrixID, randomOrder.MatrixID].TravelTime;
//                if (travelLocation2 < travelLocation1)
//                {
//                    if (!OrdersInRoute.Contains(randomOrder))
//                    {
//                        bestOrder = randomOrder;
//                    }
//                }
//            }
//            AvailableOrders.Remove(bestOrder);

//            //check if the weight and travel time does not exceed their max values
//            if (toFill.Weight + (previousAddedOrder.NumberOfContainers * previousAddedOrder.VolumePerContainer) <= maxWeight &&
//                toFill.TravelTime +
//                Data.DistanceMatrix[previousAddedOrder.MatrixID, previousAddedOrder.MatrixID].TravelTime +
//                Data.DistanceMatrix[previousAddedOrder.MatrixID, depositPoint.MatrixID].TravelTime -
//                Data.DistanceMatrix[previousAddedOrder.MatrixID, depositPoint.MatrixID].TravelTime +
//                AvailableClusterOrders[randomOrderToSelect].EmptyingTimeInSeconds <= maxTravelTime)
//            {
//                //add the order to the orderlist and update the matrix check value for the next run
//                //Console.WriteLine(OrderArray[randomInt].OrderNumber + "  Current travel time :" + TravelTime + " Added route time: " + Data.Matrix[matrixA, matrixB].TravelTime + "/" + OrderArray[randomInt].EmptyingTimeInSeconds);
//                previousAddedOrder = bestOrder;
//                toFill.AddOrder(bestOrder);
//            }
//            else
//            {
//                steps += 10;
//            }
//            steps++;
//        }
//        if (toFill.Orders.Count > 1)
//            toReturn.Add(toFill);
//        toFill = new Route(Days.None);
//    }
//    return toReturn;
//}

//public static List<Cluster> PlanStartClusters(List<ParentCluster> clusters)
//{
//// Step 1. Generate Routes for each cluster dependent on the orderfrequency within
//foreach (Cluster cluster in clusters)
//{ // if (!cluster.OrdersCounter.IsCompleted())
//    foreach (Route route in generateRoutes(cluster))
//        cluster.AddRouteToCluster(route);
//}
//// Step 2. Attempt to assign new days to clusters 
//// Based on weight and traveltime contained per day in a cluster instead?
////double maxTravelTimeOnDay = 86400.0d; // TODO: Double-check values
////int weightOnDay = 0;
////double travelTimeOnDay = 0.0d;

//List<Days> randomDayList = new List<Days>();
//foreach (Days Day in Enum.GetValues(typeof(Days)))
//{
//    if (Day != Days.None)
//        randomDayList.Add(Day);
//}

//int randomizationSteps = 20;
//foreach (Cluster cluster in clusters)
//{
//    foreach (Days day in randomDayList)
//    {
//        if (cluster.CanBePlannedOn(day))
//            cluster.DaysPlannedFor |= day;
//    }

//    for (int i = 0; i < randomizationSteps; i++)
//    {
//        int swapIndex1 = random.Next(randomDayList.Count - 1);
//        int swapIndex2 = random.Next(randomDayList.Count - 1);

//        Days toSwap = randomDayList[swapIndex1];
//        Days toSwap2 = randomDayList[swapIndex2];

//        randomDayList.Remove(toSwap);
//        randomDayList.Insert(swapIndex1, toSwap2);

//        randomDayList.Remove(toSwap2);
//        randomDayList.Insert(swapIndex2, toSwap);
//    }
//}
//// Step 3. Attempt to plan the routes based on the day restrictions of their clusters 
//foreach (Cluster cluster in clusters)
//{
//    cluster.RemoveAllRoutesFromCluster();
//    cluster.Routes = generateRoutes(cluster);
//}
//return clusters;
//}