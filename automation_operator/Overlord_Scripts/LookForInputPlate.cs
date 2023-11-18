REF "System.Web.Extensions.dll"
using System;
using System.IO;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using System.Web;
using System.Threading;
REF "C:\Program Files (x86)\PAA\Overlord3\Dll_Inventory.dll"
REF "C:\Program Files (x86)\PAA\Overlord3\DLL_RunOutput.dll"

namespace Script
{
	public class ScriptClass 
	{
	     static public void ExecuteCode()
	     {
			System.Web.Script.Serialization.JavaScriptSerializer ser = new System.Web.Script.Serialization.JavaScriptSerializer();
			[go_ahead]= false;
			
			//Get All Processes In Schedule
			string mySchedulePath = "Q:\\SYSTEMS\\" + [system_folder].ToString() + "\\labSchedule.json";
			Run myRun = ser.Deserialize<Run>(File.ReadAllText(mySchedulePath));
			int i = 0;
			Dictionary<string,int> myProcesses = new Dictionary<string,int>();//Process name, first step index
			foreach(Step oneStep in myRun.schedule)
			{
				if(!myProcesses.ContainsKey(oneStep.process))
				{
					myProcesses.Add(oneStep.process, i);
				}
				i++;
			}
			
			//Get Stack 1
			i = 0;
			while (!IsFileReady("Q:\\SYSTEMS\\" + [system_folder].ToString() + "\\SystemInventory.json") && i < 10)
               {
               	i++;
                    Thread.Sleep(1000);
               }
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
			//Get Nests Sort by Priority
			   SortedDictionary<int, Dll_Inventory.Nest> mySortedNests = new SortedDictionary<int, Dll_Inventory.Nest>();
               foreach (Dll_Inventory.Nest myNest in myDevice.nests)
               {
               	if (myNest.plate_barcode != null)
                    {
                    	mySortedNests.Add(Convert.ToInt16(myNest.priority), myNest);
               	}
				if(mySortedNests.Count < 1)
				{
					return;
				}
			}

			//loop through list of processes, The process that is first in the schedule will be first in the list
			int currentStepNum = 0;
			bool found = false;
			foreach(KeyValuePair<string,int> oneProcess in myProcesses)
			{
				foreach(KeyValuePair<int, Dll_Inventory.Nest> oneNest in mySortedNests)
				{
					Dll_Inventory.Nest myNest = oneNest.Value;//nest object from Dict
					if(oneProcess.Key == myNest.assay_id && !found)//assay _id id manually input to nest object using Inventory Tool
					{
						[Barcode]=  myNest.plate_barcode;
						[current_nest_number] = myNest.nest_number;
						[go_ahead]= true;
						found = true;
						currentStepNum = oneProcess.Value -1;//This is because the script inc value will run in main loop and artifically advance
					}
				}
			}
			myRun.current_step = currentStepNum;
			string JSONOutput = ser.Serialize(myRun).ToString();
			File.WriteAllText(mySchedulePath,JSONOutput);
		}
		
	
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////					
	///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////	
				
			
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