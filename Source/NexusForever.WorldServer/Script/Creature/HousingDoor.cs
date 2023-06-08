using NexusForever.WorldServer.Game.Entity;
using NexusForever.WorldServer.Game.Entity.Static;
using NexusForever.WorldServer.Network.Message.Model;

namespace NexusForever.WorldServer.Script.Creature
{
    // housingDecorTypeId 30
    [Script(59496)] // White Picket Fence Arch - Housing Active Prop
    [Script(59497)] // White Picket Fence Gate - Housing Active Prop
    [Script(65845)] // Housing - Decor - Doors - Chain Link Security Gate
    [Script(65852)] // Housing - Decor - Doors - Gothic Gate
    [Script(65853)] // Housing - Decor - Doors - Elegant Aurin Gate
    [Script(65867)] // Housing - Decor - Activated - Ikthian Energy Barrier
    [Script(65868)] // Housing - Decor - Activated - Ikthian Laser Fence
    [Script(65869)] // Housing - Decor - Activated - Dominion Base Fencing
    [Script(73486)] // Gate - Decor - Housing Active Prop - CLD

    // housingDecorTypeId 59
    [Script(65852)] // Housing - Decor - Doors - Gothic Gate
    [Script(70052)] // Housing - Decor - Activated - Door
    [Script(72338)] // Housing Door
    [Script(73038)] // Caution Gate (Exile) - Housing Active Prop
    [Script(73077)] // Chain-Link Gate (Protostar) - Decor - Housing Active Prop - CLD
    [Script(73087)] // Stone Door (Torine) - Decor - Housing Active Prop - CLD
    [Script(73103)] // Osun Door - Decor - Housing Active Prop - CLD
    [Script(75398)] // Redmoon Door (Circular) - Decor - Housing Active Prop
    [Script(75400)] // Redmoon Door (Square) - Decor - Housing Active Prop

    public class HousingDoor : CreatureScript
    {
        const StandState DOOR_CLOSED = StandState.State0;
        const StandState DOOR_OPEN = StandState.State1;

        public override void OnCreate(WorldEntity me)
        {
            base.OnCreate(me);

            me.StandState = DOOR_CLOSED;
        }

        public override void OnActivateSuccess(WorldEntity me, WorldEntity activator)
        {
            base.OnActivateSuccess(me, activator);

            // TODO: Add cooldown

            // If Door is Opened, Close.
            if (me.StandState == DOOR_OPEN)
                me.StandState = DOOR_CLOSED;
            else
                me.StandState = DOOR_OPEN;

            // Emit from Player due to way Decor Entities are tracked on the map being... different.
            activator.EnqueueToVisible(new ServerEmote
            {
                Guid = me.Guid,
                StandState = me.StandState
            }, true);
        }
    }
}
