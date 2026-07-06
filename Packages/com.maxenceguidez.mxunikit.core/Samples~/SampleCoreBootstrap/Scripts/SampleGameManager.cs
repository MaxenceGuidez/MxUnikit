using System.Threading.Tasks;
using MxUnikit.Log;
using MxUnikit.Provider;

namespace MxUnikit.Core.Samples.SampleCoreBootstrap
{
    public class SampleGameManager : MxCoreManager
    {
        protected override async Task Initialize()
        {
            SampleClockService clockService = new SampleClockService();

            // Simulate asynchronous startup work (asset loading, sign-in...).
            await Task.Delay(500);

            MxLog.L($"Game initialization completed at {clockService.Now}");
        }
    }
}
