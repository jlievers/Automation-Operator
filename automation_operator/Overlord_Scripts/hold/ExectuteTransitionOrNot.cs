using System;
using System.IO;
using System.Windows.Forms;

namespace Script
{
public class ScriptClass {
     static public void ExecuteCode()
     {
          try
		{
			[go_ahead] = false;	
			string myRunOutputPath = "Q:\\" + [ProcessFolder].ToString() + "\\" + [SystemFolder].ToString() + "\\RunOutputFiles\\" + [RunID].ToString() + ".json";
			if (File.Exists(myRunOutputPath))
			{
				string JSONFileText = File.ReadAllText(myRunOutputPath);
				DLL_RunOutput.RunInfo_DartPostProcesses myRunInfo = ser.Deserialize<DLL_RunOutput.RunInfo_DartPostProcesses>(JSONFileText);
				foreach (DLL_RunOutput.OperationData_DartPostProcesses myPlate in myRunInfo.operation_data_dart_post_processes)
               	{
                    	if (myPlate.plate_group_name == myCurrentStepObject.plate_name && myPlate.status == "active")
					{
						[go_ahead] = true;
						[current_reagent_set_index] = myPlate.reagent_set_index;
					}
                    }
			}
		}
		catch(Exception Ex)
		{
			throw new Exception ("Error in script 'Decide What To Do'. Run Output file not found: " + Ex.Message);
		}	
     }
}
}