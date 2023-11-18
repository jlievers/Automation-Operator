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
	public class ScriptClass 
	{
     	static public void ExecuteCode()
     	{
			// This script creates several variables for consumption; 
			// [current_step_name] comes from parsing the current schedule step. [current_step_name] resolves to which actual Transition(.ovp) to run. If This variable == "" at the end,do nothing and loop
			// [plate_group_name] comes from parsing the current schedule step. This allows lookup of run output objects. Plate_group_name is a primary key to a RunOutput object. 
			// [process_folder]
			
			// [end] is set if we are at the end of the schedule. Exit loop and run Outro procedure
			// [bravo_index] and [odtc_index] call out which of these pooled resources should be used by the step
			// [current_nest_number] is a bonus var that gives the insex of the input plate in stack1 when the current schedule step is a starter step. 
			// This is handy because you dont need to find it in the forst transition of a new process plate
			

			//There are three process routes that can be taken after the script. 1. Execute a Transition. 2. Go into the system holding pattern. 3. Do Nothing, go on to the next step of the schedule

     		System.Web.Script.Serialization.JavaScriptSerializer ser = new System.Web.Script.Serialization.JavaScriptSerializer();
			Dictionary<string,string> myProcesses = new Dictionary<string,string>();//Dict of assays present in the schedule and the names of their first steps
			Step myCurrentStepObject = new Step();
			string mySchedulePath = "Q:\\SYSTEMS\\" + [system_folder].ToString() + "\\labSchedule.json";
			Run myRun = ser.Deserialize<Run>(File.ReadAllText(mySchedulePath));
			myCurrentStepObject = myRun.schedule[myRun.current_step];
			[process_folder]= myCurrentStepObject.process;
			try
			{
				[current_step_name] = "P:\\" +  myCurrentStepObject.process + "\\Overlord_Procedures\\Transitions\\" + myCurrentStepObject.step + ".ovp"; 
         	 	[plate_group_name] = myCurrentStepObject.plate_name[0];
				
				//Get Instrument indexes
          		foreach (Instrument myInst in myCurrentStepObject.instrument)
				{
           			if (myInst.name.Contains("bravo"))
                		{
                    		[bravo_index] = myInst.index + 1;
                		}
				 	if (myInst.name.Contains("odtc"))
                		{
                    		[odtc_index] = myInst.index +1;
                		}
	     		}
				//get list of processes and the names of their 'starter steps'
				foreach(Step oneStep in myRun.schedule)
				{
					if(!myProcesses.ContainsKey(oneStep.process))
					{
						myProcesses.Add(oneStep.process, oneStep.step);
					}
				}
				
				if(myProcesses.ContainsValue(myCurrentStepObject.step))//current step is a starter step so must wait for and then reset Cadence timer
				{
					//Wait for Cadence
					var timerName = "Cadence";
					var parametersTemplate = "<timerCommand><waitFor><timer>{0}</timer><hideRuntimeForm>False</hideRuntimeForm></waitFor></timerCommand>";
					var parameters = string.Format(parametersTemplate, timerName);
					var commandId = Overlord.Utilities.Core.Commands.GetCommandIDFromName("Timers");
					var action = new Overlord.Model.Action(parameters);
					Overlord.Utilities.Core.Commands[commandId].Runtime(action);
					
					//Reset Cadence -> first get indexes of all 'starter steps'
					List<int> newPlateEntryIndexes = new List<int>();
					int i = 0;
					foreach(Step oneStep in myRun.schedule)
					{
						if(myProcesses.ContainsValue(oneStep.step))
						{
							newPlateEntryIndexes.Add(i);
						}
						i++;
					}
					//Now figure out what the 'schedule step index' of the next starter step, get its start time and subtract the start time of the current starterStep
					int indexesIndex = newPlateEntryIndexes.IndexOf(myRun.current_step);
					if(indexesIndex != newPlateEntryIndexes.Count-1)//Is it the last starter step?
					{
						int stepIndexOfNextStarterStep = newPlateEntryIndexes[indexesIndex + 1];
						Step myNextStarterStepObject = myRun.schedule[stepIndexOfNextStarterStep];
						int cadenceSetTime = myNextStarterStepObject.start_time -  myCurrentStepObject.start_time;
				
						//set Cadence timer
						var myTimer = Overlord.Utilities.Commands.Timers.TimersInteraction.GetTimer("Cadence");
						var now = DateTime.Now;
						myTimer.StartDate = now;
						myTimer.StopDate = now.AddSeconds(cadenceSetTime);
						Overlord.Utilities.Commands.Timers.TimersInteraction.SaveTimer(myTimer);
					}
				}
			}
			catch(Exception Ex)
			{
				throw new Exception ("Error reading schedule: " + Ex.Message);
			}
			//Read RunOutput, Determine if there is an active plateGroup for the current schedule step, Also determine if there are ANY active plate groups
			bool anyActivePlateGroup = false;
			bool plateGroupIsActive = false;
			try
			{
				foreach(string oneProcess in myProcesses.Keys)
				{
					string myRunOutputPath = "Q:\\" + oneProcess + "\\" + [system_folder].ToString() + "\\RunOutputFiles\\" + [run_id].ToString() + ".json";
					if (File.Exists(myRunOutputPath))
					{
						string JSONFileText = File.ReadAllText(myRunOutputPath);
						Newtonsoft.Json.Linq.JObject myRunInfo = Newtonsoft.Json.Linq.JObject.Parse(JSONFileText);
						IList<string> keys = myRunInfo.Properties().Select(p => p.Name).ToList();
          				var myObj = myRunInfo[keys[0]];
		
						foreach (var onevar in myObj)
          				{
							if (onevar["plate_group_name"].ToString() == myCurrentStepObject.plate_name[0] && onevar["status"].ToString() == "active" && oneProcess == myCurrentStepObject.process )
							{
								plateGroupIsActive = true;
							}
							if (onevar["status"].ToString() == "active")
			     			{
			      	    		anyActivePlateGroup = true; //There is at least one active plate group, so do not go into holding pattern
          					}
						}
					}
					//else
					//{
					//	throw new Exception ("Run Output file does not exist for process: " + myCurrentStepObject.process);
					//}
				}
			}
			catch(Exception Ex)
			{
				throw new Exception ("Error reading Run Output: " + Ex.Message);
			}	
			//Read Inventory, Look For Input Plates In stack1
			string[] myArray = new string[2];//Index 1 = barcode, Index 2 = stack shelf
			myArray = GetNextPlateAndPosFromInputStack(myCurrentStepObject.process);
			//////////NOW MAKE DECISION WHAT TO DO BASED ON COLLECTED INFO//////////
			[end] = false;
			if(myRun.current_step > (myRun.schedule.Count - 2))
			{
				[end] = true;
			}
			//If there is not an active plate in the RunOutput for the step, skip it. Unless, It is the first step of a schedule and there is a new plate to start in the stacker. Then don't set [current_step_name] = ""
			bool starterStep = myProcesses.ContainsValue(myCurrentStepObject.step);
			bool platePresent = (myArray[0] != null);
			bool starterStepWithPlatePresent = starterStep && platePresent;
			
			//MessageBox.Show(plateGroupIsActive.ToString() + "," + starterStep.ToString() + "," + platePresent.ToString());	
			
			if(!plateGroupIsActive && !starterStepWithPlatePresent) 
			{
				[current_step_name] = "";	
			}
			//If there are no active plate groups at all and there is nothing in the input stack, then go into the holding pattern	
			if(!platePresent && !anyActivePlateGroup) 
			{
				[current_step_name] = "P:\\automation_operator\\Overlord_Procedures\\WaitForPlate.ovp";
				
				var myTimer = Overlord.Utilities.Commands.Timers.TimersInteraction.GetTimer("Cadence");
				var now = DateTime.Now;
				myTimer.StopDate = now;
				Overlord.Utilities.Commands.Timers.TimersInteraction.SaveTimer(myTimer);
			}
			//Log transition start time
			if([current_step_name].ToString() != "P:\\automation_operator\\Overlord_Procedures\\WaitForPlate.ovp" && [current_step_name].ToString() != "")
			{
				 string transitionLogPath  = "Q:\\SYSTEMS\\" + [system_folder].ToString() + "\\TransitionLog.csv";
		 		string lineToWrite = myCurrentStepObject.process  + "," + [run_id] + [current_step_name] + "," + [plate_group_name] + ",START," + DateTime.Now.ToString("yyyy.MM.dd_HH:mm:ss")+ Environment.NewLine;
           		File.AppendAllText(transitionLogPath,lineToWrite);
			}	
    	 }
					
			
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////					
	///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////				
					
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
		
	    private static bool IsFileReady(string sFilename)
         {
         	try
              {
              	using (FileStream inputStream = File.Open(sFilename, FileMode.Open, FileAccess.Read, FileShare.None))
                   return inputStream.Length > 0;
              }
              catch (Exception)
              {
              	return false;
              }
         }
					
		private static string[] GetNextPlateAndPosFromInputStack(string processType)
		{
			System.Web.Script.Serialization.JavaScriptSerializer ser = new System.Web.Script.Serialization.JavaScriptSerializer();
			int i = 0;
			while (!IsFileReady("Q:\\SYSTEMS\\" + [system_folder].ToString() + "\\SystemInventory.json") && i < 10)
               {
               	i++;
                    Thread.Sleep(1000);
               }
			string[] myArray = new string[2];
			string JSONFileText = File.ReadAllText("Q:\\SYSTEMS\\" + [system_folder].ToString() + "\\SystemInventory.json");
               Dll_Inventory.SystemInventory myInfo_si = ser.Deserialize<Dll_Inventory.SystemInventory>(JSONFileText);
               Dll_Inventory.Device myDevice = new Dll_Inventory.Device();
               foreach (Dll_Inventory.Device _myDevice in myInfo_si.devices)
               {
               	if (_myDevice.device_name == "stack1")
                    {
                     	myDevice = _myDevice;
                    }
               }
			SortedDictionary<int, Dll_Inventory.Nest> mySortedNests = new SortedDictionary<int, Dll_Inventory.Nest>();
               foreach (Dll_Inventory.Nest myNest in myDevice.nests)
               {
               	if (myNest.plate_barcode != null)
                    {
                    	mySortedNests.Add(Convert.ToInt16(myNest.priority), myNest);
               	}
			}
			if(mySortedNests.Count < 1)
			{
				return myArray;
			}
			bool found = false;
			foreach(KeyValuePair<int, Dll_Inventory.Nest> oneNest in mySortedNests)
			{
				Dll_Inventory.Nest myNest = oneNest.Value;//nest object from Dict
				if(processType == myNest.assay_id && !found)
				{
					myArray[0]=  myNest.plate_barcode;
					myArray[1] = myNest.nest_number.ToString();
					found = true;
					[current_nest_number] = myNest.nest_number;
				}
			}
			return myArray;
		}					
	}
}