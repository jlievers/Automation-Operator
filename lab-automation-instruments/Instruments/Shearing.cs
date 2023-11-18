using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Com.Invitae.LabAutomation
{
    public static class Shearing
    {
        public static bool GetShearingSetting(string settingName, string invFilePath)
        {
            bool myRetVal = false;
            System.Web.Script.Serialization.JavaScriptSerializer ser = new System.Web.Script.Serialization.JavaScriptSerializer();
            string JSONFileText = File.ReadAllText(invFilePath);
            ShearingSettings myInfo = ser.Deserialize<ShearingSettings>(JSONFileText);
            myRetVal = Convert.ToBoolean(myInfo.GetType().GetProperty(settingName).GetValue(myInfo));
            return myRetVal;
        }
    }

    public class ShearingSettings
    {
        public bool standard_mode { get; set; }
        public bool ninetysix_mode { get; set; }
        public bool batch_available_mode { get; set; }
    }
}

