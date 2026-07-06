using UnityEngine;

namespace MxUnikit.Singleton.Samples.SampleMxSingleton
{
    public class SampleLevelController : MxMonoSingleton<SampleLevelController>
    {
        protected override MxSingletonMode Mode => MxSingletonMode.SceneScoped;

        [SerializeField] private string _levelName = "Level 1";

        protected override void OnAwake()
        {
            Debug.Log("[SampleLevelController] Scene-scoped singleton initialized");
        }

        public void CompleteLevel()
        {
            Debug.Log($"[SampleLevelController] Level '{_levelName}' completed!");
        }

        protected override void OnDestroySingleton()
        {
            Debug.Log("[SampleLevelController] Scene-scoped singleton destroyed");
        }
    }
}
