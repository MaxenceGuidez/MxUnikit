using UnityEngine;

namespace MxUnikit.Singleton.Samples.SampleMxSingleton
{
    public class SampleGameSettings : MxSingleton<SampleGameSettings>
    {
        public string PlayerName { get; set; } = "Player";

        protected override void OnInit()
        {
            Debug.Log("[SampleGameSettings] Normal singleton initialized");
        }
    }
}
