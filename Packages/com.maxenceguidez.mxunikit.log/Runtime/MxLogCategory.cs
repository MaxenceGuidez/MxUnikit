using System;

namespace MxUnikit.Log
{
    public sealed class MxLogCategory : IEquatable<MxLogCategory>
    {
        public string Id { get; }

        private MxLogCategory(string id)
        {
            Id = id;
        }

        public static readonly MxLogCategory Default = new MxLogCategory("Default");
        public static readonly MxLogCategory API = new MxLogCategory("API");
        public static readonly MxLogCategory Audio = new MxLogCategory("Audio");
        public static readonly MxLogCategory Debug = new MxLogCategory("Debug");
        public static readonly MxLogCategory Event = new MxLogCategory("Event");
        public static readonly MxLogCategory Firebase = new MxLogCategory("Firebase");
        public static readonly MxLogCategory Game = new MxLogCategory("Game");
        public static readonly MxLogCategory Inputs = new MxLogCategory("Inputs");
        public static readonly MxLogCategory Inventory = new MxLogCategory("Inventory");
        public static readonly MxLogCategory Network = new MxLogCategory("Network");
        public static readonly MxLogCategory Player = new MxLogCategory("Player");
        public static readonly MxLogCategory Session = new MxLogCategory("Session");
        public static readonly MxLogCategory UI = new MxLogCategory("UI");

        public static MxLogCategory Create(string id)
        {
            return new MxLogCategory(id);
        }

        public bool Equals(MxLogCategory other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return Id == other.Id;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as MxLogCategory);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public static bool operator ==(MxLogCategory left, MxLogCategory right)
        {
            if (left is null) return right is null;
            return left.Equals(right);
        }

        public static bool operator !=(MxLogCategory left, MxLogCategory right)
        {
            return !(left == right);
        }
    }
}
