using System;

namespace NexusForever.Database.Auth.Model
{
    public class ServerModel : IEquatable<ServerModel>
    {
        public byte Id { get; set; }
        public string Name { get; set; }
        public string Host { get; set; }
        public string InternalIP { get; set; }
        public bool AssumeOnline { get; set; }
        public ushort Port { get; set; }
        public byte Type { get; set; }

        public bool Equals(ServerModel other)
        {
            if (other == null)
                return false;
            if (ReferenceEquals(this, other))
                return true;
            return Id == other.Id &&
                   string.Equals(Name, other.Name, StringComparison.OrdinalIgnoreCase) &&
                   string.Equals(Host, other.Host, StringComparison.OrdinalIgnoreCase) &&
                   string.Equals(InternalIP, other.InternalIP, StringComparison.OrdinalIgnoreCase) &&
                   AssumeOnline == other.AssumeOnline &&
                   Port == other.Port &&
                   Type == other.Type;
        }
    }
}
