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
		
		
          string myRunOutputPath = "Q:\\" + [process_folder].ToString() + "\\" + [system_folder].ToString() + "\\RunOutputFiles\\" + [run_id].ToString() + ".json";
		//myRunOutputPath = "Q:\\DaRT_PostProcesses\\Harmonic\\RunOutputFiles\\jeff_test.json";
		string JSONFileText = File.ReadAllText(myRunOutputPath);
		Newtonsoft.Json.Linq.JObject myRunInfo = Newtonsoft.Json.Linq.JObject.Parse(JSONFileText);
		
		IList<string> keys = myRunInfo.Properties().Select(p => p.Name).ToList();
          var myObj = myRunInfo[keys[0]];
		
		foreach (var onevar in myObj)
          {
          	onevar["status"] = "xxx";
          }
		
          string json = Newtonsoft.Json.JsonConvert.SerializeObject(myRunInfo);
          File.WriteAllText(myRunOutputPath, json);
		
		
		
     }
}
}