using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicConfirmation
{
	public class SystemAction
	{
		public bool isValid { get; set; }
		public string action { get; set; }	//run, teardown, setup, getPlate, putPlate,
		public string task { get; set; }
		public string device_name { get; set; }
		public string lm_number { get; set; }
		public int stack_num { get; set; }
		public int nest_num { get; set; }
		public string plate_barcode { get; set; }
		public string plate_type { get; set; }
	}
}
