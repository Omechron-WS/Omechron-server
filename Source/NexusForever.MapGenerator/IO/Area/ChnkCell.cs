using System;
using System.Collections.Generic;
using System.IO;
using NexusForever.Shared.IO;

namespace NexusForever.MapGenerator.IO.Area
{
    public class ChnkCell : IReadable
    {
        // data size for each flag
        private static readonly int[] dataSize =
        {
            722,
            16,
            8450,
            8450,
            8450,
            4,
            64,
            16,
            4225,
            2178,
            4,
            578,
            1,
            4624,
            2312,
            8450,
            4096,
            2312,
            2312,
            2312,
            2312,
            1,
            16,
            16900,
            8,
            8450,
            21316,
            4096,
            16,
            8450,
            8450,
            2312
        };

        public uint X { get; }
        public uint Y { get; }
        public ChnkCellFlags Flags { get; private set; }

        public uint[] WorldZoneIds { get; } = new uint[4];
        public ZoneBoundFlags[,] WorldZoneBounds { get; } = new ZoneBoundFlags[64, 64];
        public ushort[,] Heightmap { get; } = new ushort[19, 19];
        public uint[] WorldLayers { get; private set; } = new uint[4];

        public HashSet<IReadable> Chunks { get; } = new();

        public ChnkCell(uint x, uint y)
        {
            X = x;
            Y = y;
        }

        public void Read(BinaryReader reader)
        {
            Flags = (ChnkCellFlags)reader.ReadUInt32();

            for (int i = 0; i < 32; i++)
            {
                ChnkCellFlags flag = (ChnkCellFlags)(1 << i);
                if ((Flags & flag) == 0)
                    continue;

                switch (flag)
                {
                    case ChnkCellFlags.HeightMap:
                        {
                            for (int y = 0; y < 19; y++)
                                for (int x = 0; x < 19; x++)
                                    Heightmap[x, y] = reader.ReadUInt16();
                            break;
                        }
                    case ChnkCellFlags.WorldLayers:
                        {
                            for(int l = 0; l < 4; l++)
                            {
                                WorldLayers[i] = reader.ReadUInt32();
                            }
                            break;
                        }
                    case ChnkCellFlags.Unk7:
                        {
                            byte[] data = reader.ReadBytes(dataSize[i]); // unknown, 16 bytes, possibly a matrix?
                            break;
                        }
                    case ChnkCellFlags.Unk13:
                        {
                            ushort[,,] data = new ushort[17, 17, 8];
                            for (int y = 0; y < 17; y++)
                            {
                                for(int x = 0; x < 17; x++)
                                {
                                    for (int l = 0; l < 8; l++)
                                    {
                                        data[x, y, l] = reader.ReadUInt16();
                                    }
                                }
                            }
                            break;
                        }
                    case ChnkCellFlags.ZoneBound:
                        {
                        for (int y = 0; y < 64; y++)
                            for (int x = 0; x < 64; x++)
                                WorldZoneBounds[x, y] = (ZoneBoundFlags)reader.ReadByte();
                        break;
                        }
                    case ChnkCellFlags.Unk26:
                        {
                            byte[,,] data = new byte[73, 73, 4];
                            for (int y = 0; y < 73; y++)
                            {
                                for (int x = 0; x < 73; x++)
                                {
                                    for (int l = 0; l < 4; l++)
                                    {
                                        data[x, y, l] = reader.ReadByte();
                                        // Each value is highly correlated.
                                        // First 0, 2 or 3 values are usually non-zero.
                                    }
                                }
                            }
                            break;
                        }
                    case ChnkCellFlags.Unk27:
                        {
                            byte[,] data = new byte[64, 64];
                            for (int y = 0; y < 64; y++)
                            {
                                for (int x = 0; x < 64; x++)
                                {
                                    // Often 0, 4, 16 or 20. Flags?
                                    // 4 or 16 often correlate to road march, but not always. 64 and 128 show up, too.
                                    data[x, y] = reader.ReadByte();
                                }
                            }
                            break;
                        }
                    case ChnkCellFlags.Zone:
                        {
                            for (int j = 0; j < 4; j++)
                                WorldZoneIds[j] = reader.ReadUInt32();
                            break;
                        }
                    // unhandled flag, skip data
                    default:
                        {
                            int z = dataSize[i];
                            uint f = (uint)flag;
                            uint f2 = (uint)ChnkCellFlags.Unk13;
                            byte[] data = reader.ReadBytes(dataSize[i]);
                            if(i != 6 && i != 17 && i != 18 && i != 21 && i != 27)
                            {
                                int a = i;
                            }
                            break;
                        }
                }
            }

            // cells can have trailing sub chunks
            while (reader.BaseStream.Position < reader.BaseStream.Length)
            {
                uint chunkMagic  = reader.ReadUInt32();
                uint chunkSize   = reader.ReadUInt32();
                byte[] chunkData = reader.ReadBytes((int)chunkSize);

                IReadable chunk;
                switch (chunkMagic)
                {
                    case 0x77627350:
                        chunk = new Wbsp();
                        break;
                    case 0x63757244:
                        chunk = new Curd();
                        break;
                    case 0x50524F50:
                        chunk = new CellProp();
                        break;
                    case 0x57417447:
                        chunk = new Watg();
                        break;
                    default:
                        throw new InvalidDataException();
                }

                using (var chunkStream = new MemoryStream(chunkData))
                using (var chunkReader = new BinaryReader(chunkStream))
                {
                    chunk.Read(chunkReader);
                }

                Chunks.Add(chunk);
            }
        }
    }
}
