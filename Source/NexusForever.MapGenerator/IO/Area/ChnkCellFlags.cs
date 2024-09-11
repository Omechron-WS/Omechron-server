using System;

namespace NexusForever.MapGenerator.IO.Area
{
    [Flags]
    public enum ChnkCellFlags
    {
        HeightMap        = 0x00000001,
        WorldLayers      = 0x00000002,
        Unk7             = 0x00000080,
        Unk13            = 0x00002000,
        ZoneBound        = 0x00010000,
        Unk26            = 0x04000000,
        Unk27            = 0x08000000,
        Zone             = 0x10000000
    }
}
