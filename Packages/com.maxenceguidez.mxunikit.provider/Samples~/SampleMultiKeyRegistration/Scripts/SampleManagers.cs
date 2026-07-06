using UnityEngine;

namespace MxUnikit.Provider.Samples.SampleMultiKeyRegistration
{
    public abstract class SampleManagerBase
    {
        public virtual void RegisterSelf()
        {
            // 'this' is typed SampleManagerBase here, so T is inferred as the base type
            MxProvider.Register(this);
        }

        public void Describe()
        {
            Debug.Log($"[Sample] Resolved instance runtime type: {GetType().Name}");
        }
    }

    public class SampleManager : SampleManagerBase
    {
        public override void RegisterSelf()
        {
            base.RegisterSelf();

            // 'this' is typed SampleManager here: adds the concrete key
            MxProvider.Register(this);
        }

        public void DoConcreteWork()
        {
            Debug.Log("[SampleManager] Concrete member accessed without any cast");
        }
    }
}
