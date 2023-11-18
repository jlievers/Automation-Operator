using System;
using System.IO;
using System.Windows.Forms;
using System.Collections.Generic;
REF "System.Web.Extensions.dll"
using System.Web.Script.Serialization;
using System.Web;
using System.Net;
using System.Threading;
REF "C:\Program Files (x86)\PAA\Overlord3\DLL_Inventory.dll"
REF "C:\Program Files (x86)\PAA\Overlord3\DLL_RunOutput.dll"
REF "C:\Program Files (x86)\PAA\Overlord3\Newtonsoft.Json.dll"
using System.Linq;

namespace Script
{
public class ScriptClass {
     static public void ExecuteCode()
     {
          System.Web.Script.Serialization.JavaScriptSerializer ser = new System.Web.Script.Serialization.JavaScriptSerializer();
		List<string> myProcesses = new List<string>();
		Step myCurrentStepObject = new Step();
		string mySchedulePath = "Q:\\SYSTEMS\\" + [system_folder].ToString() + "\\labSchedule.json";
		Run myRun = ser.Deserialize<Run>(File.ReadAllText(mySchedulePath));
		try
		{
			foreach(Step oneStep in myRun.schedule)
			{
				if(!myProcesses.Contains(oneStep.process))
				{
					myProcesses.Add(oneStep.process);
				}
			}
			List<string> myPreProcesses = new List<string>();
			foreach(string oneProcess in myProcesses)
			{
				string path = "P:\\" +  oneProcess + "\\Overlord_Procedures\\Transitions\\PreMethod.ovp";
				if (File.Exists(path))
				{
					myPreProcesses.Add(path);
				}
			}
			[integer] = myPreProcesses.Count -1;
			[object_var] = myPreProcesses;
		}
		
		catch(Exception Ex)
		{
			throw new Exception ("Error reading schedule: " + Ex.Message);
		}
     }
	
	//Classes
	//////////////////////////////////////////////////////////////////
	
    public class Instrument
    {
        public int index { get; set; }
        public string name { get; set; }
    }

    public class Step    
    {
   	public int end_time { get; set; }
        public List<Instrument> instrument { get; set; }
        public List<int> plate_index { get; set; }
	   public List<string> plate_name { get; set; }
        public string process { get; set; }
        public int start_time { get; set; }
	   public string step { get; set; }
    }

    public class Run
    {
        public int current_step { get; set; }
        public List<Step> schedule { get; set; }
    }
}
}