using UnityEngine;

namespace MxUnikit.Singleton.Samples
{
	public class MxSingletonDemoManager : MonoBehaviour
	{
		private void Start()
		{
			DemoNormalSingleton();
			DemoPersistentSingleton();
			DemoSceneScopedSingleton();
		}

		private static void DemoNormalSingleton()
		{
			Debug.Log("--- Normal Class Singleton Demo ---");

            GameSettings.Instance.PlayerName = "TestPlayer";

			Debug.Log($"[Demo] Initialized: {GameSettings.IsInitialized}");
			Debug.Log($"[Demo] Player: {GameSettings.Instance.PlayerName}");
		}

		private static void DemoPersistentSingleton()
		{
			Debug.Log("--- Persistent MonoBehaviour Singleton Demo ---");

            if (GameManager.HasInstance)
            {
                GameManager.Instance.AddScore(50);
            }
            else
            {
                Debug.Log("[Demo] GameManager not in scene");
            }
		}

		private static void DemoSceneScopedSingleton()
		{
			Debug.Log("--- Scene-Scoped Singleton Demo ---");

			if (LevelController.HasInstance)
			{
				LevelController.Instance.CompleteLevel();
			}
			else
			{
				Debug.Log("[Demo] LevelController not in scene");
			}
		}
	}
}
