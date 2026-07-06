using UnityEngine;

namespace MxUnikit.Provider.Samples.SampleMultiKeyRegistration
{
    public class SampleMultiKeyRegistrationLauncher : MonoBehaviour
    {
        private void Start()
        {
            RegisterBothKeys();
            ResolveBothWays();
            UnregisterBothKeys();
            BaseKeyOnly();
        }

        private static void RegisterBothKeys()
        {
            Debug.Log("--- Register Under Base And Concrete Keys ---");

            SampleManager manager = new SampleManager();
            manager.RegisterSelf();
        }

        private static void ResolveBothWays()
        {
            Debug.Log("--- Resolve By Base And By Concrete Type ---");

            SampleManagerBase byBase = MxProvider.Get<SampleManagerBase>();
            byBase.Describe();

            SampleManager byConcrete = MxProvider.Get<SampleManager>();
            byConcrete.DoConcreteWork();

            Debug.Log($"[Sample] Same instance both ways: {ReferenceEquals(byBase, byConcrete)}");
        }

        private static void UnregisterBothKeys()
        {
            Debug.Log("--- Unregister Both Keys ---");

            MxProvider.Unregister<SampleManager>();
            MxProvider.Unregister<SampleManagerBase>();

            Debug.Log($"[Sample] Base still registered: {MxProvider.IsRegistered<SampleManagerBase>()}");
        }

        private static void BaseKeyOnly()
        {
            Debug.Log("--- Base Key Only (why the override matters) ---");

            SampleManagerBase manager = new SampleManager();
            MxProvider.Register(manager);

            // resolution is strict: the concrete key was never registered, expect an error and null
            SampleManager missing = MxProvider.Get<SampleManager>();
            Debug.Log($"[Sample] Resolved by concrete type: {(missing == null ? "null (as expected)" : "unexpected!")}");

            MxProvider.Unregister<SampleManagerBase>();
        }
    }
}
