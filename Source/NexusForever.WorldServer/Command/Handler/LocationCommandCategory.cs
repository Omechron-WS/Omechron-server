using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Static.RBAC;
using NexusForever.WorldServer.Command.Context;

namespace NexusForever.WorldServer.Command.Handler
{
    [Command(Permission.None, "Utility command for showing the player's exact position in the game world.", "location", "loc")]
    [CommandTarget(typeof(IPlayer))]
    class LocationCommandHandler : CommandCategory
    {
        [Command(Permission.None, "Print your current world position and its teleport coordinates to the chat window.", "print")]
        public void HandleLocationPrint(ICommandContext context)
        {
            IPlayer invokingPlayer = context.GetTargetOrInvoker<IPlayer>();
            float x = invokingPlayer.Position.X;
            float y = invokingPlayer.Position.Y;
            float z = invokingPlayer.Position.Z;
            uint zoneId = invokingPlayer.Zone.Id;
            uint mapId = invokingPlayer.Map.Entry.Id;
            string teleportCommand = $"!teleport coordinates {x} {y} {z} {mapId}";
            context.SendMessage($"Your current location:\n      X:{x}, Y:{y}, Z:{z}\n     World Map ID: {mapId}\n     Zone ID: {zoneId}\n     {teleportCommand}");
        }
    }
}
