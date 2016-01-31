using System;
using System.Collections.Generic;

using GOO.Utilities;
using GOO.Model.Optimizers.Strategies;

namespace GOO.Model
{
    public class RoutePlanner
    {
        private static Random random = new Random();

        public static Solution PlanRoutesFromClustersIntoSolution(Solution solution, List<AbstractCluster> clusters)
        {
            bool listAreParentClusters = false;
            List<ParentCluster> parentClusters = new List<ParentCluster>();
            List<MarriedCluster> marriedClusters = new List<MarriedCluster>();

            foreach (AbstractCluster cluster in clusters)
            {
                if (cluster.GetType() == typeof(ParentCluster))
                {
                    listAreParentClusters = true;
                    parentClusters.Add((ParentCluster)cluster);
                }
                else if (cluster.GetType() == typeof(MarriedCluster))
                    marriedClusters.Add((MarriedCluster)cluster);
            }

            Dictionary<Days, List<Route>> DayRoutes = new Dictionary<Days, List<Route>>();
            foreach (Days Day in Enum.GetValues(typeof(Days)))
            {
                if (Day != Days.None)
                    if (listAreParentClusters)
                        DayRoutes.Add(Day, createAvailableRoutesForDayFromQuadrants(Day, parentClusters));
                    else
                        DayRoutes.Add(Day, createAvailableRoutesForDayFromHarem(Day, marriedClusters));
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
                            else if (route.TravelTime < bestRoute.TravelTime && route.Orders.Count > 1 && route.TravelTime + travelTimeOnDay <= maxTravelTimeOnDay)
                                bestRoute = route;
                        }
                        if (bestRoute == null || bestRoute.TravelTime + travelTimeOnDay >= maxTravelTimeOnDay)
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
                    foreach (Route route in plannedRoutes)
                        DayRoutes[Day].Remove(route);
                }
            }

            return solution;
        }

        public static List<ParentCluster> PlanStartClusters(List<ParentCluster> clusters)
        {
            List<Days> days = new List<Days>();

            foreach (Days Day in Enum.GetValues(typeof(Days)))
                if (Day == Days.None)
                    continue;
                else
                    days.Add(Day);

            foreach (ParentCluster parent in clusters)
            {
                for (int quadrantIndex = 0; quadrantIndex < parent.Quadrants.Length; quadrantIndex++)
                {
                    bool assigned = false;
                    int numOfTries = 0;
                    do
                    {
                        Days dayToAttempt = days[random.Next(days.Count)];
                        if (parent.CanSetDaysPlanned(parent.Quadrants[quadrantIndex], dayToAttempt))
                            assigned = parent.SetDaysPlannedForQuadrant(parent.Quadrants[quadrantIndex], dayToAttempt);

                        numOfTries++;
                    } while (!assigned && numOfTries < 32);

                    if (!assigned)
                        Console.WriteLine("Error, a quadrant could not be assigned to a day");
                }
            }

            foreach (ParentCluster parent in clusters)
            {
                foreach (Cluster quadrant in parent.Quadrants)
                {
                    Route toAdd = new Route(quadrant.DaysPlannedFor);
                    foreach (Order order in quadrant.OrdersInCluster) // TODO : Check for max-weight
                        toAdd.AddOrder(order);

                    if (toAdd.Orders.Count > 1)
                        quadrant.AddRouteToCluster(toAdd); // TODO : Check for max traveltime
                    else
                        Console.WriteLine("COULD NOT CREATE ROUTE. Quadrant : {0}", quadrant);
                }
            }

            return clusters;
        }

        public static void AssignDaysToClusters(List<ParentCluster> clusters)
        {
            List<Days> days = new List<Days>();

            foreach (Days Day in Enum.GetValues(typeof(Days)))
                if (Day == Days.None)
                    continue;
                else
                    days.Add(Day);

            foreach (ParentCluster parent in clusters)
            {
                for (int quadrantIndex = 0; quadrantIndex < parent.Quadrants.Length; quadrantIndex++)
                {
                    bool assigned = false;
                    int numOfTries = 0;
                    do
                    {
                        Days dayToAttempt = days[random.Next(days.Count)];
                        if (parent.CanSetDaysPlanned(parent.Quadrants[quadrantIndex], dayToAttempt))
                            assigned = parent.SetDaysPlannedForQuadrant(parent.Quadrants[quadrantIndex], dayToAttempt);

                        numOfTries++;
                    } while (!assigned && numOfTries < 32);

                    if (!assigned)
                        Console.WriteLine("Error, a quadrant could not be assigned to a day");
                }
            }
        }

        public static void GenerateRoutesFromClusters(List<AbstractCluster> clustersToPlan)
        {
            List<MarriedCluster> Couples = new List<MarriedCluster>();
            foreach (AbstractCluster cluster in clustersToPlan)
                if (cluster.GetType() == typeof(MarriedCluster))
                    Couples.Add((MarriedCluster)cluster);

            Couples.Sort(); // Attempt to rearrange the list

            foreach (MarriedCluster Couple in Couples)
            {
                foreach (Route HaremRoute in Couple.Routes)
                {
                    bool hasAddedAnRoute = false;

                    foreach (Cluster Concubine in Couple.Harem)
                    {
                        double TravelTime = 0.0d;
                        int Weight = 0;

                        foreach (Route ConcubineRoutes in Concubine.Routes)
                        {
                            TravelTime += ConcubineRoutes.TravelTime;
                            Weight += ConcubineRoutes.Weight;
                        }

                        if ((TravelTime + HaremRoute.TravelTime <= 43200.0d) && (Weight + HaremRoute.Weight <= 100000))
                        {
                            Concubine.AddRouteToCluster(HaremRoute);
                            hasAddedAnRoute = true;
                        }

                        if (hasAddedAnRoute)
                            break;
                        else
                            Concubine.RemoveRouteFromCluster(HaremRoute);
                    }
                }
            }
        }

        private static List<Route> createAvailableRoutesForDayFromQuadrants(Days day, List<ParentCluster> parents)
        {
            List<Route> toReturn = new List<Route>();
            foreach (ParentCluster parent in parents)
                foreach (Cluster quadrant in parent.Quadrants)
                    if (quadrant.DaysPlannedFor == day)
                        foreach (Route route in quadrant.Routes)
                            if (route.Orders.Count > 1)
                                toReturn.Add(route); // TODO: See if this holds up with married clusters.

            return toReturn;
        }

        private static List<Route> createAvailableRoutesForDayFromHarem(Days day, List<MarriedCluster> haremClusters)
        {
            List<Route> toReturn = new List<Route>();
            foreach (MarriedCluster haremCluster in haremClusters)
                foreach (Cluster concubine in haremCluster.Harem)
                    if (concubine.DaysPlannedFor == day)
                        foreach (Route route in concubine.Routes)
                            if (route.Orders.Count > 1)
                                toReturn.Add(route); // TODO: See if this holds up with married clusters.

            return toReturn;
        }

        private static List<Order> createAvailableOrdersForDay(Days day, List<Order> ordersToUse)
        {
            List<Order> toReturn = new List<Order>();
            foreach (Order order in ordersToUse)
                if (order.CanBeAddedOnDay(day))
                    toReturn.Add(order);

            return toReturn;
        }
    }
}