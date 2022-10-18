using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Entity;
using NexusForever.Game.Social;
using NexusForever.Game.Static.RBAC;
using NexusForever.Game.Static.Social;
using NexusForever.WorldServer.Command.Context;
using NexusForever.WorldServer.Command.Convert;
using NexusForever.WorldServer.Command.Static;
using NLog;
using System;

namespace NexusForever.WorldServer.Command.Handler
{
    [Command(Permission.Chron, "Send a message to another player's datachron channel.", "chron")]
    public class ChronCommandCategory : CommandCategory
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();
        /// <summary>
        /// Invoke <see cref="CommandCategory"/> with the supplied <see cref="ICommandContext"/> and <see cref="ParameterQueue"/>.
        /// </summary>
        public override CommandResult Invoke(ICommandContext context, ParameterQueue queue)
        {
            try {
                CommandResult result = CanInvoke(context); // check permissions.
                if (result != CommandResult.Ok)
                    return result;

                ChronCommandParameterConverter con = new ChronCommandParameterConverter();
                con.Convert(context, queue);

                IPlayer player = context.GetTargetOrInvoker<IPlayer>();

                IPlayer targetPlayer = null;

                if (con.PlayerName != null && con.PlayerName.Length > 0)
                {
                    targetPlayer = PlayerManager.Instance.GetPlayer(con.PlayerName);
                }
                else
                {
                    context.SendError($"Player {con.PlayerName} not found.");
                    return CommandResult.InvalidParameters;
                }

                string echoText = con.TextMode ? "[TXT]> " + con.Message : con.Message;
                string targetText = con.TextMode ? "[TXT]< " + con.Message : con.Message;

                ChatMessageBuilder cmb = new ChatMessageBuilder()
                {
                    Guid = targetPlayer.Guid,
                    Type = ChatChannelType.Datachron,
                    FromName = "to " + targetPlayer.Name,
                    Self = true,
                    Text = echoText
                };
                player.Session.EnqueueMessageEncrypted(cmb.Build());

                cmb = new ChatMessageBuilder()
                {
                    Guid = targetPlayer.Guid,
                    Type = ChatChannelType.Datachron,
                    FromName = "from " + player.Name,
                    Self = false,
                    Text = targetText
                };
                targetPlayer.Session.EnqueueMessageEncrypted(cmb.Build());

            }
            catch (Exception e)
            {
                log.Error($"Exception caught in ChronCommandCategory.Invoke!\nInvoked by {context.GetTargetOrInvoker<IPlayer>().Name}; {e.Message} :\n{e.StackTrace}");
                context.SendError("Oops! An error occurred. Please check your command input and try again.");
            }

            return CommandResult.Ok;
        }
    }
}
