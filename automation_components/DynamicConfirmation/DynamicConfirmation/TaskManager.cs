using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicConfirmation
{
	public class JobQueue
	{
		public List<Job> jobs { get; set; }
	}

	public class Job
	{
		public string description { get; set; }
		public string jobID { get; set; }
		public string userID { get; set; }
		public string jobStatus { get; set; }
		public List<TaskList> tasks { get; set; }
		public List<Plate> reagents { get; set; }
	}

	public class TaskList
	{
		public string task { get; set; }
		public string device_name { get; set; }
		public string instrumentID { get; set; }
		public string status { get; set; }
		public List<Plate> reagents { get; set; }
		public List<Plate> labware { get; set; }
	}

	public class Plate
	{
		public string barcode { get; set; }
		public string type { get; set; }
		public int targetNest { get; set; }
		public bool consumed { get; set; }
	}
}
