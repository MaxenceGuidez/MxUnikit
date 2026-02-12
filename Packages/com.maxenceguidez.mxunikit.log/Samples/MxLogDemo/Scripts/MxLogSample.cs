using UnityEngine;

namespace MxUnikit.Log.Samples
{
    // public static readonly MxLogCategory Gameplay = MxLogCategoryRegistry.Register("Gameplay");
    // public static readonly MxLogCategory Combat = MxLogCategoryRegistry.Register("Combat");
    // public static readonly MxLogCategory Loot = MxLogCategoryRegistry.Register("Loot");
    public class MxLogSample : MonoBehaviour
    {
        private void Start()
        {
            DemoBasicLogging();
            DemoCustomCategories();
        }

        private static void DemoBasicLogging()
        {
            MxLog.L("Basic log message");
            MxLog.W("Warning message");
            MxLog.E("Error message");
        }

        private static void DemoCustomCategories()
        {
            MxLog.L("Player started level 1");
            MxLog.L("Enemy defeated");
            MxLog.L("Item collected: Health Potion");
        }
    }
}
