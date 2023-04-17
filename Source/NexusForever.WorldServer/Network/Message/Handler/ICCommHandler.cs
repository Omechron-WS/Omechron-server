using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.InterClient;
using NexusForever.WorldServer.Network.Message.Model;
using NLog;
using NexusForever.WorldServer.Game.InterClient.Static;
using NexusForever.WorldServer.Game.CharacterCache;
using NexusForever.WorldServer.Game.Entity;

namespace NexusForever.WorldServer.Network.Message.Handler
{
    public static class ICCommHandler
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        [MessageHandler(GameMessageOpcode.ClientIcCommJoin)]
        public static void HandleIcCommJoin(WorldSession session, ClientIcCommJoin icCommJoin)
        {
            if(icCommJoin.Type != IcCommChannelType.Global) // not supporting guilds or parties right now.
            {
                session.EnqueueMessageEncrypted(new ServerIcCommJoinResult()
                {
                    Result = icCommJoin.Type == IcCommChannelType.Group ? IcCommJoinResult.NoGroup : IcCommJoinResult.NoGuild,
                    Type = icCommJoin.Type,
                    Channel = icCommJoin.Channel
                });
                return;
            }

            IcCommChannel channel = GlobalIcCommManager.Instance.GetICChannel(icCommJoin.Type, icCommJoin.Channel, icCommJoin.GuildContext);
            if (channel == null)
            {
                channel = GlobalIcCommManager.Instance.CreateChatChannel(icCommJoin.Type, icCommJoin.Channel, icCommJoin.GuildContext);
            }

            IcCommJoinResult result = channel.CanJoin(session.Player);
            if (result == IcCommJoinResult.Join)
            {
                channel.Join(session.Player.CharacterId);

                session.EnqueueMessageEncrypted(new ServerIcCommJoinSuccess() // Also acts as "onjoinresult"
                {
                    ChannelType = (byte)channel.Type,
                    ChannelID = channel.Id,
                    ChannelName = icCommJoin.Channel
                });
            }
            else
            {
                session.EnqueueMessageEncrypted(new ServerIcCommJoinResult() // Used to indicate a failure to join?
                {
                    Result = result,
                    Type = channel.Type,
                    Channel = channel.Name
                });
            }

            /*session.EnqueueMessageEncrypted(new Server054D()
            {
                Channel     = channel.Id,
                Unknown8    = 0, // message ID?
                UnknownC    = ""
            });*/
            /*session.EnqueueMessageEncrypted(new ServerIcCommSendMessage()
            {
                Channel = channel.Id,
                Message = "",
                SenderName = ""
            });*/
        }

        [MessageHandler(GameMessageOpcode.ClientIcCommSendMessage)]
        public static void HandleIcCommSendMessage(WorldSession session, ClientIcCommSendMessage icCommSendMessage)
        {
            IcCommMessageResult result = IcCommMessageResult.Sent;
            IcCommChannel channel = GlobalIcCommManager.Instance.GetICChannel(icCommSendMessage.ChannelID);
            if (channel == null)
            {
                result = IcCommMessageResult.NotInChannel; // closest to the correct kind of error
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(icCommSendMessage.Recipient))
                {
                    Player recipient = CharacterManager.Instance.GetPlayer(icCommSendMessage.Recipient);
                    if (recipient == null)
                    {
                        result = IcCommMessageResult.NotInChannel;
                    }
                    else
                    {
                        result = channel.SendMessageToPlayer(recipient.CharacterId, icCommSendMessage.Message, session.Player);
                    }
                }
                else
                {
                    result = channel.SendMessageToChannel(icCommSendMessage.Message, session.Player);
                }
            }

            session.EnqueueMessageEncrypted(new ServerIcCommSendMessageResult()
            {
                Channel = icCommSendMessage.ChannelID,
                MessageID = icCommSendMessage.MessageId,
                Result = result
            });
        }

        [MessageHandler(GameMessageOpcode.Client0549)]
        public static void HandleClient0549(WorldSession session, Client0549 client0549)
        {
            log.Trace("0549");
        }
    }
}
