namespace NexusForever.WorldServer.Game.InterClient.Static
{
    public enum IcCommMessageResult
    {
        Sent               = 1,
        Throttled          = 2,
        NotInChannel       = 3,
        InvalidText        = 7,
        MissingEntitlement = 8
    }
}
