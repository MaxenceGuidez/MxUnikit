using System;
using UnityEngine.UIElements;

namespace MxUnikit.UI.Samples
{
    public class SampleHomeMenu : MxMenu
    {
        public event Action OnClickPlay;
        public event Action OnClickSettings;
        public event Action OnClickInfos;
        public event Action OnClickQuit;

        public SampleHomeMenu()
        {
            Label title = new Label("MxUnikit UI Demo");
            title.AddToClassList("mx-demo-title");
            Add(title);

            Button buttonPlay = new Button(() => OnClickPlay?.Invoke()) { text = "Play" };
            Button buttonSettings = new Button(() => OnClickSettings?.Invoke()) { text = "Settings" };
            Button buttonInfos = new Button(() => OnClickInfos?.Invoke()) { text = "Infos" };
            Button buttonQuit = new Button(() => OnClickQuit?.Invoke()) { text = "Quit" };

            Add(buttonPlay);
            Add(buttonSettings);
            Add(buttonInfos);
            Add(buttonQuit);
        }
    }
}
