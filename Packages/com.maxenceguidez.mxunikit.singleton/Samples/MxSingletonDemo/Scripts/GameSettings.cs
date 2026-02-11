using UnityEngine;

namespace MxUnikit.Singleton.Samples
{
    public class GameSettings : MxSingleton<GameSettings>
    {
        public string PlayerName { get; set; } = "Player";

        protected override void OnInit()
        {
            Debug.Log("[GameSettings] Normal singleton initialized");
        }
    }
}
