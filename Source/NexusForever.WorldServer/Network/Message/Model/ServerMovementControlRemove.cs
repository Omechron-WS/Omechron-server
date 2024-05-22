using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ServerMoveemntControlRemove)]
    public class ServerMovementControlRemove : IWritable
    {
        public void Write(GamePacketWriter writer)
        {
        }
    }
}
