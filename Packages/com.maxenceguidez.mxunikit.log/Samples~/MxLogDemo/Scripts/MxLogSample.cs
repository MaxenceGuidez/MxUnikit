using System;
using UnityEngine;

namespace MxUnikit.Log.Samples
{
    public class MxLogSample : MonoBehaviour
    {
        private void Start()
        {
            DemoBasicLogging();
            DemoCustomCategories();
            DemoContains();
        }

        private static void DemoBasicLogging()
        {
            MxLog.L("Basic log message");
            MxLog.W("Warning message");
            MxLog.E("Error message");
            MxLog.Ex(new Exception("Exception message"));
        }

        private static void DemoCustomCategories()
        {
            MxLog.L("Player started level 1");
            MxLog.L("Enemy defeated");
            MxLog.L("Item collected: Health Potion");
        }

        private static void DemoContains()
        {
            MxLog.L("Opening main menu");
            MxLog.L("All menus loaded");
            MxLog.L("Displaying submenu options");
            MxLog.L("Menu opened");
        }
    }
}
