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
		
     	static public void ExecuteCode()
     	{
     		System.Web.Script.Serialization.JavaScriptSerializer ser = new System.Web.Script.Serialization.JavaScriptSerializer();
			bool doSomething = false; //This variable stays false when the current_step plate_group_name entry is not active. It can be true however if the previous is true AND there are no active plates.
			//This is because we want to execute the first transition of a process but go into the schedule holding pattern by setting [GoAhead] = false.  
			
			Step myCurrentStepObject = new Step();
			[end] = false; 
			//First Read Schedule
			try
			{
				string mySchedulePath = "Q:\\DaRT_PostProcesses\\" + [SystemFolderPath].ToString() + "\\Schedules\\labSchedule.json";
				Run myRun = ser.Deserialize<Run>(File.ReadAllText(mySchedulePath));
				int myCurrentStepIndex = myRun.current_step;
				if(myCurrentStepIndex > (myRun.schedule.Count -1))
				{
					[end] = true;
					return;
				}
				myCurrentStepObject = myRun.schedule[myCurrentStepIndex];
			}
			catch(Exception Ex)
			{
				throw new Exception ("Error in script 'Decide What To Do'. Error reading schedule: " + Ex.Message);
			}
			
			
			//Read RunOutput, Determine if there is an active plateGroup for the current schedule step, Also determine if there are ANY active plate groups
			bool activePlateGroup = false;
			try
			{
				
				string myRunOutputPath = "Q:\\DaRT_PostProcesses\\" + [SystemFolderPath].ToString() + "\\RunOutputFiles\\" + [RunID].ToString() + ".json";
				if (File.Exists(myRunOutputPath))
				{
					string JSONFileText = File.ReadAllText(myRunOutputPath);
					DLL_RunOutput.RunInfo_DartPostProcesses myRunInfo = ser.Deserialize<DLL_RunOutput.RunInfo_DartPostProcesses>(JSONFileText);///Process Specific!///
					foreach (DLL_RunOutput.OperationData_DartPostProcesses myPlate in myRunInfo.operation_data_dart_post_processes)
               		{
                    		if (myPlate.plate_group_name == myCurrentStepObject.plate_name && myPlate.status == "active")
						{
							doSomething = true;
							[current_reagent_set_index] = myPlate.reagent_set_index;
						}
						if (myPlate.status == "active")
			     		{
			      	    	activePlateGroup = true; //There is at least one active plate group. We need to know this for the "hold at step1 feature"
						} 
                    	}
				}
			}
			catch(Exception Ex)
			{
				throw new Exception ("Error in script 'Decide What To Do'. Error reading Run Output file: " + Ex.Message);
			}	
			

               //Determine whether or not to jump into "wait for plate feature" basically
			[Pos.StackShelf] = -1;
			[go_ahead] = true;	
			if(myCurrentStepObject.step == "DartPP_A_Exo1")//The first step for a plate  ///Process Specific!///
			{
					
				//doSomething = true; 
				//Look For Plates in stack1 Inventory to start out
				try
				{
					int i = 0;
					while (!IsFileReady("Q:\\SystemInventory\\" + [SystemFolderPath].ToString() + "\\SystemInventory.json") && i < 10)
               		{
               			i++;
                    		Thread.Sleep(1000);
               		}
					string JSONFileText = File.ReadAllText("Q:\\SystemInventory\\" + [SystemFolderPath].ToString() + "\\SystemInventory.json");
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
							 doSomething = true;
							[Barcode]=  _myNest.plate_barcode;
							[Pos.StackShelf] = _myNest.nest_number;
                   	 	}
               		}
					//Enable wait for plate feature. Jump into the "wait for plate to enter stack1 loop" in 1st transition if:
					//1. It is the first step 
					//2. There are no plates in the input stacker
					//3. There are no active plate groups 
					//Otherwise pass by plate transition group and do not "add an active plate to ferris wheel car"
					if(Convert.ToInt32([Pos.StackShelf]) == -1 && !activePlateGroup)
					{
						[go_ahead] = false; //This allows entry into the wait loop in Transition 1 
						doSomething = true; 
					}
				}
				catch(Exception Ex)
				{
					throw new Exception ("Error in script 'Decide What To Do'. Error reading Stack1 inventory: " + Ex.Message);
				}	
			}
			
			
			
			////////////////////////////DO SOMETHING OR NOT, IF NOT the timer will run
			[current_step_name] = "";
			if(doSomething)
			{
				try
				{
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
					[current_step_name] = myCurrentStepObject.step;
         	 		[plate_group_name] = myCurrentStepObject.plate_name;
				}
				catch(Exception Ex)
				{
					throw new Exception ("Error in script 'Decide What To Do'. Error setting runtime variables: " + Ex.Message);
				}
			}
    	 }
	}
}