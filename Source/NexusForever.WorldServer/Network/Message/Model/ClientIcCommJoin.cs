using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.InterClient.Static;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ClientIcCommJoin)]
    public class ClientIcCommJoin : IReadable
    {
        public IcCommChannelType Type { get; private set; }
        public ulong GuildContext { get; private set; } // to determine which guild to join a channel for. Possibly for circles too?
        public string Channel { get; private set; }

        public void Read(GamePacketReader reader)
        {
            Type            = reader.ReadEnum<IcCommChannelType>(3u);
            GuildContext    = reader.ReadULong();
            Channel         = reader.ReadWideString();
        }
    }
}
