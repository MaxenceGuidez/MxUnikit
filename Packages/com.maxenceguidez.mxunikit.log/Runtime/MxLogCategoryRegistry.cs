using System.Collections.Generic;
using System.Linq;

namespace MxUnikit.Log
{
    public static class MxLogCategoryRegistry
    {
        private static readonly Dictionary<string, MxLogCategory> _customCategories = new Dictionary<string, MxLogCategory>();

        public static MxLogCategory Register(string id)
        {
            if (_customCategories.TryGetValue(id, out MxLogCategory existing))
            {
                return existing;
            }

            MxLogCategory category = MxLogCategory.Create(id);
            _customCategories[id] = category;
            return category;
        }

        public static bool TryGet(string id, out MxLogCategory category)
        {
            return _customCategories.TryGetValue(id, out category);
        }

        public static IReadOnlyCollection<MxLogCategory> GetAllCustomCategories()
        {
            return _customCategories.Values.ToList().AsReadOnly();
        }

        public static IEnumerable<MxLogCategory> GetAllCategories()
        {
            return GetBuiltInCategories().Concat(_customCategories.Values);
        }

        private static IEnumerable<MxLogCategory> GetBuiltInCategories()
        {
            yield return MxLogCategory.Default;
            yield return MxLogCategory.API;
            yield return MxLogCategory.Audio;
            yield return MxLogCategory.Debug;
            yield return MxLogCategory.Event;
            yield return MxLogCategory.Firebase;
            yield return MxLogCategory.Game;
            yield return MxLogCategory.Inputs;
            yield return MxLogCategory.Inventory;
            yield return MxLogCategory.Network;
            yield return MxLogCategory.Player;
            yield return MxLogCategory.Session;
            yield return MxLogCategory.UI;
        }
    }
}
