using System;

namespace MxUnikit.Core.Samples.SampleCoreBootstrap
{
    public class SampleClockService
    {
        public string Now => DateTime.Now.ToString("HH:mm:ss");
    }
}
