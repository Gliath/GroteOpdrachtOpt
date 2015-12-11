using System;
using System.Collections.Generic;

namespace GOO.Model
{
    public static class RouteGenerator
    {
        /// <summary>
        /// This method generates a route depth-first
        /// </summary>
        /// <param name="maxDepth">The maximum depth it is going to search (distance away from current location)</param>
        /// <param name="maxDepthSteps">The maximum steps into the depth</param>
        /// <returns>The generated route</returns>
        public static Route GenerateRouteDF(int maxDepth, int maxDepthSteps)
        {
            Stack<Order> stack = new Stack<Order>();

            return null;
        }

        /// <summary>
        /// This method generates a route depth-first
        /// </summary>
        /// <param name="maxDepth">The maximum depth it is going to search (distance away from current location)</param>
        /// <param name="maxDepthSteps">The maximum steps into the depth</param>
        /// <param name="maxTime">The maximum time the route will take</param>
        /// <returns>The generated route</returns>
        public static Route GenerateRouteDF(int maxDepth, int maxDepthSteps, int maxTime)
        {
            Stack<Order> stack = new Stack<Order>();

            return null;
        }

        /// <summary>
        /// This method generates a route breath-first
        /// </summary>
        /// <param name="maxBreath">The maximum depth it is going to search (distance away from current location)</param>
        /// <param name="maxBreathSteps">The maximum steps into the breath</param>
        /// <returns>The generated route</returns>
        public static Route GenerateRouteBF(int maxBreath, int maxBreathSteps)
        {
            Queue<Order> queue = new Queue<Order>();

            return null;
        }

        /// <summary>
        /// This method generates a route breath-first
        /// </summary>
        /// <param name="maxBreath">The maximum depth it is going to search (distance away from current location)</param>
        /// <param name="maxBreathSteps">The maximum steps into the breath</param>
        /// <param name="maxBreathSteps">The maximum time the route will take</param>
        /// <returns>The generated route</returns>
        public static Route GenerateRouteBF(int maxBreath, int maxBreathSteps, int maxTime)
        {
            Queue<Order> queue = new Queue<Order>();

            return null;
        }
    }
}