using System;

namespace MxUnikit.Timer
{
    public readonly struct MxTimerHandle : IEquatable<MxTimerHandle>
    {
        internal readonly int Id;
        internal readonly int Version;

        internal MxTimerHandle(int id, int version)
        {
            Id = id;
            Version = version;
        }

        public bool IsValid => Id > 0;

        public static readonly MxTimerHandle Invalid = default(MxTimerHandle);

        public bool Equals(MxTimerHandle other) => Id == other.Id && Version == other.Version;
        public override bool Equals(object obj) => obj is MxTimerHandle other && Equals(other);
        public override int GetHashCode() => HashCode.Combine(Id, Version);
        public static bool operator ==(MxTimerHandle a, MxTimerHandle b) => a.Equals(b);
        public static bool operator !=(MxTimerHandle a, MxTimerHandle b) => !a.Equals(b);
    }
}
