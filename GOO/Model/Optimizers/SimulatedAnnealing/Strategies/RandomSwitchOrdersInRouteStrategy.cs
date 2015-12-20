using System;
using System.Collections.Generic;

using GOO.Utilities;

namespace GOO.Model.Optimizers.SimulatedAnnealing.Strategies
{
    public class RandomSwitchOrdersInRouteStrategy : Strategy
    {
        public RandomSwitchOrdersInRouteStrategy() : base() { }

        public override Solution executeStrategy(Solution toStartFrom)
        {
            Solution toReturn = toStartFrom.GetShallowCopy();

            Day randomDay = toReturn.GetDays()[random.Next(toReturn.GetDays().Length)];

            List<Route> randomRoutes = randomDay.GetRoutes(random.Next(Day.NUMBER_OF_TRUCKS));
            Route randomChosenRoute = randomRoutes[random.Next(randomRoutes.Count)];
            List<Order> orders = randomChosenRoute.Orders;

            int ordersLength = orders.Count;
            int orderIndex1 = random.Next(1, ordersLength);
            int orderIndex2 = -1;
            do
            {
                orderIndex2 = random.Next(1, ordersLength);
            } while (!(orderIndex2 >= 0 && orderIndex2 != orderIndex1));

            Order toSwitch = orders[orderIndex1];
            Order toSwitch2 = orders[orderIndex2];

            Order toAddAfter = orders[orderIndex1 - 1];
            Order toAddAfter2 = orders[orderIndex2 - 1];

            randomChosenRoute.RemoveOrder(toSwitch, toReturn.GetOrdersCounter());
            randomChosenRoute.RemoveOrder(toSwitch2, toReturn.GetOrdersCounter());
            randomChosenRoute.AddOrder(toSwitch, toAddAfter2, toReturn.GetOrdersCounter());           
            randomChosenRoute.AddOrder(toSwitch, toAddAfter, toReturn.GetOrdersCounter());

            return toReturn;
        }
    }
}