using System;
using System.IO;
using System.Windows.Forms;

namespace Script
{
public class ScriptClass {
     static public void ExecuteCode()
     {
        [run_id] =  DateTime.Today.ToShortDateString().Replace('/','_');
     }
}
}