using UnityEngine;

namespace MxUnikit.Provider.Samples.MxProviderDemo
{
    public class MxProviderDemoManager : MonoBehaviour
    {
        private void Start()
        {
            DemoBasicRegistration();
            DemoDataService();
            DemoTryGet();
            DemoMonoBehaviourManager();
        }

        private static void DemoBasicRegistration()
        {
            Debug.Log("--- Basic Registration Demo ---");

            AudioService audioService = new AudioService();
            MxProvider.Register(audioService);

            AudioService service = MxProvider.Get<AudioService>();
            service.PlaySound("demo_sound");
        }

        private static void DemoDataService()
        {
            Debug.Log("--- Data Service Demo ---");

            DataService dataService = new DataService();
            MxProvider.Register(dataService);

            DataService service = MxProvider.Get<DataService>();
            string data = service.LoadData();
            Debug.Log($"[Demo] Loaded: {data}");
        }

        private static void DemoTryGet()
        {
            Debug.Log("--- TryGet Demo ---");

            if (MxProvider.TryGet(out DataService dataService))
            {
                dataService.SaveData("Test Data");
            }
            else
            {
                Debug.Log("[Demo] Service not found");
            }

            if (!MxProvider.TryGet(out SaveService _))
            {
                Debug.Log("[Demo] SaveService not registered (as expected)");
            }
        }

        private static void DemoMonoBehaviourManager()
        {
            Debug.Log("--- MonoBehaviour Manager Demo ---");

            GameObject inputGo = new GameObject("InputManager");
            InputManager inputManager = inputGo.AddComponent<InputManager>();

            MxProvider.Register(inputManager);

            InputManager service = MxProvider.Get<InputManager>();
            service.ProcessInput();
        }
    }
}
