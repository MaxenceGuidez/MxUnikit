using UnityEngine;

namespace MxUnikit.ServiceLocator.Samples
{
    #region Audio

    public interface IAudioService : IService
    {
        public void PlaySound(string soundName);
        public void StopSound();
    }

    public class AudioService : IAudioService
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

    #endregion

    #region Data

    public interface IDataService : IService
    {
        public void SaveData(string data);
        public string LoadData();
    }

    public class DataService : IDataService
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

    #endregion

    #region Input

    public interface IInputManager : IService
    {
        public void ProcessInput();
    }

    public class InputManager : MonoBehaviour, IInputManager
    {
        private void Awake()
        {
            Debug.Log("[InputManager] MonoBehaviour service initialized");
        }

        public void ProcessInput()
        {
            Debug.Log("[InputManager] Processing input");
        }
    }

    #endregion

    public interface ISaveService : IService
    {

    }
}
