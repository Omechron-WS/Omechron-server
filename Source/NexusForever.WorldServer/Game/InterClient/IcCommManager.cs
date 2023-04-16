using NexusForever.WorldServer.Game.Entity;
using NexusForever.WorldServer.Game.Social;
using System.Collections.Generic;

namespace NexusForever.WorldServer.Game.InterClient
{
    public class IcCommManager
    {
        private readonly Player owner;
        private readonly Dictionary<ulong, IcCommChannel> channels = new();

        /// <summary>
        /// Create a new <see cref="ChatManager"/>.
        /// </summary>
        public IcCommManager(Player player)
        {
            owner = player;
        }
        /// <summary>
        /// Invoked when a <see cref="Player"/> goes offline.
        /// </summary>
        public void OnLogout()
        {
            foreach (IcCommChannel channel in channels.Values)
                channel.OnMemberLogout(owner);
        }
    }
}
