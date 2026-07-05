using System;
using UnityEngine.UIElements;

namespace MxUnikit.UI.Samples
{
    public class SamplePauseMenu : MxMenu
    {
        public event Action OnClickResume;
        public event Action OnClickQuitToMenu;

        public SamplePauseMenu()
        {
            Label title = new Label("Paused");
            title.AddToClassList("mx-demo-subtitle");
            Add(title);

            Button resumeButton = new Button(() => OnClickResume?.Invoke()) { text = "Resume" };
            Button quitButton = new Button(() => OnClickQuitToMenu?.Invoke()) { text = "Quit to Menu" };

            Add(resumeButton);
            Add(quitButton);
        }
    }
}
