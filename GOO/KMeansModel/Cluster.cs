using System;
using System.Collections.Generic;
using System.Windows;

using GOO.Model;
using GOO.Utilities;
using System.Text;

namespace GOO.KMeansModel
{
    public class Cluster
    {
        private Point point;
        public Point centerPoint // Could probably not use a property.
        {
            get
            {
                return point;
            }
            set
            {
                point = value;
            }
        }

        public List<Order> ordersInCluster;

        public Cluster(Point centerPoint)
            : this(centerPoint, new List<Order>())
        {
        }

        public Cluster(Point centerPoint, List<Order> ordersInCluster)
        {
            this.centerPoint = centerPoint;
            this.ordersInCluster = ordersInCluster;
        }

        public bool ReCenterPoint()
        {
            double newX = 0;
            double newY = 0;
            foreach (Order order in ordersInCluster)
            {
                newX += order.X;
                newY += order.Y;
            }
            newX = newX / ordersInCluster.Count;
            newY = newY / ordersInCluster.Count;
            if (newX != point.X || newY != point.Y)
            {
                this.point = new Point(newX, newY);
                return true;
            }

            return false;
        }

        public void AddOrderToCluster(Order toAdd)
        {
            ordersInCluster.Add(toAdd);
        }

        public void RemoveOrderFromCluster(Order toRemove)
        {
            ordersInCluster.Remove(toRemove);
        }

        public void RemoveAllOrdersFromCluster()
        {
            ordersInCluster.Clear();
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("CenterPoint = " + point.ToString());
            builder.AppendLine("Orders = {");
            //foreach (Order order in ordersInCluster)
            //{
            //    builder.AppendLine(order.ToString());
            //}
            builder.AppendLine("" + ordersInCluster.Count);
            builder.AppendLine("}");
            builder.AppendLine("");
            return builder.ToString();
        }

        public string ToRouteString(int truckNr, int dayNr)
        {
            StringBuilder builder = new StringBuilder();

            int seqNr = 1;
            foreach (Order order in ordersInCluster)
                builder.AppendLine(String.Format("{0};{1};{2};{3}", truckNr, dayNr, seqNr++, order.OrderNumber));

            builder.AppendLine(String.Format("{0};{1};{2};0", truckNr, dayNr, seqNr));
            return builder.ToString();
        }
    }
}
