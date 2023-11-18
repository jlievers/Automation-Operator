using System;
using gDNA_LIMS_COM;

namespace Test_Console
{
    class Program
    {
        static void Main(string[] args)
        {
            LIMSCalls.initializeExtractionBatch("lims.locusdev.net", "C:\\FakeQ\\Testing\\", "testrun", "IB3130246", "IB4434834");
        }
    }
}
