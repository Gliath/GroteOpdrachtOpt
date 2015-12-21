using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;

using GOO.Model;

namespace GOO.Utilities
{
    public static class FilesInitializer
    {
        private static String DistanceMatrix { get { return GOO.Properties.Resources.AfstandenMatrix; } }
        private static String Orders { get { return GOO.Properties.Resources.Orderbestand; } }

        public static Distances _DistanceMatrix { get; private set; }
        public static Dictionary<int, Order> _Orders { get; private set; }
        private static Order order0;

        public static void InitializeFiles()
        {
            Task DistanceInitializer = Task.Factory.StartNew(() => InitializeDistanceMatrix());
            Task OrdersInitializer = Task.Factory.StartNew(() => InitializeOrders());

            Task.WaitAll(DistanceInitializer, OrdersInitializer);
        }

        private static void InitializeDistanceMatrix()
        {
            string[] lines = DistanceMatrix.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            _DistanceMatrix = new Distances();

            for (int i = 1; i < lines.Length; i++) // Skip the first line
            {
                string[] variables = lines[i].Split(';');

                int MatrixIDFrom = int.Parse(variables[0].Trim());
                int MatrixIDTo = int.Parse(variables[1].Trim());
                DistanceMatrixItem MatrixInfo = new DistanceMatrixItem(int.Parse(variables[2].Trim()), int.Parse(variables[3].Trim()));

                _DistanceMatrix.AddMatrixItem(MatrixIDFrom, MatrixIDTo, MatrixInfo);
            }

            _DistanceMatrix.AssembleMatrix();
        }

        private static void InitializeOrders()
        {
            string[] lines = Orders.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            _Orders = new Dictionary<int, Order>();

            for (int i = 1; i < lines.Length; i++) // Skip the first line
            {
                string[] variables = lines[i].Split(';');

                int OrderNumber = int.Parse(variables[0].Trim());
                string Place = variables[1].Trim();
                OrderFrequency Frequency;
                switch (variables[2].Trim())
                {
                    case "1PWK":
                        Frequency = OrderFrequency.PWK1;
                        break;
                    case "2PWK":
                        Frequency = OrderFrequency.PWK2;
                        break;
                    case "3PWK":
                        Frequency = OrderFrequency.PWK3;
                        break;
                    case "4PWK":
                        Frequency = OrderFrequency.PWK4;
                        break;
                    case "5PWK":
                        Frequency = OrderFrequency.PWK5;
                        break;
                    default:
                        Frequency = OrderFrequency.PWK1;
                        break;
                }

                int NumberOfContainers = int.Parse(variables[3].Trim());
                int VolumePerContainer = int.Parse(variables[4].Trim());
                double EmptyingTimeInSeconds = double.Parse(variables[5].Trim(), CultureInfo.InvariantCulture) * 60.0d;
                int MatrixID = int.Parse(variables[6].Trim());
                int X = int.Parse(variables[7].Trim());
                int Y = int.Parse(variables[8].Trim());

                _Orders.Add(OrderNumber, new Order(OrderNumber, Place, Frequency, NumberOfContainers, VolumePerContainer, EmptyingTimeInSeconds, MatrixID, X, Y));
            }
        }

        public static Order GetOrder0()
        {
            if(order0 == null)
                order0 = new Order(0, "MAARHEEZE", OrderFrequency.PWK5, 0, 0, 0.0d, 287, 0, 0);

            return order0;
        }
    }
}