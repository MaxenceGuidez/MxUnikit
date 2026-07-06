using UnityEngine;

namespace MxUnikit.Provider.Samples.MultiKeyRegistration
{
    public abstract class DemoManagerBase
    {
        public virtual void RegisterSelf()
        {
            // 'this' is typed DemoManagerBase here, so T is inferred as the base type
            MxProvider.Register(this);
        }

        public void Describe()
        {
            Debug.Log($"[Demo] Resolved instance runtime type: {GetType().Name}");
        }
    }

    public class DemoManager : DemoManagerBase
    {
        public override void RegisterSelf()
        {
            base.RegisterSelf();

            // 'this' is typed DemoManager here: adds the concrete key
            MxProvider.Register(this);
        }

        public void DoConcreteWork()
        {
            Debug.Log("[DemoManager] Concrete member accessed without any cast");
        }
    }
}
