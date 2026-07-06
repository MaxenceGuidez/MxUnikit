using UnityEngine;

namespace MxUnikit.Provider.Samples.MultiKeyRegistration
{
    public class MultiKeyRegistrationDemo : MonoBehaviour
    {
        private void Start()
        {
            DemoRegisterBothKeys();
            DemoResolveBothWays();
            DemoUnregister();
            DemoBaseKeyOnly();
        }

        private static void DemoRegisterBothKeys()
        {
            Debug.Log("--- Register Under Base And Concrete Keys ---");

            DemoManager manager = new DemoManager();
            manager.RegisterSelf();
        }

        private static void DemoResolveBothWays()
        {
            Debug.Log("--- Resolve By Base And By Concrete Type ---");

            DemoManagerBase byBase = MxProvider.Get<DemoManagerBase>();
            byBase.Describe();

            DemoManager byConcrete = MxProvider.Get<DemoManager>();
            byConcrete.DoConcreteWork();

            Debug.Log($"[Demo] Same instance both ways: {ReferenceEquals(byBase, byConcrete)}");
        }

        private static void DemoUnregister()
        {
            Debug.Log("--- Unregister Both Keys ---");

            MxProvider.Unregister<DemoManager>();
            MxProvider.Unregister<DemoManagerBase>();

            Debug.Log($"[Demo] Base still registered: {MxProvider.IsRegistered<DemoManagerBase>()}");
        }

        private static void DemoBaseKeyOnly()
        {
            Debug.Log("--- Base Key Only (why the override matters) ---");

            DemoManagerBase manager = new DemoManager();
            MxProvider.Register(manager);

            // resolution is strict: the concrete key was never registered, expect an error and null
            DemoManager missing = MxProvider.Get<DemoManager>();
            Debug.Log($"[Demo] Resolved by concrete type: {(missing == null ? "null (as expected)" : "unexpected!")}");

            MxProvider.Unregister<DemoManagerBase>();
        }
    }
}
