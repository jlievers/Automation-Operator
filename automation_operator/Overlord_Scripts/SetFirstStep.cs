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
			myRun.current_step = Convert.ToInt32([integer]) - 1;
			string JSONOutput = ser.Serialize(myRun).ToString();
			File.WriteAllText(myPath,JSONOutput);
	
    	 }
	}
}