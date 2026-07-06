using UnityEngine;

namespace MxUnikit.Provider.Samples.SampleMxProvider
{
    public class SampleProviderManager : MonoBehaviour
    {
        private void Start()
        {
            BasicRegistration();
            DataServiceRegistration();
            TryGetUsage();
            MonoBehaviourManagerRegistration();
        }

        private static void BasicRegistration()
        {
            Debug.Log("--- Basic Registration ---");

            AudioService audioService = new AudioService();
            MxProvider.Register(audioService);

            AudioService service = MxProvider.Get<AudioService>();
            service.PlaySound("sample_sound");
        }

        private static void DataServiceRegistration()
        {
            Debug.Log("--- Data Service ---");

            DataService dataService = new DataService();
            MxProvider.Register(dataService);

            DataService service = MxProvider.Get<DataService>();
            string data = service.LoadData();
            Debug.Log($"[Sample] Loaded: {data}");
        }

        private static void TryGetUsage()
        {
            Debug.Log("--- TryGet ---");

            if (MxProvider.TryGet(out DataService dataService))
            {
                dataService.SaveData("Test Data");
            }
            else
            {
                Debug.Log("[Sample] Service not found");
            }

            if (!MxProvider.TryGet(out SaveService _))
            {
                Debug.Log("[Sample] SaveService not registered (as expected)");
            }
        }

        private static void MonoBehaviourManagerRegistration()
        {
            Debug.Log("--- MonoBehaviour Manager ---");

            GameObject inputGo = new GameObject("InputManager");
            InputManager inputManager = inputGo.AddComponent<InputManager>();

            MxProvider.Register(inputManager);

            InputManager service = MxProvider.Get<InputManager>();
            service.ProcessInput();
        }
    }
}
