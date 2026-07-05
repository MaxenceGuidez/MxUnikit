using System;
using UnityEngine.UIElements;

namespace MxUnikit.UI.Samples
{
    public class SampleSettingsMenu : MxMenu
    {
        public event Action OnClickBack;

        public SampleSettingsMenu()
        {
            Label title = new Label("Settings");
            title.AddToClassList("mx-demo-subtitle");
            Add(title);

            Toggle vsyncToggle = new Toggle("VSync") { value = true };
            Slider volumeSlider = new Slider("Volume", 0f, 1f) { value = 0.8f };
            Add(vsyncToggle);
            Add(volumeSlider);

            Button backButton = new Button(() => OnClickBack?.Invoke()) { text = "Back" };
            backButton.AddToClassList("mx-demo-back-button");
            Add(backButton);
        }
    }
}
