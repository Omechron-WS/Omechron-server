using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.Housing.Static;
using NexusForever.WorldServer.Network.Message.Model.Shared;
using System.Collections.Generic;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ServerHousingNeighborInvite)]
    public class ServerHousingNeighborInvite : IWritable
    {
        public TargetPlayerIdentity Sender = new TargetPlayerIdentity();
        public string SenderName;

        public void Write(GamePacketWriter writer)
        {
            Sender.Write(writer);
            writer.WriteStringWide(SenderName);
        }
    }
}
