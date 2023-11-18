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
          	List<string> myProcesses = (List<string>)[object_var];
			int loopCounter = Convert.ToInt32([loop_counter]);
			[current_step_name] =  myProcesses[loopCounter];
     	}
	}
}