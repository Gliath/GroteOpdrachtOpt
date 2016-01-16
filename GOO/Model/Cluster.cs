﻿using System;
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
        }

        //public bool canbeplannedon(days day)
        //{
        //    for (int i = 0; i < ordersincluster.count; i++)
        //        //if (!orderscounter.canaddorder(ordersincluster[i].ordernumber, day)) /* ask solution */
        //            return false;

        //    return true;
        //}

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