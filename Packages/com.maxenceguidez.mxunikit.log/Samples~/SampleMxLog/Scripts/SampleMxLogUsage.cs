using System;
using UnityEngine;

namespace MxUnikit.Log.Samples.SampleMxLog
{
    public class SampleMxLogUsage : MonoBehaviour
    {
        private void Start()
        {
            BasicLogging();
            CustomCategories();
            ContainsFiltering();
        }

        private static void BasicLogging()
        {
            MxLog.L("Basic log message");
            MxLog.W("Warning message");
            MxLog.E("Error message");
            MxLog.Ex(new Exception("Exception message"));
        }

        private static void CustomCategories()
        {
            MxLog.L("Player started level 1");
            MxLog.L("Enemy defeated");
            MxLog.L("Item collected: Health Potion");
        }

        private static void ContainsFiltering()
        {
            MxLog.L("Opening main menu");
            MxLog.L("All menus loaded");
            MxLog.L("Displaying submenu options");
            MxLog.L("Menu opened");
        }
    }
}
