using System.Numerics;

namespace NexusForever.Shared.GameTable.Model
{
    public class WorldLocation2Entry
    {
        public uint Id;
        public float Radius;
        public float MaxVerticalDistance;
        public float Position0;
        public float Position1;
        public float Position2;
        public float Facing0;
        public float Facing1;
        public float Facing2;
        public float Facing3;
        public uint WorldId;
        public uint WorldZoneId;
        public uint Phases;

        public Vector3 GetPosition()
        {
            return new Vector3(Position0, Position1, Position2);
        }

        public Quaternion GetQuaternion()
        {
            return new Quaternion(Facing0, Facing1, Facing2, Facing3);
        }
    }
}
