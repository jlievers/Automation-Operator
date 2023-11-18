using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicConfirmation
{
	public class ProtocolTaskList
	{
		public List<Protocol> protocols { get; set; }
	}

	public class Protocol
	{
		public string protocol_name { get; set; }
		public List<Task> tasks { get; set; }
	}

	public class Task
	{
		public int task_num { get; set; }
		public string task { get; set; }
		public string device_name { get; set; }
		public List<PlateData> reagent_list { get; set; }
		public List<PlateData> labware_list { get; set; }
	}

	public class PlateData
	{
		public string type { get; set; }
		public int nest { get; set; }
	}
}
