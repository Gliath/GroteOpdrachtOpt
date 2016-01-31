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
        public Days initialRestrictions { get; set; }

        public override Days DaysPlannedFor { get; set; }
        public override List<Order> OrdersInCluster { get; set; }
        public override List<Route> Routes { get; set; }

        public Cluster(Point centerPoint)
            : this(centerPoint, new List<Order>(), Days.None)
        { }

        public Cluster(Point centerPoint, List<Order> ordersInCluster, Days daysPlannedFor)
        {
            this.CenterPoint = centerPoint;
            this.OrdersInCluster = ordersInCluster;
            this.DaysPlannedFor = daysPlannedFor;
            this.Routes = new List<Route>();

            foreach (Order order in ordersInCluster)
                order.PutOrderInCluster(this);
        }

        public bool ReCenterPoint()
        {
            double newX = 0.0d;
            double newY = 0.0d;
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
            toAdd.PutOrderInCluster(this);
        }

        public void RemoveOrderFromCluster(Order toRemove)
        {
            toRemove.RemoveOrderFromCluster();
            OrdersInCluster.Remove(toRemove);
        }

        public void RemoveAllOrdersFromCluster()
        {
            foreach (Order order in OrdersInCluster)
                order.RemoveOrderFromCluster();

            OrdersInCluster.Clear();
        }

        public void AddRouteToCluster(Route toAdd)
        {
            Routes.Add(toAdd);
        }

        public void RemoveRouteFromCluster(Route toRemove)
        {
            Routes.Remove(toRemove);
        }

        public void RemoveAllRoutesFromCluster()
        {
            Routes.Clear();
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine(String.Format("Single Cluster: "));
            builder.AppendLine(String.Format("Days planned: {0}", DaysPlannedFor));
            builder.AppendLine(String.Format("Center Point: {0}", CenterPoint.ToString()));
            builder.AppendLine(String.Format("Number of Orders: {0}", OrdersInCluster.Count));
            builder.AppendLine(String.Format("Number of Routes: {0}", Routes.Count));
            builder.AppendLine("");
            return builder.ToString();
        }
    }
}