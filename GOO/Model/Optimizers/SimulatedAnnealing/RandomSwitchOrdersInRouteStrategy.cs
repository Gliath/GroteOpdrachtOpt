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

            Day randomDay = toReturn.GetRoutes()[random.Next(toReturn.GetRoutes().Length)];
            
            List<Route> randomRoutes = randomDay.GetRoutes(random.Next(Day.NUMBER_OF_TRUCKS));
            List<Order> orders = randomRoutes[random.Next(randomRoutes.Count)].Orders;

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
