using System;

namespace MxUnikit.Timer
{
    public readonly struct MxTimerHandle : IEquatable<MxTimerHandle>
    {
        internal readonly int Id;

        internal MxTimerHandle(int id)
        {
            Id = id;
        }

        public bool IsValid => Id > 0;

        public static readonly MxTimerHandle Invalid = default(MxTimerHandle);

        public bool Equals(MxTimerHandle other) => Id == other.Id;
        public override bool Equals(object obj) => obj is MxTimerHandle other && Equals(other);
        public override int GetHashCode() => Id;
        public static bool operator ==(MxTimerHandle a, MxTimerHandle b) => a.Equals(b);
        public static bool operator !=(MxTimerHandle a, MxTimerHandle b) => !a.Equals(b);
    }
}
