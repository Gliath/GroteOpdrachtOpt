using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

using GOO.Utilities;

namespace GOO.Model
{
    public class Cluster
    {
        public Point CenterPoint { get; private set; }
        public List<Order> OrdersInCluster { get; private set; }
        public List<Days> DaysRestrictions { get; set; }

        public Cluster(Point centerPoint)
            : this(centerPoint, new List<Order>())
        {}

        public Cluster(Point centerPoint, List<Order> ordersInCluster)
        {
            this.CenterPoint = centerPoint;
            this.OrdersInCluster = ordersInCluster;
        }

        public bool ReCenterPoint()
        {
            double newX = 0;
            double newY = 0;
            foreach (Order order in OrdersInCluster)
            {
                newX += order.X;
                newY += order.Y;
            }
            newX = newX / OrdersInCluster.Count;
            newY = newY / OrdersInCluster.Count;
            if (newX != CenterPoint.X || newY != CenterPoint.Y)
            {
                this.CenterPoint = new Point(newX, newY);
                return true;
            }

            return false;
        }

        public void AddOrderToCluster(Order toAdd)
        {
            OrdersInCluster.Add(toAdd);
        }

        public void RemoveOrderFromCluster(Order toRemove)
        {
            OrdersInCluster.Remove(toRemove);
        }

        public void RemoveAllOrdersFromCluster()
        {
            OrdersInCluster.Clear();
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("CenterPoint = " + CenterPoint.ToString());
            //builder.AppendLine("Orders = {");
            //foreach (Order order in ordersInCluster)
            //{
            //    builder.AppendLine(order.ToString());
            //}
            //builder.AppendLine("}");
            builder.AppendLine("Number of Orders: " + OrdersInCluster.Count);
            builder.AppendLine("");
            return builder.ToString();
        }

        public string ToRouteString(int truckNr, int dayNr)
        {
            StringBuilder builder = new StringBuilder();

            int seqNr = 1;
            foreach (Order order in OrdersInCluster)
                builder.AppendLine(String.Format("{0};{1};{2};{3}", truckNr, dayNr, seqNr++, order.OrderNumber));

            builder.AppendLine(String.Format("{0};{1};{2};0", truckNr, dayNr, seqNr));
            return builder.ToString();
        }
    }
}
