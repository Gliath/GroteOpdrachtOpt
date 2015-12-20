using System;
using System.Collections.Generic;

using GOO.Utilities;

namespace GOO.KMeansModel
{
    public class DayRestrictionFactory
    {
        public static List<Days> GetDayRestrictions(OrderFrequency frequency)
        {
            switch (frequency)
            {
                case OrderFrequency.PWK1:
                    return new List<Days>(new Days[] { Days.Monday, Days.Tuesday, Days.Wednesday, Days.Thursday, Days.Friday });
                case OrderFrequency.PWK2:
                    return new List<Days>(new Days[] { Days.Monday | Days.Thursday, Days.Tuesday | Days.Friday });
                case OrderFrequency.PWK3:
                    return new List<Days>(new Days[] { Days.Monday | Days.Wednesday | Days.Friday });
                case OrderFrequency.PWK4:
                    return new List<Days>(new Days[] { Days.Monday | Days.Tuesday | Days.Wednesday | Days.Thursday, Days.Monday | Days.Tuesday | Days.Wednesday | Days.Friday, Days.Monday | Days.Tuesday | Days.Thursday | Days.Friday, Days.Monday | Days.Wednesday | Days.Thursday | Days.Friday, Days.Tuesday | Days.Wednesday | Days.Thursday | Days.Friday });
                case OrderFrequency.PWK5:
                    return new List<Days>(new Days[] { Days.Monday | Days.Tuesday | Days.Wednesday | Days.Thursday | Days.Friday });
            }

            return null;
        }
    }
}