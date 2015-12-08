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
        private List<int> MatrixItemsToRemove;

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
            MaximumMatrixID++; // Needs to be one higher
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

        public void QueueRemoveItem(int MatrixID)
        {
            if (MatrixItemsToRemove == null)
                MatrixItemsToRemove = new List<int>();

            MatrixItemsToRemove.Add(MatrixID);
        }

        public void RemoveQueuedItems()
        {
            if (MatrixItemsToRemove == null)
                return;

            if (MatrixItemsToRemove.Count == 0)
            {
                MatrixItemsToRemove = null; // Disposes it?
                return;
            }

            // There are items to remove
            for (int i = 0; i < Matrix.GetLength(0); i++)
            {
                Boolean RemoveRow = false;
                if (MatrixItemsToRemove.Contains(i)) // If MatrixIDFrom is the ID that needs to be removed ....
                    RemoveRow = true;

                for (int j = 0; j < Matrix.GetLength(1); j++)
                    if (RemoveRow || MatrixItemsToRemove.Contains(j)) // Or if MatrixIDTo is the ID that needs to be removed ...
                        Matrix[i, j] = null; // Dispose the informatiion of the specified location
            }
        }
    }
}