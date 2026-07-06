using UnityEngine;

namespace MxUnikit.Singleton.Samples.SampleMxSingleton
{
    public class SampleGameManager : MxMonoSingleton<SampleGameManager>
    {
        public int Score { get; private set; }

        protected override void OnAwake()
        {
            Debug.Log("[SampleGameManager] Persistent singleton initialized");
            Score = 0;
        }

        public void AddScore(int points)
        {
            Score += points;
            Debug.Log($"[SampleGameManager] Score: {Score}");
        }

        protected override void OnDestroySingleton()
        {
            Debug.Log("[SampleGameManager] Persistent singleton destroyed");
        }
    }
}
