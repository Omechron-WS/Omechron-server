using NexusForever.Database.Character.Migrations;
using NexusForever.Shared;
using NexusForever.WorldServer.Game.InterClient.Static;
using NexusForever.WorldServer.Game.Social;
using NexusForever.WorldServer.Game.Social.Static;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexusForever.WorldServer.Game.InterClient
{
    public sealed class GlobalIcCommManager : Singleton<GlobalIcCommManager>
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        private readonly Dictionary<ulong, IcCommChannel> channels = new();
        private ulong nextChannelId = 1;
        private readonly Dictionary<IcCommChannelType, Dictionary<ulong /* Party or guild ID */, Dictionary<string, ulong>>> channelNames = new();

        public Dictionary<string, ulong> GetNameDictionary(IcCommChannelType type, ulong context = 0, bool create = false)
        {
            switch (type)
            {
                case IcCommChannelType.Global:
                    context = 0;
                    break;
                case IcCommChannelType.Group:
                    context = 0; // no parties yet, lol. Overwrite with player's party ID later.
                    break;
                case IcCommChannelType.Guild:
                    break; // don't overwrite the provided guildContext.
            }

            if (!channelNames[type].TryGetValue(context, out var nameDictionary))
            {
                if (create)
                {
                    nameDictionary = new Dictionary<string, ulong>();
                    channelNames[type].Add(context, nameDictionary);
                    return nameDictionary;
                }
                return null;
            }

            return nameDictionary;
        }

        public void Initialise()
        {
            foreach (IcCommChannelType type in Enum.GetValues(typeof(IcCommChannelType)))
            {
                channelNames.Add(type, new());
            }
        }

        /// <summary>
        /// Returns an existing <see cref="IcCommChannel"/> with supplied <see cref="IcCommChannelType"/> and id.
        /// </summary>
        public IcCommChannel GetICChannel(ulong id)
        {
            if (channels.TryGetValue(id, out IcCommChannel channel))
                return channel;

            return null;
        }

        /// <summary>
        /// Returns an existing <see cref="IcCommChannel"/> with supplied <see cref="IcCommChannelType"/>, name and guild/party ID.
        /// </summary>
        public IcCommChannel GetICChannel(IcCommChannelType type, string name, ulong context = 0)
        {
            if (GetNameDictionary(type, context)?.TryGetValue(name, out ulong chatId) != true)
                return null;

            return GetICChannel(chatId);
        }

        /// <summary>
        /// Create a new <see cref="IcCommChannel"/> with supplied <see cref="IcCommChannelType"/>, name and guild/party ID.
        /// </summary>
        public IcCommChannel CreateChatChannel(IcCommChannelType type, string name, ulong context = 0)
        {
            Dictionary<string, ulong> nameDictionary = GetNameDictionary(type, context, true);
            if (nameDictionary.TryGetValue(name, out ulong chatId))
            {
                if (!channels.TryGetValue(chatId, out IcCommChannel channel))
                    throw new InvalidOperationException($"ICC channel {type},{name} already exists!");
                return channel;
            }
            else
            {
                ulong id = nextChannelId++;
                var channel = new IcCommChannel(type, id, context, name);
                channels.Add(id, channel);
                nameDictionary.Add(name, id);
                return channel;
            }
        }
    }
}
