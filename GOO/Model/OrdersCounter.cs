using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

using GOO.Utilities;

namespace GOO.Model
{
    [Serializable]
    public class OrdersCounter
    {
        private static readonly List<OrderCounter> BasicEmptyCounterList;
        static OrdersCounter()
        {
            BasicEmptyCounterList = new List<OrderCounter>();
            foreach (Order order in FilesInitializer._Orders)
                BasicEmptyCounterList.Add(new OrderCounter(order.OrderNumber, order.FrequencyNumber));
        }

        public List<OrderCounter> CounterList { get; set; }

        public OrdersCounter()
        {
            CounterList = DeepCopy<List<OrderCounter>>.CopyFrom(BasicEmptyCounterList);
        }

        public void AddOccurrence(int OrderNumber)
        {
            foreach (OrderCounter order in CounterList)
                if (order.OrderNumber == OrderNumber)
                {
                    order.OrderOccurrences++;

                    #if DEBUG
                    if (order.OrderOccurrences > order.OrderFrequency)
                        Console.WriteLine("Order {0} has occurred to many times, {1}/{2} times", OrderNumber, order.OrderOccurrences, order.OrderFrequency);
                    #endif

                    break;
                }
        }

        public void RemoveOccurrence(int OrderNumber)
        {
            foreach (OrderCounter order in CounterList)
                if (order.OrderNumber == OrderNumber)
                {
                    order.OrderOccurrences--;

                    #if DEBUG
                    if (order.OrderOccurrences < 0)
                        Console.WriteLine("Order {0} has occurred negative amount of times, {1} times", OrderNumber, order.OrderOccurrences);
                    #endif

                    break;
                }
        }

        public Boolean IsCompleted()
        {
            foreach (OrderCounter order in CounterList)
                if (!order.IsOrderCompleted())
                    return false;

            return true;
        }

        [Serializable]
        public class OrderCounter
        {
            public int OrderNumber { get; private set; }
            public int OrderFrequency { get; private set; }
            public int OrderOccurrences { get; set; }

            public OrderCounter(int OrderNumber, int OrderFrequency)
            {
                this.OrderNumber = OrderNumber;
                this.OrderFrequency = OrderFrequency;

                OrderOccurrences = 0;
            }

            public Boolean IsOrderCompleted()
            {
                #if DEBUG // Debug, test if order occurrs to many times
                if (OrderOccurrences > OrderFrequency)
                    Console.WriteLine("Order {0} has occurred to many times, {1}/{2} times", OrderNumber, OrderOccurrences, OrderFrequency);
                #endif

                return OrderFrequency == OrderOccurrences;
            }
        }
    }
}