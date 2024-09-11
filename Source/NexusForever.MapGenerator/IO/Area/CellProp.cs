using System;
using System.Collections.Generic;
using System.IO;
using NexusForever.Shared.IO;

namespace NexusForever.MapGenerator.IO.Area
{
    public class CellProp : IReadable
    {
        public List<uint> Props { get; } = new();

        private byte[] _data;

        public void Read(BinaryReader reader)
        {
            _data = reader.ReadBytes((int)reader.BaseStream.Length);
            string data = BitConverter.ToString(_data).Replace("-", "");
            reader.BaseStream.Position = 0;

            while (reader.BaseStream.Position < reader.BaseStream.Length)
            {
                uint prop = reader.ReadUInt32();
                Props.Add(prop);
            }
        }
    }
}