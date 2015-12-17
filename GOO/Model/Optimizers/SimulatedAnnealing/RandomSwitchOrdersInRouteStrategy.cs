using System;
using System.Collections.Generic;

using GOO.Utilities;

namespace GOO.Model.Optimizers.SimulatedAnnealing
{
    public class RandomSwitchOrdersInRouteStrategy : Strategy
    {   

        public override Solution executeStrategy(Solution toStartFrom)
        {
            Day[] days = toStartFrom.GetRoutes();
            int day = random.Next(days.Length);
            Day randomDay = days[day];
            List<Route> randomRoutes = DeepCopy<List<Route>>.CopyFrom(randomDay.GetRoutes(random.Next(Day.NUMBER_OF_TRUCKS)));
            int route = random.Next(randomRoutes.Count);
            Route randomRoute = randomRoutes[route];

            List<Order> orders = DeepCopy<List<Order>>.CopyFrom(randomRoute.Orders);
            int ordersLength = orders.Count;
            int orderIndex1 = random.Next(ordersLength);
            int orderIndex2 = -1;
            do
            {
                orderIndex2 = random.Next(ordersLength);
            } while (orderIndex2 >= 0 && orderIndex2 != orderIndex1);

            Order toSwitch = orders[orderIndex1];
            orders[orderIndex1] = orders[orderIndex2];
            orders[orderIndex2] = toSwitch;

            return base.executeStrategy(toStartFrom);
        }
    }
}
