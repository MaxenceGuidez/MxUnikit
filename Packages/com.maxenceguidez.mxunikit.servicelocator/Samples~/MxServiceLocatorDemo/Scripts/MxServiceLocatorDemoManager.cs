using UnityEngine;

namespace MxUnikit.ServiceLocator.Samples
{
	public class MxServiceLocatorDemoManager : MonoBehaviour
	{
		private void Start()
		{
			DemoBasicRegistration();
			DemoDataService();
			DemoTryGet();
			DemoMonoBehaviourService();
		}

		private static void DemoBasicRegistration()
		{
			Debug.Log("--- Basic Registration Demo ---");

			AudioService audioService = new AudioService();
			MxServiceLocator.Register<IAudioService>(audioService);

			IAudioService service = MxServiceLocator.Get<IAudioService>();
			service.PlaySound("demo_sound");
		}

		private static void DemoDataService()
		{
			Debug.Log("--- Data Service Demo ---");

			DataService dataService = new DataService();
			MxServiceLocator.Register<IDataService>(dataService);

			IDataService service = MxServiceLocator.Get<IDataService>();
			string data = service.LoadData();
			Debug.Log($"[Demo] Loaded: {data}");
		}

		private static void DemoTryGet()
		{
			Debug.Log("--- TryGet Demo ---");

			if (MxServiceLocator.TryGet(out IDataService dataService))
			{
				dataService.SaveData("Test Data");
			}
			else
			{
				Debug.Log("[Demo] Service not found");
			}

			if (!MxServiceLocator.TryGet(out ISaveService _))
			{
				Debug.Log("[Demo] ISaveService not registered (as expected)");
			}
		}

		private static void DemoMonoBehaviourService()
		{
			Debug.Log("--- MonoBehaviour Service Demo ---");

			GameObject inputGo = new GameObject("InputManager");
			InputManager inputManager = inputGo.AddComponent<InputManager>();

			MxServiceLocator.Register(inputManager);

			InputManager service = MxServiceLocator.Get<InputManager>();
            service.ProcessInput();
		}
	}
}
