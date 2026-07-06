using UnityEngine;

namespace MxUnikit.Provider.Samples.MxProviderDemo
{
    public class AudioService
    {
        public AudioService()
        {
            Debug.Log("[AudioService] Initialized");
        }

        public void PlaySound(string soundName)
        {
            Debug.Log($"[AudioService] Playing sound: {soundName}");
        }

        public void StopSound()
        {
            Debug.Log("[AudioService] Stopping sound");
        }
    }

    public class DataService
    {
        public DataService()
        {
            Debug.Log("[DataService] Initialized");
        }

        public void SaveData(string data)
        {
            Debug.Log($"[DataService] Saving data: {data}");
        }

        public string LoadData()
        {
            Debug.Log("[DataService] Loading data");
            return "Sample Data";
        }
    }

    public class InputManager : MonoBehaviour
    {
        private void Awake()
        {
            Debug.Log("[InputManager] MonoBehaviour manager initialized");
        }

        public void ProcessInput()
        {
            Debug.Log("[InputManager] Processing input");
        }
    }

    public class SaveService
    {

    }
}
