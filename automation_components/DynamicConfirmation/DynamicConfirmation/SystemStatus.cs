using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicConfirmation
{
	//public class Resources
	//{
	//	public string systemID { get; set; }
	//	public List<Instrument> instruments { get; set; }
	//}

	//public class Instrument
	//{
	//	public string instrumentType { get; set; }
	//	public string instrumentName { get; set; }
	//	public string instrumentID { get; set; }
	//	public string instrumentStatus { get; set; }
	//	public List<Shelf> nests { get; set; }
	//	public List<Rack> racks { get; set; }
	//}

	//public class Rack
	//{
	//	public int rackNumber { get; set; }
	//	public int count { get; set; }
	//	public string type { get; set; }
	//	public List<Shelf> shelves { get; set; }
	//}

	//public class Shelf
	//{
	//	public int shelfNumber { get; set; }
	//	public string plateBarcode { get; set; }
	//	public string type { get; set; }
	//}

	public class SystemInventory
	{
		public string system_name { get; set; }
		public string system_lm { get; set; }
		public List<Device> devices { get; set; }
	}

	public class Device
	{
		public string device_name { get; set; }
		public string device_type { get; set; }
		public string lm_number { get; set; }
		public string status { get; set; }
		public bool enabled { get; set; }
		public int num_nests { get; set; }
		public List<Stack> stacks { get; set; }
		public List<Nest> nests { get; set; }
	}

	public class Stack
	{
		public int stack_num { get; set; }
		public int count { get; set; }
		public string type { get; set; }
		public List<Nest> nests { get; set; }
	}

	public class Nest
	{
		public int nest_num { get; set; }
		public string workflow_id { get; set; }
		public string plate_barcode { get; set; }
		public string type { get; set; }
		public string priority { get; set; }
	}
}