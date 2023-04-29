using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Network.Message.Model.Shared;
using System.Collections.Generic;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ServerHousingNeighbors)]
    public class ServerHousingNeighbors : IWritable
    {
        public class NeighborData : IWritable
        {
            public TargetPlayerIdentity roomie;
            //public ulong CharacterId;
            public byte RoommateType = 0;
            public void Write(GamePacketWriter writer)
            {
                // 238 bits long; 2 bits short of 30 bytes
                writer.Write((ulong)1); // 64
                writer.Write((ulong)1); // 128
                roomie.Write(writer); // 206
                writer.Write(RoommateType, 2); // 208
                writer.Write((ulong)0, 30); // 238
            }
        }

        public List<NeighborData> neighbors = new();

        public void Write(GamePacketWriter writer)
        {
            writer.Write(neighbors.Count);
            foreach (NeighborData neighbor in neighbors)
            {
                neighbor.Write(writer);
            }
        }
    }
}
