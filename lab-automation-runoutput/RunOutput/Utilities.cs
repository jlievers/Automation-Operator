using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Com.Invitae.LabAutomation
{
    public static class Utilities
    {
        public static string ReadAllText(string runFilePath)
        {
            string rawTextString = "";
            int retryCounter = 10;
            bool fileOperationsuccess = false;
            while (retryCounter > 0 && !fileOperationsuccess)
            {
                try
                {
                    rawTextString = File.ReadAllText(runFilePath);
                    fileOperationsuccess = true;
                }
                catch (IOException)
                {
                    retryCounter--;
                    Thread.Sleep(1000);
                }
            }
            if (!fileOperationsuccess)
            {
                throw new Exception("Read operation with run output file failed.");
            }
            return rawTextString;
        }

        public static string OpenRunOutputFile(string systemFolderPath,string systemFolderPathExtension, string runID)  //open json file and return text
        {
            string path = systemFolderPath + "\\RunOutputFiles\\" + systemFolderPathExtension + "\\" + runID + ".json";
            if (!File.Exists(path))
            {
                throw new FileNotFoundException("Error opening RunOutput file: " + path);
            }
            return ReadAllText(path);
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

        public static string GetAttributeVal(bool usingRunFilePath, string runFilePath, string workflowGroupName, string primaryKeyName, string primaryKeyValue, string returnValueKey)
        {
            Dictionary<string, object> outDictionary = ReadDatafromRunOutputFile("GetAttributeVal", runFilePath: runFilePath, workflowGroupName: workflowGroupName, primaryKeyName: primaryKeyName, primaryKeyValue: primaryKeyValue, returnValueKey: returnValueKey);
            return outDictionary["attribute_return"].ToString();
        }

        public static string SetAttributeVal(string runFileText, string workflowGroupName, string primaryKeyName, string primaryKeyValue, string setValueKey, string setValueValue)
        {
            JObject runOutputFileObject = JObject.Parse(runFileText);
            JObject match = runOutputFileObject[workflowGroupName].Values<JObject>()
                .Where(p => p[primaryKeyName].Value<string>() == primaryKeyValue)
                .Where(q => q["status"].Value<string>() == "active")
                .FirstOrDefault();
            if (match == null || !match.ContainsKey(setValueKey))
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

        public static void SetAttributeVal(bool usingRunFilePath, string runFilePath, string workflowGroupName, string primaryKeyName, string primaryKeyValue, string setValueKey, string setValueValue)
        {
            ReadAndWriteRunOutputFile("SetAttributeVal", runFilePath: runFilePath, workflowGroupName: workflowGroupName, primaryKeyName: primaryKeyName, primaryKeyValue: primaryKeyValue, setValueKey: setValueKey, setValueValue: setValueValue);         
        }

        public static void SetWorkfloworPlateObject(string runFilePath, string workflowGroupName, string primaryKeyName, string primaryKeyValue, object workfloworPlatebOject)
        {
            ReadAndWriteRunOutputFile("SetWorkfloworPlateObject", runFilePath: runFilePath, workflowGroupName: workflowGroupName, primaryKeyName: primaryKeyName, primaryKeyValue: primaryKeyValue, workfloworPlateObject: workfloworPlatebOject);
        }

        ////private functions
        ///
        private static Dictionary<string, object> ReadDatafromRunOutputFile(string operation, string runFilePath = "", string workflowGroupName = "", string primaryKeyName = "", string primaryKeyValue = "", string returnValueKey = "")
        {
            Dictionary<string, object> outDictionary = new Dictionary<string, object>();
            JObject runOutputFileObject = JObject.Parse(ReadAllText(runFilePath));
            switch (operation)
            {
                case "GetAttributeVal":
                    outDictionary = GetAttributeValOperation(runOutputFileObject, workflowGroupName, primaryKeyName, primaryKeyValue, returnValueKey);
                    break;
            }
            return outDictionary;
        }

        private static Dictionary<string, object> ReadAndWriteRunOutputFile(string operation, string runFilePath = "", string workflowGroupName = "", string primaryKeyName = "", string primaryKeyValue = "", string setValueKey = "", string setValueValue = "", object workfloworPlateObject = null)
        {           
            Dictionary<string, object> outDictionary = new Dictionary<string, object>();
            int retryCounter = 10;
            bool fileOperationsuccess = false;
            while (retryCounter > 0 && !fileOperationsuccess)
            {
                try
                {
                    using (FileStream fs = File.Open(runFilePath, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
                    {
                        using (StreamReader sr = new StreamReader(fs))
                        {
                            string runFileText = sr.ReadToEnd();
                            JObject runOutputFileObject = JObject.Parse(runFileText);
                            switch (operation)
                            {
                                case "SetAttributeVal":
                                    SetAttributeValOperation(fs, runOutputFileObject, workflowGroupName, primaryKeyName, primaryKeyValue, setValueKey, setValueValue);
                                    break;
                                case "SetWorkfloworPlateObject":
                                    SetWorkfloworPlateObjectOperation(fs, runOutputFileObject, workflowGroupName, primaryKeyName, primaryKeyValue, workfloworPlateObject);
                                    break;
                            }
                        }
                    }
                    fileOperationsuccess = true;
                }
                catch (IOException)
                {
                    retryCounter--;
                    Thread.Sleep(1000);
                }
            }
            if (!fileOperationsuccess)
            {
                throw new Exception("Read and write operation with run output file failed.");
            }
            return outDictionary;
        }

        private static Dictionary<string, object> GetAttributeValOperation(JObject runOutputFileObject, string workflowGroupName, string primaryKeyName, string primaryKeyValue, string returnValueKey)
        {
            Dictionary<string, object> outDictionary = new Dictionary<string, object>
            {
                ["attribute_return"] = ""
            };
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
                outDictionary["attribute_return"] = match[returnValueKey].ToString();
            }
            return outDictionary;
        }

        private static void SetAttributeValOperation(FileStream fs, JObject runOutputFileObject, string workflowGroupName, string primaryKeyName, string primaryKeyValue, string setValueKey, string setValueValue)
        {
            JObject backupCopy = runOutputFileObject;
            JObject match = runOutputFileObject[workflowGroupName].Values<JObject>()
                .Where(p => p[primaryKeyName].Value<string>() == primaryKeyValue)
                .Where(q => q["status"].Value<string>() == "active")
                .FirstOrDefault();
            if (match == null || !match.ContainsKey(setValueKey))
            {
                throw new Exception("Error Setting RunOutput Value: " + setValueKey);
            }
            else
            {
                if (setValueValue == "null") match[setValueKey] = null;
                else match[setValueKey] = setValueValue;
            }
            WriteToFileStream(fs, runOutputFileObject, backupCopy);
        }

        private static void SetWorkfloworPlateObjectOperation(FileStream fs, JObject runOutputFileObject, string workflowGroupName, string primaryKeyName, string primaryKeyValue, object workfloworPlateObject)
        {
            JObject backupCopy = runOutputFileObject;
            JObject inputObject = JObject.Parse(JsonConvert.SerializeObject(workfloworPlateObject));
            var list = runOutputFileObject[workflowGroupName].Values<JObject>().ToList();
            JObject match = runOutputFileObject[workflowGroupName].Values<JObject>()
                .Where(p => p[primaryKeyName].Value<string>() == primaryKeyValue)
                .Where(q => q["status"].Value<string>() == "active")
                .FirstOrDefault();
            int index = list.IndexOf(match);          
            if (match == null)
            {
                throw new Exception("Error Setting RunOutput Value: " + inputObject.ToString());
            }
            else
            {
                runOutputFileObject[workflowGroupName][index] = inputObject;
            }
            WriteToFileStream(fs, runOutputFileObject, backupCopy);
        }

        private static void WriteToFileStream(FileStream fs, JObject runOutputFileObject, JObject backupCopy)
        {
            try
            {
                fs.Position = 0;
                fs.SetLength(0);
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    sw.Write(runOutputFileObject.ToString());
                }
            }
            catch (Exception)
            {
                fs.Position = 0;
                fs.SetLength(0);
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    sw.Write(backupCopy.ToString());
                }
                throw new Exception("Write operation with run output file failed.");
            }
        }
    }
}
