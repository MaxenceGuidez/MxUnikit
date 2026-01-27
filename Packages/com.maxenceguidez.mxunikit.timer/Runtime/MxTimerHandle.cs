namespace MxUnikit.Timer
{
    public readonly struct MxTimerHandle
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

        public override bool Equals(object obj) => obj is MxTimerHandle other && Id == other.Id && Version == other.Version;
        public override int GetHashCode() => Id.GetHashCode() ^ Version.GetHashCode();
        public static bool operator ==(MxTimerHandle a, MxTimerHandle b) => a.Id == b.Id && a.Version == b.Version;
        public static bool operator !=(MxTimerHandle a, MxTimerHandle b) => !(a == b);
    }
}
