using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

using GOO.Utilities;

namespace GOO.Model
{
    public class OrdersCounter
    {
        private static readonly List<OrderCounter> BasicEmptyCounterList;
        static OrdersCounter()
        {
            BasicEmptyCounterList = new List<OrderCounter>();
            foreach (Order order in FilesInitializer._Orders)
            {
                BasicEmptyCounterList.Add(new OrderCounter(order.OrderNumber, order.FrequencyNumber));
            }
        }

        private List<OrderCounter> counterList;

        public OrdersCounter()
        {
            counterList = BasicEmpty<List<OrderCounter>>.Copy(BasicEmptyCounterList);
        }

        private static class BasicEmpty<T>
        {
            public static T Copy(object objectToCopy)
            {
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    BinaryFormatter binaryFormatter = new BinaryFormatter();
                    binaryFormatter.Serialize(memoryStream, objectToCopy);
                    memoryStream.Seek(0, SeekOrigin.Begin);
                    return (T)binaryFormatter.Deserialize(memoryStream);
                }
            }
        }

        private class OrderCounter
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
                return OrderFrequency == OrderOccurrences;
            }
        }
    }
}