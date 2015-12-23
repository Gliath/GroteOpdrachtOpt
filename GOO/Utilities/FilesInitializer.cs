using System;
using System.Threading.Tasks;

using GOO.Model;

namespace GOO.Utilities
{
    public static class FilesInitializer
    {
        public static void InitializeFiles()
        {
            Task DistanceInitializer = Task.Factory.StartNew(() => InitializeDistanceMatrix());
            Task OrdersInitializer = Task.Factory.StartNew(() => InitializeOrders());

            Task.WaitAll(DistanceInitializer, OrdersInitializer);
        }

        private static void InitializeDistanceMatrix()
        {
            string[] lines = GOO.Properties.Resources.AfstandenMatrix.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

            for (int i = 1; i < lines.Length; i++) // Skip the first line
            {
                string[] variables = lines[i].Split(';');

                Data.AddMatrixItem(int.Parse(variables[0].Trim()), int.Parse(variables[1].Trim()), new DistanceMatrixItem(int.Parse(variables[2].Trim()), int.Parse(variables[3].Trim())));
            }

            Data.AssembleMatrix();
        }

        private static void InitializeOrders()
        {
            string[] lines = GOO.Properties.Resources.Orderbestand.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

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
                        Frequency = OrderFrequency.PWK5;
                        break;
                }

                int NumberOfContainers = int.Parse(variables[3].Trim());
                int VolumePerContainer = int.Parse(variables[4].Trim());
                double EmptyingTimeInSeconds = double.Parse(variables[5].Trim(), System.Globalization.CultureInfo.InvariantCulture) * 60.0d;
                int MatrixID = int.Parse(variables[6].Trim());
                int X = int.Parse(variables[7].Trim());
                int Y = int.Parse(variables[8].Trim());

                Data.AddOrder(OrderNumber, new Order(OrderNumber, Place, Frequency, NumberOfContainers, VolumePerContainer, EmptyingTimeInSeconds, MatrixID, X, Y));
            }
        }
    }
}