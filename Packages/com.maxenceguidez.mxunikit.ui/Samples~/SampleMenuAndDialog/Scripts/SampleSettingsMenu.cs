using System;
using UnityEngine.UIElements;

namespace MxUnikit.UI.Samples.SampleMenuAndDialog
{
    public class SampleSettingsMenu : MxMenu
    {
        public event Action OnClickBack;

        public SampleSettingsMenu()
        {
            Label title = new Label("Settings");
            title.AddToClassList("mx-sample-subtitle");
            Add(title);

            Toggle toggleVsync = new Toggle("VSync") { value = true };
            Slider sliderVolume = new Slider("Volume", 0f, 1f) { value = 0.8f };
            Add(toggleVsync);
            Add(sliderVolume);

            Button buttonBack = new Button(() => OnClickBack?.Invoke()) { text = "Back" };
            buttonBack.AddToClassList("mx-sample-back-button");
            Add(buttonBack);
        }
    }
}
