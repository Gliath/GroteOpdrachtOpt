using System;
using System.Collections.Generic;

namespace GOO.Utilities
{
    public class Distances
    {
        public DistanceMatrixItem[,] Matrix { get; private set; }

        // Temporarily to assemble the matrix (take the red pill)
        private int MatrixCounter;
        private int MaximumMatrixID;
        private List<int> MatrixItemsTo;
        private List<int> MatrixItemsFrom;
        private List<DistanceMatrixItem> MatrixItemsInfo;

        public Distances()
        {
            // Will be disposed after assembling the matrix
            MatrixCounter = 0;
            MaximumMatrixID = 0;
            MatrixItemsTo = new List<int>();
            MatrixItemsFrom = new List<int>();
            MatrixItemsInfo = new List<DistanceMatrixItem>();
        }

        public void AddMatrixItem(int MatrixIDFrom, int MatrixIDTo, DistanceMatrixItem MatrixItem)
        {
            // Get the highest MatrixID for later
            MaximumMatrixID = ((MaximumMatrixID < MatrixIDFrom) ? MatrixIDFrom : MaximumMatrixID);
            MaximumMatrixID = ((MaximumMatrixID < MatrixIDTo) ? MatrixIDTo : MaximumMatrixID);

            MatrixItemsFrom.Add(MatrixIDFrom);
            MatrixItemsTo.Add(MatrixIDTo);
            MatrixItemsInfo.Add(MatrixItem);

            MatrixCounter++;
        }

        public void AssembleMatrix()
        {
            Matrix = new DistanceMatrixItem[MaximumMatrixID, MaximumMatrixID];

            for (int i = 0; i < MatrixCounter; i++)
                Matrix[MatrixItemsFrom[i], MatrixItemsTo[i]] = MatrixItemsInfo[i];

            // Dispose temporarily objects
            MaximumMatrixID = 0; // Disposes it (?) 
            MatrixCounter = 0;
            MatrixItemsFrom.Clear();
            MatrixItemsTo.Clear();
            MatrixItemsInfo.Clear();
            MatrixItemsFrom = null;
            MatrixItemsTo = null;
            MatrixItemsInfo = null;
        }
    }
}