using System;
using Microsoft.Extensions.DependencyInjection;
using NexusForever.Game;
using NexusForever.Game.Social;
using NexusForever.Game.Static.RBAC;
using NexusForever.Game.Static.Social;
using NexusForever.Network.Session;
using NexusForever.Shared;
using NexusForever.WorldServer.Command.Context;
using NexusForever.WorldServer.Network;
using System.Collections.Generic;
using System.Linq;
using NexusForever.Game.Entity;
using NexusForever.Game.Abstract.Entity;

namespace NexusForever.WorldServer.Command.Handler
{
    [Command(Permission.Realm, "A collection of commands to manage the realm.", "realm")]
    public class RealmCommandCategory : CommandCategory
    {
        [Command(Permission.RealmShutdown, "A collection of commands to manage realm shutdown.", "shutdown")]
        public class RealmShutdownCommandCategory : CommandCategory
        {
            [Command(Permission.RealmShutdownStart, "Start a new realm shutdown.", "start")]
            public void HandleRealmShutdownStart(ICommandContext context,
                [Parameter("Time till shutdown. (Format: dd:hh:mm:ss)")]
                TimeSpan span)
            {
                if (ShutdownManager.Instance.IsShutdownPending)
                {
                    context.SendError("Realm already has a pending shutdown!");
                    return;
                }

                ShutdownManager.Instance.StartShutdown(span);
            }

            [Command(Permission.RealmShutdownCancel, "Cancel pending realm shutdown.", "cancel")]
            public void HandleRealmShutdownCancel(ICommandContext context)
            {
                if (!ShutdownManager.Instance.IsShutdownPending)
                {
                    context.SendError("Realm doesn't have a pending shutdown!");
                    return;
                }

                ShutdownManager.Instance.CancelShutdown();
            }
        }

        [Command(Permission.RealmMOTD, "Set the realm's message of the day and announce to the realm.", "motd")]
        public void HandleRealmMotd(ICommandContext context,
            [Parameter("New message of the day for the realm.")]
            string message)
        {
            RealmContext.Instance.Motd = message;

            // TODO: move commands to dependency injection...
            var networkManager = LegacyServiceProvider.Provider.GetService<INetworkManager<IWorldSession>>();
            foreach (IWorldSession session in networkManager)
                GlobalChatManager.Instance.SendMessage(session, RealmContext.Instance.Motd, "MOTD", ChatChannelType.Realm);
        }

        [Command(Permission.RealmMaxPlayers, "Set the maximum players allowed to connect.", "max")]
        public void HandleRealmMax(ICommandContext context,
            [Parameter("Max players allowed.")]
            uint maxPlayers)
        {
            // TODO: move commands to dependency injection...
            var queueManager = LegacyServiceProvider.Provider.GetService<ILoginQueueManager>();
            queueManager.SetMaxPlayers(maxPlayers);
        }

        [Command(Permission.RealmOnline, "Displays the users online", "online")]
        public void HandleRealmOnline(ICommandContext context)
        {
            List<IPlayer> allPlayers = PlayerManager.Instance.ToList();

            int index = 0;
            foreach (IPlayer player in allPlayers)
            {
                string infoString = "";
                TimeSpan uptime = player.Session.GetUptime();
                infoString += $"[{index++}] {player.Account?.Email} id:{player.Account?.Id} | {player.Name} | {uptime:%d}d {uptime:%h}h {uptime:%m}m";

                context.SendMessage(infoString);
            }

            if (allPlayers.Count == 0)
                context.SendMessage($"No sessions connected.");
        }

        [Command(Permission.RealmUptime, "Display the current uptime of the server.", "uptime")]
        public void HandleUptimeCheck(ICommandContext context)
        {
            context.SendMessage($"Currently up for {WorldServer.Uptime:%d}d {WorldServer.Uptime:%h}h {WorldServer.Uptime:%m}m {WorldServer.Uptime:%s}s");
        }
    }
}
