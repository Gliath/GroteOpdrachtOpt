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
                CounterList.Add(new OrderCounter(OrderNumber, FilesInitializer._Orders[OrderNumber].DayRestrictions));    
            }

            foreach (OrderCounter order in CounterList)
                if (order.OrderNumber == OrderNumber)
                {
                    if ((order.OrderDayOccurrences & OccurredOn) == Days.None)
                    {
                        #if DEBUG
                        Console.WriteLine("Order {0} has already occurred on {1}", OrderNumber, OccurredOn);
                        #endif
                    }

                    order.OrderDayOccurrences |= OccurredOn;
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
                        Console.WriteLine("Order {0} has yet to occur on {1}", OrderNumber, OccurredOn);
                        #endif
                    }

                    order.OrderDayOccurrences ^= (order.OrderDayOccurrences & OccurredOn);
                    break;
                }

            CounterList.RemoveAll(o => o.OrderNumber == OrderNumber && o.OrderDayOccurrences.Equals(Days.None));
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