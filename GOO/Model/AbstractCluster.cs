using System;
using System.Collections.Generic;
using System.Text;

using GOO.Utilities;

namespace GOO.Model
{
    public abstract class AbstractCluster
    {
        public abstract List<Order> AvailableOrdersInCluster { get; set; }

        public string ToRouteString(int truckNr, int dayNr)
        {
            StringBuilder builder = new StringBuilder();

            int seqNr = 1;
            foreach (Order order in AvailableOrdersInCluster)
                builder.AppendLine(String.Format("{0};{1};{2};{3}", truckNr, dayNr, seqNr++, order.OrderNumber));

            builder.AppendLine(String.Format("{0};{1};{2};0", truckNr, dayNr, seqNr));
            return builder.ToString();
        }
    }
}