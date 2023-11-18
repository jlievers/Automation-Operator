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
			//This script creates several variables for consumption; 
			//1. [end] is set if we are at the end of the schedule. Exit loop and run Outro procedure
			//2. [current_reagent_set_index] comes from run output query for a currently active "plate_group_name"
			//3. [current_step_name] comes from parsing the current schedule step. Current step name resolves to which actual Transition(.ovp) to run. If This variable == "" at the end,do nothing and loop
			//4. [plate_group_name] comes from parsing the current schedule step. This allows lookup of run output objects. Plate_group_name is a primary key to a runOutput object
			//5. [bravo_index],[odtc_index],[ligation_station_index] comes from parsing the current schedule step. These tell the robot what pooled index to use
			//6. [barcode] and [current_nest_number] are set from an Inventory query of stack1

			//There are three process routes that can be taken after the script. 1. Execute a Transition. 2. Go into the system holding pattern. 3. Do Nothing, go on to the next step of the schedule

     		System.Web.Script.Serialization.JavaScriptSerializer ser = new System.Web.Script.Serialization.JavaScriptSerializer();
			Dictionary<string,string> myProcesses = new Dictionary<string,string>();//Dict of assays present in the schedule and the names of their first steps
			Step myCurrentStepObject = new Step();
			
			try
			{
				string mySchedulePath = "Q:\\SYSTEMS\\" + [system_folder].ToString() + "\\labSchedule.json";
				Run myRun = ser.Deserialize<Run>(File.ReadAllText(mySchedulePath));
				myCurrentStepObject = myRun.schedule[myRun.current_step];
				
				[current_step_name] = "P:\\" +  myCurrentStepObject.process + "\\" + [SystemFolder]+ "\\" + myCurrentStepObject.step + ".ovp"; 
         	 	[plate_group_name] = myCurrentStepObject.plate_name;
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
					 if (myInst.name.Contains("ligation_station"))
                	     {
                    		[ligation_station_index] = myInst.index +1;
                	     }
	     		}
				//Get list of processes in schedule
				foreach(Step oneStep in myRun.schedule)
				{
					if(!myProcesses.ContainsKey(oneStep.process))
					{
						myProcesses.Add(oneStep.process, oneStep.step);
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
					string JSONFileText = File.ReadAllText(myRunOutputPath);
					Newtonsoft.Json.Linq.JObject myRunInfo = Newtonsoft.Json.Linq.JObject.Parse(JSONFileText);
					IList<string> keys = myRunInfo.Properties().Select(p => p.Name).ToList();
          			var myObj = myRunInfo[keys[0]];
		
					foreach (var onevar in myObj)
          			{
						if (onevar["plate_group_name"] == myCurrentStepObject.plate_name && onevar["status"] == "active")
						{
							[current_reagent_set_index] = onevar["reagent_set_index"]; 
							plateGroupIsActive = true;
						}
						if (onevar["status"] == "active")
			     		{
			      	    	anyActivePlateGroup = true; //There is at least one active plate group, so do not go into holding pattern
          				}
					}
				}
			}
			catch(Exception Ex)
			{
				throw new Exception ("Error reading Run Output: " + Ex.Message);
			}	
				
			//Read Inventory, Look For Input Plates In stack1
			string[] myArray = new string[2];//Index 1 = barcode, Index 2 = stack shelf
			try
			{    
				string startTime = myCurrentStepObject.start_time.ToString();
				myArray = GetNextPlateAndPosFromInputStack();
				if(myArray[0]!=null)
				{
					[barcode]=  myArray[0];
					[current_nest_number] = Convert.ToInt32(myArray[1]);
				}
			}
			catch(Exception Ex)
			{
				throw new Exception ("Error reading Stack1 inventory: " + Ex.Message);
			}
			
			
			//////////NOW MAKE DECISION WHAT TO DO BASED ON COLLECTED INFO//////////
			[end] = false;
			if(myRun.current_step > (myRun.schedule.Count - 1))
			{
				[end] = true;
			}
			//If there is not an active plate in the RunOutput for the step, skip it. Unless, It is the first step of a schedule and there is a new plate to start in the stacker		
			if(!plateGroupIsActive &&(!myArray[0] == null && myProcesses.ContainsValue(myCurrentStepObject.step))) 
			{
				[current_step_name] = "";	
			}
			//If there are no active plate groups at all and there is nothing in the input stack, then go into the holding pattern	
			if(myArray[0] == null && !anyActivePlateGroup) 
			{
				[current_step_name] = "P:\\system_init\\" + [SystemFolder]+ "\\WaitForPlate.ovp";
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
					
		private static string[] GetNextPlateAndPosFromInputStack()
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
 	         foreach (Dll_Inventory.Nest _myNest in myDevice.nests)
               {
               	if (Convert.ToInt16(_myNest.priority) == 1)
                    {
					myArray[0]=  _myNest.plate_barcode;
					myArray[1] = _myNest.nest_number.ToString();
                    }
               }
			return myArray;
		}						
	}
}