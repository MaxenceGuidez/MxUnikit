using MxUnikit.Log;

namespace MxUnikit.UI.Samples
{
    public class SampleOverlayUiManager : MxOverlayUiManager
    {
        protected override void OnUiReady() { }

        protected override void OnFirstDialogOpened()
        {
            MxLog.L("Would disable gameplay input here.");
        }

        protected override void OnLastDialogClosed()
        {
            MxLog.L("Would restore gameplay input here.");
        }
    }
}
