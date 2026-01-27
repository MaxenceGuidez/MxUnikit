using UnityEngine;

namespace MxUnikit.Timer
{
    internal class MxTimerRunner : MonoBehaviour
    {
        private void Update()
        {
            MxTimer.Update(Time.deltaTime, Time.unscaledDeltaTime);
        }
    }
}
