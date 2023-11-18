using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace DLL_Instruments
{
    public static class Verso
    {
        public static bool GetVersoSetting(string settingName, string invFilePath)
        {
            bool myRetVal = false;
            System.Web.Script.Serialization.JavaScriptSerializer ser = new System.Web.Script.Serialization.JavaScriptSerializer();
            string JSONFileText = File.ReadAllText(invFilePath);
            VersoSettings myInfo = ser.Deserialize<VersoSettings>(JSONFileText);
            myRetVal = Convert.ToBoolean(myInfo.GetType().GetProperty(settingName).GetValue(myInfo));
            return myRetVal;
        }
    }

    public class VersoSettings
    {
        public bool verso_after_norm { get; set; }
        public bool verso_after_stamp { get; set; }
        public bool pull_new_batches_from_verso { get; set; }
    }
}
