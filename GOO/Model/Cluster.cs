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
        public override List<Order> OrdersInCluster { get; set; }
        public override Days DaysPlannedFor { get; set; }
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
        }

        public bool CanBePlannedOn(Days day)
        {
            for (int i = 0; i < OrdersInCluster.Count; i++)
                //if (!OrdersCounter.CanAddOrder(OrdersInCluster[i].OrderNumber, day)) /* Ask Solution */
                    return false;

            return true;
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

        public void AddRouteToCluster(Route toAdd)
        {
            Routes.Add(toAdd);

            //foreach (Order order in toAdd.Orders)
            //    OrdersCounter.AddOccurrence(order.OrderNumber, toAdd.Day); /* A job for Solution or the class who adds the orders themselves */
        }

        public void RemoveRouteFromCluster(Route toRemove)
        {
            Routes.Remove(toRemove);

            //foreach (Order order in toRemove.Orders)
            //    OrdersCounter.RemoveOccurrence(order.OrderNumber, toRemove.Day); /* A job for Solution or the class who removes the orders themselves */
        }

        public void RemoveAllRoutesFromCluster()
        {
            Routes.Clear();
            //OrdersCounter.ClearAllOccurences(); /* A job for Solution or the class who clears the orders themselves */
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