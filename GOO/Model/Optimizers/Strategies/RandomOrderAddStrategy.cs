﻿using System;
using System.Collections.Generic;

using GOO.Utilities;

namespace GOO.Model.Optimizers.Strategies
{
    public class RandomOrderAddStrategy : Strategy
    {
        private Days day;
        private int truck;
        private Route old_route;
        private Route new_route;
        private List<Route> RoutesFromSolution;

        public RandomOrderAddStrategy() : base()
        {

        }

        public override Solution executeStrategy(Solution toStartFrom)
        {
            Tuple<Days, int, List<Route>> Planning = toStartFrom.GetRandomPlanning();
            day = Planning.Item1;
            truck = Planning.Item2;
            RoutesFromSolution = Planning.Item3;
            
            for (int planningCounter = 0; planningCounter < 5; planningCounter++)
            {
                Planning = toStartFrom.GetRandomPlanning();
                RoutesFromSolution = Planning.Item3;
                old_route = RoutesFromSolution[random.Next(RoutesFromSolution.Count)];

                for (int counter = 0; counter < 5 && old_route.Orders.Count < 3; counter++)
                    old_route = RoutesFromSolution[random.Next(RoutesFromSolution.Count)];

                if (old_route.Orders.Count >= 3)
                    break;
            }

            //copy the begin route for the 2-opt and create the route for the 2-opt check
            new_route = new Route(day);
            foreach (Order order in old_route.Orders)
                new_route.AddOrder(order);

            //start removing a random order
            if (new_route.Orders.Count >= 2) //fix needed as well?
            {
                int ordertoAddAfter = random.Next(new_route.Orders.Count - 2);
                Order afterOrder = new_route.Orders[ordertoAddAfter];
                Order order = new_route.Orders[ordertoAddAfter]; //TODO: Get a right order numbahhhh
                int neworder = order.OrderNumber;

                if (afterOrder.CanBeAddedOnDay(new_route.Day) == true)
                    new_route.AddOrderAt(afterOrder, order);  //TODO: remove order form orderlist
            }

            RoutesFromSolution.Remove(old_route);
            RoutesFromSolution.Add(new_route);

            toStartFrom.RemoveItemFromPlanning(day, truck);
            toStartFrom.AddNewItemToPlanning(day, truck, RoutesFromSolution);

            return toStartFrom;
        }

        public override Solution undoStrategy(Solution toStartFrom)
        {
            // Nette manier zou zijn om de tuple weer op te halen gebaseerd op truck en dag.
            RoutesFromSolution.Remove(new_route);
            RoutesFromSolution.Add(old_route);

            toStartFrom.RemoveItemFromPlanning(day, truck);
            toStartFrom.AddNewItemToPlanning(day, truck, RoutesFromSolution);

            return toStartFrom;
        }
    }
}