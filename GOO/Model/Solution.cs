﻿using System;
using System.Collections.Generic;
using System.Collections;

using GOO.Utilities;
using System.Text;

namespace GOO.Model
{
    public class Solution
    {
        private List<ParentCluster> clusters;

        public List<OrderTracker> CounterList { get; private set; }
        public List<Route> AllRoutes { get; private set; }
        public List<Route> AvailableRoutes { get; private set; }

        public double SolutionScore { get; private set; }
        public double PenaltyScore { get; private set; }
        public double TravelTimeScore { get; private set; }

        private List<Tuple<Days, int, List<Route>>> TruckPlanning;

        public Solution(List<ParentCluster> clusters)
        {
            this.clusters = clusters;
            CounterList = new List<OrderTracker>();
            AllRoutes = new List<Route>();
            AvailableRoutes = new List<Route>();
            TruckPlanning = new List<Tuple<Days, int, List<Route>>>();
        }

        public void MakeBasicPlannings()
        {
            foreach (Days day in Enum.GetValues(typeof(Days)))
            {
                if (day.Equals(Days.None))
                    continue;

                for (int i = 0; i < 2; i++)
                    this.AddNewItemToPlanning(day, i, new List<Route>());
            }
        }

        public ParentCluster GetRandomParentCluster()
        {
            int random = new Random().Next(clusters.Count);
            return this.clusters[random];
        }

        public Cluster GetRandomCluster()
        {
            Random rng = new Random();
            ParentCluster cluster = this.clusters[rng.Next(clusters.Count)];
            return cluster.Quadrants[rng.Next(cluster.Quadrants.Length)];
        }

        public List<ParentCluster> GetAllClusters()
        {
            return this.clusters;
        }

        public void InitializeCounterList()
        {
            foreach (Order order in Data.Orders.Values)
                CounterList.Add(order.OrderTracker);
        }

        public void RemoveRouteFromPlanning(Days day, int truckID, Route toRemove)
        {
            Tuple<Days, int, List<Route>> DayPlanning = TruckPlanning.Find(t => t.Item1 == day && t.Item2 == truckID);
            if (DayPlanning != null)
            {
                if (DayPlanning.Item3.Remove(toRemove))
                    foreach (Order order in toRemove.Orders)
                        RemovePlannedOccurrence(order.OrderNumber, toRemove);
                AvailableRoutes.Add(toRemove);
            }
        }

        public void AddRouteToPlanning(Days day, int truckID, Route toAdd)
        {
            Tuple<Days, int, List<Route>> DayPlanning = TruckPlanning.Find(t => t.Item1 == day && t.Item2 == truckID);
            if (DayPlanning != null)
            {
                DayPlanning.Item3.Add(toAdd);
                foreach (Order order in toAdd.Orders)
                    AddPlannedOccurrence(order.OrderNumber, toAdd);
                AvailableRoutes.Remove(toAdd);
            }
        }

        public void AddNewItemToPlanning(Days day, int truckID, List<Route> routes)
        {
            TruckPlanning.Add(new Tuple<Days, int, List<Route>>(day, truckID, routes));
            foreach (Route route in routes)
            {
                foreach (Order order in route.Orders)
                    this.AddPlannedOccurrence(order.OrderNumber, route);
                AvailableRoutes.Remove(route);
            }
        }

        public void RemoveItemFromPlanning(Days day, int truckID)
        {
            Tuple<Days, int, List<Route>> toRemove = GetPlanningForATruck(day, truckID);
            TruckPlanning.Remove(toRemove);
            foreach (Route route in toRemove.Item3)
            {
                foreach (Order order in route.Orders)
                    this.RemovePlannedOccurrence(order.OrderNumber, route);
                AvailableRoutes.Add(route);
            }
        }

        public Tuple<Days, int, List<Route>> GetPlanningForATruck(Days day, int truckID)
        {
            return this.TruckPlanning.Find(t => t.Item1 == day && t.Item2 == truckID);
        }

        public Tuple<Days, int, List<Route>> GetRandomPlanning()
        {
            int random = new Random().Next(TruckPlanning.Count);
            return this.TruckPlanning[random];
        }

        public Tuple<Days, int, List<Route>> GetPlanningForRoute(List<Route> route)
        {
            return this.TruckPlanning.Find(t => t.Item3 == route);
        }

        public List<Tuple<Days, int, List<Route>>> GetEntirePlanning()
        {
            return this.TruckPlanning;
        }

        public void ClearTruckPlanning()
        {
            foreach (OrderTracker oc in CounterList)
                oc.PlannedDayOccurences = Days.None;

            foreach (Tuple<Days, int, List<Route>> t in TruckPlanning)
                foreach (Route route in t.Item3)
                    if (!AvailableRoutes.Contains(route))
                        AvailableRoutes.Add(route);

            this.TruckPlanning.Clear();
        }

        public void UpdateOrdersCounterAfterAdding(List<Route> addedRoutes)
        {
            foreach (Route route in addedRoutes)
                foreach (Order order in route.Orders)
                    if (order.OrderNumber != 0)
                        AddOrderOccurrence(order.OrderNumber, route);
        }

        public void UpdateOrdersCounterAfterRemoval(List<Route> removedRoutes)
        {
            foreach (Route route in removedRoutes)
                foreach (Order order in route.Orders)
                    RemoveOrderOccurrence(order.OrderNumber, route);
        }

        public void ClearAllOccurences() // TODO: How to deal with routes containing the order?
        {
            CounterList.Clear();
            CounterList = new List<OrderTracker>();
        }

        public void AddOrderOccurrence(int OrderNumber, Route OccurredIn)
        {
            OrderTracker counter = this.FindOrderTracker(OrderNumber);
            counter.OrderDayOccurrences |= OccurredIn.Day;
            counter.UpdateOrderDayRestrictions();
            if (!counter.PartOfRoutes.Contains(OccurredIn))
                counter.PartOfRoutes.Add(OccurredIn);
        }

        public void AddPlannedOccurrence(int OrderNumber, Route OccurredIn)
        {
            OrderTracker counter = this.FindOrderTracker(OrderNumber);
            counter.PlannedDayOccurences |= OccurredIn.Day;
        }

        public void RemovePlannedOccurrence(int OrderNumber, Route OccurredIn)
        {
            OrderTracker counter = this.FindOrderTracker(OrderNumber);
            counter.PlannedDayOccurences ^= (counter.OrderDayOccurrences & OccurredIn.Day);
        }

        public void RemoveOrderOccurrence(int OrderNumber, Route OccurredIn)
        {
            OrderTracker counter = this.FindOrderTracker(OrderNumber);
            counter.OrderDayOccurrences ^= (counter.OrderDayOccurrences & OccurredIn.Day);
            counter.UpdateOrderDayRestrictions();
            counter.PartOfRoutes.Remove(OccurredIn);
        }

        public bool CanAddOrder(int OrderNumber, Days day)
        {
            return this.FindOrderTracker(OrderNumber).CanAddOrder(day);
        }

        public Boolean OrderHasOccurence(Days day, int OrderNumber)
        {
            return CounterList.Find(o => o.OrderNumber == OrderNumber && o.OrderDayOccurrences.HasFlag(day)) != null;
        }

        public Boolean OrderHasPlannedOccurence(Days day, int OrderNumber)
        {
            return CounterList.Find(o => o.OrderNumber == OrderNumber && o.PlannedDayOccurences.HasFlag(day)) != null;
        }

        public Boolean AllOrdersCompleted()
        {
            foreach (OrderTracker counter in CounterList)
                if (!counter.IsCompleted())
                    return false;

            return true;
        }

        public Boolean IsOrderCompleted(int OrderNumber)
        {
            foreach (OrderTracker counter in CounterList)
                if (counter.OrderNumber == OrderNumber)
                    return counter.IsCompleted();

            return false;
        }

        private OrderTracker FindOrderTracker(int OrderNumber)
        {
            if (!CounterList.Exists(o => o.OrderNumber == OrderNumber))
                CounterList.Add(new OrderTracker(OrderNumber, new List<Days>(Data.Orders[OrderNumber].DayRestrictions)));

            return CounterList.Find(o => o.OrderNumber == OrderNumber); // CounterList.Add states that it adds to end, maybe return [Count - 1]?
        }

        public double GetSolutionScore() // TODO: Maybe start working with delta's instead of recalculating everytime
        {
            double travelTime = 0.0d;
            double penaltyTime = 0.0d;

            List<int> uncompleteOrders = new List<int>();
            for (int i = 0; i < CounterList.Count; i++)
            {
                if (!CounterList[i].IsCompleted())
                {
                    int orderNumber = CounterList[i].OrderNumber;
                    if (uncompleteOrders.Contains(orderNumber)) // Has already been punished
                        continue;
                    else
                        uncompleteOrders.Add(orderNumber);

                    penaltyTime += Data.Orders[orderNumber].PenaltyTime;
                    break;
                }
            }

            foreach (Tuple<Days, int, List<Route>> tuple in TruckPlanning)
                foreach (Route route in tuple.Item3)
                    travelTime += route.TravelTime;

            return travelTime + penaltyTime;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            foreach (Tuple<Days, int, List<Route>> tuple in TruckPlanning)
            {
                List<Route> routes = tuple.Item3;
                int sequenceID = 0;
                for (int routeID = 0; routeID < routes.Count; routeID++)
                    for (int orderID = 0; orderID < routes[routeID].Orders.Count; orderID++)
                        sb.AppendLine(String.Format("{0};{1};{2};{3}", tuple.Item2 + 1, (int)DayInt(tuple.Item1), ++sequenceID, routes[routeID].Orders[orderID].OrderNumber));
            }

            return sb.ToString();
        }

        private int DayInt(Days day)
        {
            switch (day)
            {
                case Days.Monday:
                    return 1;
                case Days.Tuesday:
                    return 2;
                case Days.Wednesday:
                    return 3;
                case Days.Thursday:
                    return 4;
                case Days.Friday:
                    return 5;
                default:
                    return -1;
            }
        }
    }
}