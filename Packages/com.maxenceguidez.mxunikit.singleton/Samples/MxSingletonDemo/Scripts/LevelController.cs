using UnityEngine;

namespace MxUnikit.Singleton.Samples
{
    public class LevelController : MxMonoSingleton<LevelController>
    {
        protected override MxSingletonMode Mode => MxSingletonMode.SceneScoped;

        [SerializeField] private string _levelName = "Level 1";

        protected override void OnAwake()
        {
            Debug.Log("[LevelController] Scene-scoped singleton initialized");
        }

        public void CompleteLevel()
        {
            Debug.Log($"[LevelController] Level '{_levelName}' completed!");
        }

        protected override void OnDestroySingleton()
        {
            Debug.Log("[LevelController] Scene-scoped singleton destroyed");
        }
    }
}
