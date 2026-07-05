using System;
using UnityEngine.UIElements;

namespace MxUnikit.UI.Samples
{
    public class SampleHomeMenu : MxMenu
    {
        public event Action OnClickPlay;
        public event Action OnClickSettings;
        public event Action OnClickQuit;

        public SampleHomeMenu()
        {
            Label title = new Label("MxUnikit UI Demo");
            title.AddToClassList("mx-demo-title");
            Add(title);

            Button playButton = new Button(() => OnClickPlay?.Invoke()) { text = "Play" };
            Button settingsButton = new Button(() => OnClickSettings?.Invoke()) { text = "Settings" };
            Button quitButton = new Button(() => OnClickQuit?.Invoke()) { text = "Quit" };

            Add(playButton);
            Add(settingsButton);
            Add(quitButton);
        }
    }
}
