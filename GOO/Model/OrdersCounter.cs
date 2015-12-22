using System;
using System.Collections.Generic;

using GOO.Utilities;

namespace GOO.Model
{
    public class OrdersCounter
    {
        public List<OrderCounter> CounterList { get; set; }

        public OrdersCounter()
        {
            CounterList = new List<OrderCounter>();
        }

        public void ClearAllOccurences()
        {
            CounterList.Clear();
            CounterList = new List<OrderCounter>();
        }

        public void AddOccurrence(int OrderNumber, Days OccurredOn)
        {
            if(!CounterList.Exists(o => o.OrderNumber == OrderNumber))
            {
                CounterList.Add(new OrderCounter(OrderNumber, new List<Days>(FilesInitializer._Orders[OrderNumber].DayRestrictions)));
            }

            foreach (OrderCounter order in CounterList)
                if (order.OrderNumber == OrderNumber)
                {
                    if ((order.OrderDayOccurrences & OccurredOn) == Days.None)
                    {
                        #if DEBUG
                        //Console.WriteLine("Order {0} has already occurred on {1}", OrderNumber, OccurredOn);
                        #endif
                    }

                    order.OrderDayOccurrences |= OccurredOn;
                    UpdateDayRestrictions(order);
                    break;
                }
        }

        public void RemoveOccurrence(int OrderNumber, Days OccurredOn)
        {
            foreach (OrderCounter order in CounterList)
                if (order.OrderNumber == OrderNumber)
                {
                    if (!((order.OrderDayOccurrences & OccurredOn) == Days.None))
                    {
                        #if DEBUG
                        //Console.WriteLine("Order {0} has yet to occur on {1}", OrderNumber, OccurredOn);
                        #endif
                    }

                    order.OrderDayOccurrences ^= (order.OrderDayOccurrences & OccurredOn);
                    UpdateDayRestrictions(order);
                    break;
                }

            CounterList.RemoveAll(o => o.OrderNumber == OrderNumber && o.OrderDayOccurrences.Equals(Days.None));
        }

        private void UpdateDayRestrictions(OrderCounter order)
        {
            order.OrderDayRestrictions.Clear();

            foreach (Days restrictions in FilesInitializer._Orders[order.OrderNumber].DayRestrictions)
                if (restrictions.HasFlag(order.OrderDayOccurrences))
                    order.OrderDayRestrictions.Add(restrictions);
        }

        public bool CanAddOrder(int orderNumber, Days day)
        {
            foreach (OrderCounter order in CounterList)
                if (order.OrderNumber == orderNumber)
                {
                    if (order.OrderDayOccurrences.HasFlag(day))
                        return false; // Already occurred on specified day

                    foreach (Days restrictions in order.OrderDayRestrictions)
                        if (restrictions.HasFlag(day))
                            return true; // Order has yet to occur on specified day and can be added accorrding to the restrictions

                    return false; // restrictions is either empty or does not have the specified day in its restriction
                }

            // Order has yet to be added to the list of CounterList
            foreach (Days restrictions in FilesInitializer._Orders[orderNumber].DayRestrictions)
                if (restrictions.HasFlag(day))
                    return true; // Order has never been added to this list, but according to the order.DayRestrictions it should be able to add to the specified day

            return false; // Could not be found in the CounterList or the orders restrictions, it is not supposed to be added this day
        }

        public Boolean HasOccurence(Days day, int OrderNumber)
        {
            return CounterList.Find(o => o.OrderNumber == OrderNumber && o.OrderDayOccurrences.HasFlag(day)) != null;
        }

        public Boolean IsCompleted()
        {
            foreach (OrderCounter order in CounterList)
                if (!order.IsCompleted())
                    return false;

            return true;
        }

        public Boolean IsOrderCompleted(int OrderNumber)
        {
            foreach (OrderCounter order in CounterList)
                if (order.OrderNumber == OrderNumber)
                    return order.IsCompleted();

            return false;
        }

        public class OrderCounter
        {
            public int OrderNumber { get; private set; }
            public List<Days> OrderDayRestrictions { get; private set; }
            public Days OrderDayOccurrences { get; set; }

            public OrderCounter(int OrderNumber, List<Days> OrderDayRestrictions)
            {
                this.OrderNumber = OrderNumber;
                this.OrderDayRestrictions = OrderDayRestrictions;

                OrderDayOccurrences = Days.None;
            }

            public Boolean IsCompleted()
            {
                #if DEBUG // Debug, test if order occurrs to many times
                //if (OrderDayOccurrences > OrderDayRestrictions) // TODO FIXZ THIS DEBUG CODE
                //    Console.WriteLine("Order {0} has occurred to many times, {1}/{2} times", OrderNumber, OrderDayOccurrences, OrderDayRestrictions);
                #endif

                foreach (Days restrictions in OrderDayRestrictions)
                    if(OrderDayOccurrences.Equals(restrictions))
                        return true;

                return false;
            }
        }
    }
}