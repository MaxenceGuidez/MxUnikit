using UnityEngine;

namespace MxUnikit.Singleton.Samples
{
    public class GameManager : MxMonoSingleton<GameManager>
    {
        public int Score { get; private set; }

        protected override void OnAwake()
        {
            Debug.Log("[GameManager] Persistent singleton initialized");
            Score = 0;
        }

        public void AddScore(int points)
        {
            Score += points;
            Debug.Log($"[GameManager] Score: {Score}");
        }

        protected override void OnDestroySingleton()
        {
            Debug.Log("[GameManager] Persistent singleton destroyed");
        }
    }
}
