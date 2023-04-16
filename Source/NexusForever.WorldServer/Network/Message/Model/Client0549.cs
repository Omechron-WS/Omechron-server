using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message
{
    [Message(GameMessageOpcode.Client0549)]
    public class Client0549 : IReadable
    {
        public ulong Unknown0 { get; private set; }

        public void Read(GamePacketReader reader)
        {
            Unknown0 = reader.ReadULong();
        }
    }
}
