using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.InterClient;
using NexusForever.WorldServer.Game.Social.Static;
using NexusForever.WorldServer.Game.Social;
using NexusForever.WorldServer.Network.Message.Model;
using NLog;
using NexusForever.WorldServer.Game.InterClient.Static;

namespace NexusForever.WorldServer.Network.Message.Handler
{
    public static class InterClientCommsHandler
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        [MessageHandler(GameMessageOpcode.ClientIcCommJoin)]
        public static void HandleIcCommJoin(WorldSession session, ClientIcCommJoin icCommJoin)
        {
            InterClientChannel channel = GlobalInterClientManager.Instance.GetICChannel(icCommJoin.Type, icCommJoin.Channel, icCommJoin.GuildContext);
            if (channel == null)
            {
                channel = GlobalInterClientManager.Instance.CreateChatChannel(icCommJoin.Type, icCommJoin.Channel, icCommJoin.GuildContext);
            }

            IcCommJoinResult result = channel.CanJoin(session.Player);
            if (result == IcCommJoinResult.Join)
                channel.Join(session.Player.CharacterId);

            session.EnqueueMessage(new Server0548()
            {
                Unknown0    = (byte) channel.Type,
                Channel     = channel.Id,
                Unknown10   = channel.Name
            });
            session.EnqueueMessage(new ServerIcCommJoinResult()
            {
                Result = result,
                Type = channel.Type,
                Channel = channel.Name
            });
            session.EnqueueMessage(new Server054A()
            {
                Channel = channel.Id,
            });
            session.EnqueueMessage(new Server054C() // probably ServerIcCommMessageResult
            {
                Channel     = channel.Id,
                Unknown8    = 0, // Sequential; message ID
                UnknownC    = (byte) channel.Type
            });
            session.EnqueueMessage(new Server054D()
            {
                Channel     = channel.Id,
                Unknown8    = 0,
                UnknownC    = ""
            });
            session.EnqueueMessage(new ServerIcCommSendMessage()
            {
                Channel = channel.Id,
                Message = "",
                SenderName = ""
            });
        }

        [MessageHandler(GameMessageOpcode.ClientIcCommSendMessage)]
        public static void HandleIcCommSendMessage(WorldSession session, ClientIcCommSendMessage icCommSendMessage)
        {
            log.Trace("Sendmessage");
        }

        [MessageHandler(GameMessageOpcode.Client0549)]
        public static void HandleClient0549(WorldSession session, Client0549 client0549)
        {
            log.Trace("0549");
        }
    }
}
