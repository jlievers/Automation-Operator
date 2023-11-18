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
		[go_ahead] = false;
          System.Web.Script.Serialization.JavaScriptSerializer ser = new System.Web.Script.Serialization.JavaScriptSerializer();
		Dictionary<string,string> myProcesses = new Dictionary<string,string>();//Dict of assays present in the schedule and the names of their first steps
		Step myCurrentStepObject = new Step();
		string mySchedulePath = "Q:\\SYSTEMS\\" + [system_folder].ToString() + "\\labSchedule.json";
		Run myRun = ser.Deserialize<Run>(File.ReadAllText(mySchedulePath));
		myCurrentStepObject = myRun.schedule[myRun.current_step];
				
		//get list of processes and the names of their 'starter steps'
		foreach(Step oneStep in myRun.schedule)
		{
			if(!myProcesses.ContainsKey(oneStep.process))
			{
				myProcesses.Add(oneStep.process, oneStep.step);
			}
		}	
		if(myProcesses.ContainsValue(myCurrentStepObject.step)&& myRun.current_step != 0)//current step is a starter step so must wait for 'Cadence' timer
		{
			[go_ahead] = true;
		}
	}

	public class Instrument
    	{
        	public int index { get; set; }
        	public string name { get; set; }
   	 }

    	public class Step    
	    {
       	 public int end_time { get; set; }
        	public List<Instrument> instrument { get; set; }
        	public int plate_index { get; set; }
	        public string plate_name { get; set; }
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