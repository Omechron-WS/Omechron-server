using NexusForever.WorldServer.Game.Entity;
using NexusForever.WorldServer.Game.InterClient.Static;
using NexusForever.WorldServer.Game.Social;
using NexusForever.WorldServer.Game.Social.Static;
using NLog;
using System;
using System.Collections.Generic;

namespace NexusForever.WorldServer.Game.InterClient
{
    public class InterClientChannel
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
        public InterClientChannel(IcCommChannelType type, ulong id, ulong context, string name)
        {
            Type = type;
            Id = id;
            Context = context;
            Name = name;
        }

        /// <summary>
        /// Returns if <see cref="Player"/> can join the <see cref="InterClientChannel"/> with supplied password.
        /// </summary>
        public IcCommJoinResult CanJoin(Player player)
        {
            /*if (members.Contains(player.CharacterId))
                return IcCommJoinResult.Join;*/ // Wat do? There's no result for "you're already a member".

            return IcCommJoinResult.Join;
        }

        /// <summary>
        /// Add a new character to <see cref="InterClientChannel"/>.
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
    }
}
