using System;
using System.Collections.Generic;

using GOO.Utilities;

namespace GOO.Model
{
    public class OrderTracker
    {
        public int OrderNumber { get; private set; }
        public List<Days> OrderDayRestrictions { get; private set; }
        public Days OrderDayOccurrences { get; set; }
        public Days PlannedDayOccurences { get; set; }

        private Order Order;

        public OrderTracker(Order Order, List<Days> OrderDayRestrictions)
        {
            this.Order = Order;

            this.OrderNumber = Order.OrderNumber;
            this.OrderDayRestrictions = new List<Days>(OrderDayRestrictions);

            this.OrderDayOccurrences = Days.None;
        }

        public void AddOrderOccurrence(Route OccurredIn)
        {
            OrderDayOccurrences |= OccurredIn.Day;
            UpdateOrderDayRestrictions();
        }

        public void RemoveOrderOccurrence(Route OccurredIn)
        {
            OrderDayOccurrences ^= (OrderDayOccurrences & OccurredIn.Day);
            UpdateOrderDayRestrictions();
        }

        public Boolean IsCompleted()
        {
            foreach (Days restrictions in OrderDayRestrictions)
                if (!PlannedDayOccurences.HasFlag(restrictions))
                    return false;

            return true;
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

        public double CurrentPenalty()
        {
            if (IsCompleted())
                return 0.0d;
            return Order.PenaltyTime;
        }
    }
}
