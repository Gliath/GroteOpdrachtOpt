using System;
using System.Collections.Generic;

namespace GOO.Model
{
    /// <summary>
    /// Part of a route for a day for a specific truck
    /// </summary>
    public class Route
    {
        // add variable to indicate for which day this route is meant for

        public List<Order> Orders { get; set; } // List of orders for this route
    }
}