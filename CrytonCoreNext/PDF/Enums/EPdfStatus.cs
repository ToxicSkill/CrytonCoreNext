using System;

namespace CrytonCoreNext.PDF.Enums
{
    [Flags]
    public enum EPdfStatus
    {
        Opened = 1 << 1,
        Protected = 1 << 2,
        Damaged = 1 << 3
    }
}
