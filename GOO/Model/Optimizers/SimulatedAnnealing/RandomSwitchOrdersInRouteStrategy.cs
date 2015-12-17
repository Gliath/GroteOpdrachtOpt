using System;
using System.Collections.Generic;

using GOO.Utilities;

namespace GOO.Model.Optimizers.SimulatedAnnealing
{
    public class RandomSwitchOrdersInRouteStrategy : Strategy
    {   
        public RandomSwitchOrdersInRouteStrategy () : base(new Random()){}

        public override Solution executeStrategy(Solution toStartFrom)
        {
            Solution toReturn = toStartFrom.GetShallowCopy();

            Day[] days = toReturn.GetRoutes();
            int day = random.Next(days.Length);
            Day randomDay = days[day];
            int randomTruck = random.Next(Day.NUMBER_OF_TRUCKS);
            List<Route> randomRoutes = DeepCopy<List<Route>>.CopyFrom(randomDay.GetRoutes(randomTruck));
            int routeNumber = random.Next(randomRoutes.Count);
            Route randomRoute = randomRoutes[routeNumber];

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

            randomRoutes[routeNumber] = randomRoute;

            Day copyOfDay = new Day();
            copyOfDay.SetRoutes(randomTruck, randomRoutes);
            int otherTruckNumber = otherTruck(randomTruck);
            copyOfDay.SetRoutes(otherTruckNumber, randomDay.GetRoutes(otherTruckNumber));

            days[day] = copyOfDay;

            return toReturn;
        }

        private int otherTruck(int truckNumber)
        {
            if (truckNumber > 1)
                return 0;
            return 1;
        }
    }
}
