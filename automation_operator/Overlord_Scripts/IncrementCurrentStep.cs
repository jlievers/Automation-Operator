using System;
using System.IO;
using System.Windows.Forms;
using System.Collections.Generic;
REF "System.Web.Extensions.dll"
using System.Web.Script.Serialization;
using System.Web;
using System.Net;
REF "C:\Program Files (x86)\PAA\Overlord3\DLL_Inventory.dll"

namespace Script
{
	public class ScriptClass 
	{
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
		
     	static public void ExecuteCode()
     	{
     		System.Web.Script.Serialization.JavaScriptSerializer ser = new System.Web.Script.Serialization.JavaScriptSerializer();
			string myPath = "Q:\\SYSTEMS\\" + [system_folder].ToString() + "\\labSchedule.json";
		     Run myRun = ser.Deserialize<Run>(File.ReadAllText(myPath));
			myRun.current_step = myRun.current_step + 1;
			string JSONOutput = ser.Serialize(myRun).ToString();
			File.WriteAllText(myPath,JSONOutput);
			
			
			//I do not think setting [end] = true in whattodo.cs is working. Also setting the loop to "start of loop" does not work. Setting [end] = true here in incrementstep.cs does work. Jeff to check.
			int myCurrentStepIndex = myRun.current_step;
			if(myCurrentStepIndex > (myRun.schedule.Count - 1))
			{
				[end] = true;
			}
			
			//Log transition start time
			if([current_step_name].ToString() != "P:\\automation_operator\\Overlord_Procedures\\WaitForPlate.ovp" && [current_step_name].ToString() != "")
			{
				 string transitionLogPath  = "Q:\\SYSTEMS\\" + [system_folder].ToString() + "\\TransitionLog.csv";
		 		string lineToWrite = [process_folder]  + "," + [run_id] + [current_step_name] + "," + [plate_group_name] + ",END," + DateTime.Now.ToString("yyyy.MM.dd_HH:mm:ss")+ Environment.NewLine;
           		File.AppendAllText(transitionLogPath,lineToWrite);
			}			
    	 }
	}
}