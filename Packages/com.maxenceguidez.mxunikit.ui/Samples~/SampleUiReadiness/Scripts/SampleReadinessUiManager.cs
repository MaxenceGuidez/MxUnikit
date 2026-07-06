using UnityEngine.UIElements;

namespace MxUnikit.UI.Samples.SampleUiReadiness
{
    public class SampleReadinessUiManager : MxUiManager
    {
        public Label StatusLabel { get; private set; }

        protected override void OnUiReady()
        {
            StatusLabel = new Label("UI loaded.")
            {
                style =
                {
                    fontSize = 24,
                    marginTop = 16,
                    marginLeft = 16
                }
            };

            Root.Add(StatusLabel);
        }
    }
}
