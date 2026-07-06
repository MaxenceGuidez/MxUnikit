using UnityEngine;

namespace MxUnikit.Singleton.Samples.SampleMxSingleton
{
	public class SampleSingletonManager : MonoBehaviour
	{
		private void Start()
		{
			NormalSingletonUsage();
			PersistentSingletonUsage();
			SceneScopedSingletonUsage();
		}

		private static void NormalSingletonUsage()
		{
			Debug.Log("--- Normal Class Singleton ---");

            SampleGameSettings.Instance.PlayerName = "TestPlayer";

			Debug.Log($"[Sample] Initialized: {SampleGameSettings.IsInitialized}");
			Debug.Log($"[Sample] Player: {SampleGameSettings.Instance.PlayerName}");
		}

		private static void PersistentSingletonUsage()
		{
			Debug.Log("--- Persistent MonoBehaviour Singleton ---");

            if (SampleGameManager.HasInstance)
            {
                SampleGameManager.Instance.AddScore(50);
            }
            else
            {
                Debug.Log("[Sample] SampleGameManager not in scene");
            }
		}

		private static void SceneScopedSingletonUsage()
		{
			Debug.Log("--- Scene-Scoped Singleton ---");

			if (SampleLevelController.HasInstance)
			{
				SampleLevelController.Instance.CompleteLevel();
			}
			else
			{
				Debug.Log("[Sample] SampleLevelController not in scene");
			}
		}
	}
}
