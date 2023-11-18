using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Windows.Forms;
using System.Threading;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using System.Security.Permissions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DLL_RunOutput
{
    public static class Utilities
    {
        public static string OpenRunOutputFile(string systemFolderPath,string systemFolderPathExtension, string runID)  //open json file and return text
        {
            string JSONFileText = "";
            string path = systemFolderPath + "\\RunOutputFiles\\" + systemFolderPathExtension + "\\" + runID + ".json";
            try
            {
                JSONFileText = File.ReadAllText(path);
            }
            catch (FileNotFoundException)
            {
                throw new Exception("Error opening RunOutput file: " + path);
            }
            return JSONFileText;
        }

        public static void WriteJson(string jsonString, string systemFolderPath, string systemFolderPathExtension,string runID)
        {
            string path = systemFolderPath + "\\RunOutputFiles\\" + systemFolderPathExtension + "\\" + runID + ".json";
            File.WriteAllText(path, jsonString);
        }

        public static string GetAttributeVal(string runFileText, string workflowGroupName, string primaryKeyName, string primaryKeyValue, string returnValueKey)
        {
            string attributeReturn = "";
            JObject runOutputFileObject = JObject.Parse(runFileText);
            JObject match = runOutputFileObject[workflowGroupName].Values<JObject>()
                .Where(p => p[primaryKeyName].Value<string>() == primaryKeyValue)
                .Where(q => q["status"].Value<string>() == "active")
                .FirstOrDefault();
            if (match == null)
            {
                throw new Exception("Error Getting RunOutput Value: " + returnValueKey);
            }
            else
            {
                attributeReturn = match[returnValueKey].ToString();
                //foreach (JProperty prop in match.Properties())
                //{
                //    Console.WriteLine(prop.Name + ": " + prop.Value);
                //}
            }
            return attributeReturn;
        }

        public static string SetAttributeVal(string runFileText, string workflowGroupName, string primaryKeyName, string primaryKeyValue, string setValueKey, string setValueValue)
        {
            JObject runOutputFileObject = JObject.Parse(runFileText);
            JObject match = runOutputFileObject[workflowGroupName].Values<JObject>()
                .Where(p => p[primaryKeyName].Value<string>() == primaryKeyValue)
                .Where(q => q["status"].Value<string>() == "active")
                .FirstOrDefault();
            if (match == null)
            {
                throw new Exception("Error Setting RunOutput Value: " + setValueKey);
            }
            else
            {
                if (setValueValue == "null") match[setValueKey] = null;
                else match[setValueKey] = setValueValue;
            }

            string JSONOutput = runOutputFileObject.ToString();
            return JSONOutput;
        }

    }
}
