using System.Threading.Tasks;
using MxUnikit.Log;
using MxUnikit.Provider;

namespace MxUnikit.Core.Samples
{
    public class SampleGameManager : MxCoreManager
    {
        protected override async Task Initialize()
        {
            MxProvider.Register<SampleClockService>(new SampleClockService());

            // Simulate asynchronous startup work (asset loading, sign-in...).
            await Task.Delay(500);

            MxLog.L($"Boot completed at {MxProvider.Get<SampleClockService>().Now}");
        }
    }
}
