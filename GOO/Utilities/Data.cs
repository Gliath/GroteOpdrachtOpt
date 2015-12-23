using System;
using System.Collections.Generic;

using GOO.Model;

namespace GOO.Utilities
{
    public static class Data
    {
        public static DistanceMatrixItem[,] DistanceMatrix { get; private set; }
        public static Dictionary<int, Order> Orders { get; private set; }

        private static Order order0 = new Order(0, "MAARHEEZE", OrderFrequency.PWK5, 0, 0, 0.0d, 287, 0, 0);

        // Temporarily needed to assemble the matrix
        private static int MatrixCounter = 0;
        private static int MaximumMatrixID = 0;
        private static List<int> MatrixItemsTo = new List<int>();
        private static List<int> MatrixItemsFrom = new List<int>();
        private static List<DistanceMatrixItem> MatrixItemsInfo = new List<DistanceMatrixItem>();

        public static void AddMatrixItem(int MatrixIDFrom, int MatrixIDTo, DistanceMatrixItem MatrixItem)
        {
            // Get the highest MatrixID for later
            MaximumMatrixID = ((MaximumMatrixID < MatrixIDFrom) ? MatrixIDFrom : MaximumMatrixID);
            MaximumMatrixID = ((MaximumMatrixID < MatrixIDTo) ? MatrixIDTo : MaximumMatrixID);

            MatrixItemsFrom.Add(MatrixIDFrom);
            MatrixItemsTo.Add(MatrixIDTo);
            MatrixItemsInfo.Add(MatrixItem);

            MatrixCounter++;
        }

        public static void AssembleMatrix()
        {
            MaximumMatrixID++; // Needs to be one higher
            DistanceMatrix = new DistanceMatrixItem[MaximumMatrixID, MaximumMatrixID];

            for (int i = 0; i < MatrixCounter; i++)
                DistanceMatrix[MatrixItemsFrom[i], MatrixItemsTo[i]] = MatrixItemsInfo[i];

            // Dispose temporarily objects (or the very least removes their memory usage)
            MaximumMatrixID = 0;
            MatrixCounter = 0;
            MatrixItemsFrom.Clear();
            MatrixItemsTo.Clear();
            MatrixItemsInfo.Clear();
            MatrixItemsFrom = null;
            MatrixItemsTo = null;
            MatrixItemsInfo = null;
        }

        public static void AddOrder(int OrderNumber, Order Order)
        {
            if (Orders == null)
                Orders = new Dictionary<int, Order>();

            Orders.Add(OrderNumber, Order);
        }

        public static Order GetOrder0()
        {
            return order0;
        }
    }
}