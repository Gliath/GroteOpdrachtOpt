using System;
using System.Threading.Tasks;

namespace GOO.Utilities
{
    public static class FilesInitializer
    {
        private static String DistanceMatrix { get { return GOO.Properties.Resources.AfstandenMatrix; } }
        private static String Orders { get { return GOO.Properties.Resources.Orderbestand; } }

        public static void InitializeFiles()
        {
            Task DistanceInitializer = Task.Factory.StartNew(() => InitializeDistanceMatrix());
            Task OrdersInitializer = Task.Factory.StartNew(() => InitializeOrders());

            Task.WaitAll(DistanceInitializer, OrdersInitializer);
        }

        private static void InitializeDistanceMatrix()
        {
            // TODO
        }

        private static void InitializeOrders()
        {
            // TODO
        }
    }
}