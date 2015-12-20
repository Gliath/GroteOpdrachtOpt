using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

using GOO.Utilities;

namespace GOO.Obsolete.Model
{
    [Serializable]
    public class OrdersCounter
    {
        private static List<OrderCounter> BasicEmptyCounterList;
        private static void GenerateBasicEmptyCounterList()
        {
            BasicEmptyCounterList = new List<OrderCounter>();
            foreach (Order order in FilesInitializer._Orders)
                if(order != null)
                    BasicEmptyCounterList.Add(new OrderCounter(order.OrderNumber, order.FrequencyNumber));
        }

        public List<OrderCounter> CounterList { get; set; }

        public OrdersCounter()
        {
            if (BasicEmptyCounterList == null)
                OrdersCounter.GenerateBasicEmptyCounterList();

            CounterList = DeepCopy<List<OrderCounter>>.CopyFrom(BasicEmptyCounterList);
        }

        public void ClearAllOccurences()
        {
            foreach (OrderCounter order in CounterList)
                order.OrderOccurrences = 0;
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

            public Boolean IsCompleted()
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