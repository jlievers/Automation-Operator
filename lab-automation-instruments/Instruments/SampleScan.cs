using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com.Invitae.LabAutomation
{
    public static class SampleScan
    {
    }

    public class TubeScan
    {
        public string rack_barcode { get; set; }
        public List<ScannedTube> scanned_tubes { get; set; }
    }

    public class ScannedTube
    {
        public string barcode { get; set; }
        public int row { get; set; }
        public int column { get; set; }
        public int status { get; set; }
    }
}
