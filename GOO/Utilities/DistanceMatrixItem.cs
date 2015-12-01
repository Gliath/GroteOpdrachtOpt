namespace GOO.Utilities
{
    public class DistanceMatrixItem
    {
        public int Distance { get; private set; }
        public int TravelTime { get; private set; }

        public DistanceMatrixItem(int Distance, int TravelTime)
        {
            this.Distance = Distance;
            this.TravelTime = TravelTime;
        }
    }
}