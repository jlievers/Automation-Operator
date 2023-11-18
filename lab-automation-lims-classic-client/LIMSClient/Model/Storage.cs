using System.Collections.Generic;

namespace Com.Invitae.LabAutomation.Model
{

    public class SystemInventory
    {
        public string automation_system { get; set; }
        public List<StorageDevice> storage_devices { get; set; }
    }

    public class StorageDevice
    {
        public string device_name { get; set; }
        public string device_type { get; set; }
        public string lm_number { get; set; }
        public string enabled { get; set; }
        public string companion_system { get; set; }
        public List<Nest> nests { get; set; }
    }

    public class Nest
    {
        public int nest_number { get; set; }
		public string workflow_id { get; set; }
        public string plate_barcode { get; set; }
        public string priority { get; set; }
    }
}
