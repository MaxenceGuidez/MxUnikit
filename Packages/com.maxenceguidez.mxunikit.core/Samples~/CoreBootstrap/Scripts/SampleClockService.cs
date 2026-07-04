using System;

namespace MxUnikit.Core.Samples
{
    public class SampleClockService
    {
        public string Now => DateTime.Now.ToString("HH:mm:ss");
    }
}
