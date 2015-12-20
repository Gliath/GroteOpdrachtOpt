using System;

namespace GOO.Utilities
{
    [Flags]
    public enum Days
    {
        None        = 0, // 00000
        Monday      = 1, // 00001
        Tuesday     = 2, // 00010
        Wednesday   = 4, // 00100
        Thursday    = 8, // 01000
        Friday      = 16 // 10000
    }
}