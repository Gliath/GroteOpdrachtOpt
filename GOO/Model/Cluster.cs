using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

using GOO.Utilities;

namespace GOO.Model
{
    public class Cluster : AbstractCluster
    {
        public Point CenterPoint { get; private set; }
        public override List<Order> AvailableOrdersInCluster { get; set; }

        public Cluster(Point centerPoint)
            : this(centerPoint, new List<Order>(), Days.None)
        { }

        public Cluster(Point centerPoint, List<Order> ordersInCluster, Days daysPlannedFor)
        {
            this.CenterPoint = centerPoint;
            this.AvailableOrdersInCluster = ordersInCluster;

            foreach (Order order in ordersInCluster)
                order.PutInCluster(this);
        }

        public bool ReCenterPoint()
        {
            double newX = 0.0d;
            double newY = 0.0d;
            foreach (Order order in AvailableOrdersInCluster)
            {
                newX += order.X;
                newY += order.Y;
            }
            newX = newX / AvailableOrdersInCluster.Count;
            newY = newY / AvailableOrdersInCluster.Count;
            if (newX != CenterPoint.X || newY != CenterPoint.Y)
            {
                this.CenterPoint = new Point(newX, newY);
                return true;
            }

            return false;
        }

        public void AddOrderToCluster(Order toAdd)
        {
            AvailableOrdersInCluster.Add(toAdd);
            toAdd.PutInCluster(this);
        }

        public void RemoveOrderFromCluster(Order toRemove)
        {
            toRemove.RemoveAvailableOrderFromCluster();
            AvailableOrdersInCluster.Remove(toRemove);
        }

        public void RemoveAllOrdersFromCluster()
        {
            foreach (Order order in AvailableOrdersInCluster)
                order.RemoveCluster();

            AvailableOrdersInCluster.Clear();
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine(String.Format("Single Cluster: "));
            builder.AppendLine(String.Format("Center Point: {0}", CenterPoint.ToString()));
            builder.AppendLine(String.Format("Number of Orders: {0}", AvailableOrdersInCluster.Count));
            builder.AppendLine("");
            return builder.ToString();
        }
    }
}