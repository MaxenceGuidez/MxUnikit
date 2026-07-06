using System.Threading.Tasks;
using MxUnikit.Log;

namespace MxUnikit.Core.Samples.SampleCoreBootstrap
{
    public class SampleBootstrapper : MxBootstrapper
    {
        protected override async Task Preload()
        {
            // Simulate connecting to backend services (Unity Services, Firebase...).
            await Task.Delay(500);

            MxLog.L("Preload completed.");
        }
    }
}
