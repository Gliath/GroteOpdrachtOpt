using System;
using System.Collections.Generic;

using GOO.Utilities;

namespace GOO.Model
{
    public class OrderTracker
    {
        public int OrderNumber { get; private set; }
        public List<Days> OrderDayRestrictions { get; private set; }
        public List<Route> PartOfRoutes { get; private set; }
        public Days OrderDayOccurrences { get; set; }
        public Days PlannedDayOccurences { get; set; }

        public OrderTracker(int OrderNumber, List<Days> OrderDayRestrictions)
        {
            this.OrderNumber = OrderNumber;
            this.OrderDayRestrictions = OrderDayRestrictions;

            this.OrderDayOccurrences = Days.None;
            this.PartOfRoutes = new List<Route>();
        }

        public void AddOrderOccurrence(Route OccurredIn)
        {
            OrderDayOccurrences |= OccurredIn.Day;
            UpdateOrderDayRestrictions();
            if (!PartOfRoutes.Contains(OccurredIn))
                PartOfRoutes.Add(OccurredIn);
        }

        public void RemoveOrderOccurrence(Route OccurredIn)
        {
            OrderDayOccurrences ^= (OrderDayOccurrences & OccurredIn.Day);
            UpdateOrderDayRestrictions();
            PartOfRoutes.Remove(OccurredIn);
        }

        public Boolean IsCompleted()
        {
            foreach (Days restrictions in OrderDayRestrictions)
                if (PlannedDayOccurences.Equals(restrictions))
                    return true;

            return false;
        }

        public void UpdateOrderDayRestrictions()
        {
            OrderDayRestrictions.Clear();

            foreach (Days restrictions in Data.Orders[OrderNumber].DayRestrictions)
                if (restrictions.HasFlag(OrderDayOccurrences))
                    OrderDayRestrictions.Add(restrictions);
        }

        public bool CanAddOrder(Days day)
        {
            if (OrderDayOccurrences.HasFlag(day))
                return false; // Already occurred on specified day

            foreach (Days restrictions in OrderDayRestrictions)
                if (restrictions.HasFlag(day))
                    return true; // Order has yet to occur on specified day and can be added accorrding to the restrictions

            return false; // restrictions is either empty or does not have the specified day in its restriction
        }
    }
}
