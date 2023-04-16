using NexusForever.WorldServer.Game.CharacterCache;
using NexusForever.WorldServer.Game.Entity;
using NexusForever.WorldServer.Game.InterClient.Static;
using NexusForever.WorldServer.Game.Social;
using NexusForever.WorldServer.Game.Social.Static;
using NexusForever.WorldServer.Network.Message.Model;
using NLog;
using System;
using System.Collections.Generic;

namespace NexusForever.WorldServer.Game.InterClient
{
    public class IcCommChannel
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        public IcCommChannelType Type { get; }
        public ulong Id { get; }
        public ulong Context { get; }
        public string Name { get; set; }

        private readonly HashSet<ulong> members = new();

        /// <summary>
        /// Create a new <see cref="ChatChannel"/> with supplied <see cref="ChatChannelType"/> and id.
        /// </summary>
        public IcCommChannel(IcCommChannelType type, ulong id, ulong context, string name)
        {
            Type = type;
            Id = id;
            Context = context;
            Name = name;
        }

        /// <summary>
        /// Returns if <see cref="Player"/> can join the <see cref="IcCommChannel"/> with supplied password.
        /// </summary>
        public IcCommJoinResult CanJoin(Player player)
        {
            /*if (members.Contains(player.CharacterId))
                return IcCommJoinResult.Join;*/ // Wat do? There's no result for "you're already a member".

            return IcCommJoinResult.Join;
        }

        /// <summary>
        /// Add a new character to <see cref="IcCommChannel"/>.
        /// </summary>
        public void Join(ulong characterId)
        {
            if (members.Contains(characterId))
            {
                //throw new InvalidOperationException($"Member {characterId} for ICC channel {Type},{Id} already exists!");
            }
            else
            {
                members.Add(characterId);
            }

            log.Trace($"Added member {characterId} to ICC channel {Type},{Id}.");
        }

        public bool HasPlayer(ulong characterId)
        {
            return members.Contains(characterId);
        }

        public IcCommMessageResult SendMessageToPlayer(ulong characterId, string message, Player sender)
        {
            if(!members.Contains(characterId))
            {
                return IcCommMessageResult.NotInChannel;
            }

            Player player = CharacterManager.Instance.GetPlayer(characterId);

            player.Session.EnqueueMessageEncrypted(new ServerIcCommSendMessage()
            {
                Channel = Id,
                Message = message,
                SenderName = sender.Name
            });

            return IcCommMessageResult.Sent;
        }

        public IcCommMessageResult SendMessageToChannel(string message, Player sender)
        {
            if(!members.Contains(sender.CharacterId))
            {
                return IcCommMessageResult.NotInChannel; // Sender needs to be in channel to send.
            }

            var sendMessage = new ServerIcCommSendMessage()
            {
                Channel = Id,
                Message = message,
                SenderName = sender.Name
            };

            foreach (ulong member in members)
            {
                Player player = CharacterManager.Instance.GetPlayer(member);

                player.Session.EnqueueMessageEncrypted(sendMessage);
            }

            return IcCommMessageResult.Sent;
        }
        
        /// <summary>
        /// Invoked when a <see cref="Player"/> goes offline.
        /// </summary>
        public void OnMemberLogout(Player player)
        {
            members.Remove(player.CharacterId);
        }
    }
}
