using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Reflection;

namespace Com.Invitae.LabAutomation
{
    public static class Inventory
    {
        public static string ReadAllText(string invFilePath)
        {
            string rawTextString = "";
            int retryCounter = 10;
            bool fileOperationSuccess = false;
            while (retryCounter > 0 && !fileOperationSuccess)
            {
                try
                {
                    rawTextString = File.ReadAllText(invFilePath);
                    fileOperationSuccess = true;
                }
                catch (IOException)
                {
                    retryCounter--;
                    Thread.Sleep(1000);
                }
            }
            if (!fileOperationSuccess)
            {
                throw new Exception("Read operation with inventory file failed.");
            }
            return rawTextString;
        }

        public static int GetAvailableNest(string invFilePath, string deviceName) //Get open nest with lowest nest_num
        {          
            Dictionary<string, object> outDictionary = ReadDatafromInventoryFile("GetAvailableNest", invFilePath: invFilePath, deviceName: deviceName);
            bool found = (bool)outDictionary["found"];
            if (!found)
            {
                //throw new Exception(deviceName + " Is Full!");
            }
            return (int)outDictionary["nest_number"];
        }

        public static void AddLabwareToNest(string invFilePath, string deviceName, int nest_num, string barcode, string workflowId)
        {
            ReadAndWriteInventoryFile("AddLabwareToNest", invFilePath: invFilePath, deviceName: deviceName, nest_num: nest_num, barcode: barcode, workflow_id: workflowId);
        }

        public static int GetNextNestPosition(string invFilePath, string deviceName, out string barcodeOut) //Get occupied nest_num with highest priority
        {
            Dictionary<string, object> outDictionary = ReadDatafromInventoryFile("GetNextNestPosition", invFilePath: invFilePath, deviceName: deviceName);
            int nextNestPosition = (int)outDictionary["nest_number"];
            barcodeOut = outDictionary["barcode_out"].ToString();                   
            if (nextNestPosition == -1)
            {
                throw new Exception("No Available Nests");
            }
            return nextNestPosition;
        }

        public static string RemoveLabwareFromNest(string invFilePath, string deviceName, int nest_num)
        {
            Dictionary<string, object> outDictionary = ReadAndWriteInventoryFile("RemoveLabwareFromNest", invFilePath: invFilePath, deviceName: deviceName, nest_num: nest_num);        
            return outDictionary["barcode_out"].ToString();
        }

        public static string GetNestAttribute(string attributeName,string invFilePath, string deviceName, int nest_num)
        {
            Dictionary<string, object> outDictionary = ReadDatafromInventoryFile("GetNestAttribute", attributeName: attributeName, invFilePath: invFilePath, deviceName: deviceName, nest_num: nest_num);
            return outDictionary["attribute_out"].ToString();
        }

        public static void SetNestAttribute(string attributeName, string attributeValue, string invFilePath, string deviceName, int nest_num)
        {
            ReadAndWriteInventoryFile("SetNestAttribute", attributeName: attributeName, attributeValue: attributeValue, invFilePath: invFilePath, deviceName: deviceName, nest_num: nest_num);            
        }

        public static string[] GetNestNumAndDeviceFromBarcode(string barcode, string invFilePath)
        {
            Dictionary<string, object> outDictionary = ReadDatafromInventoryFile("GetNestNumAndDeviceFromBarcode", barcode: barcode, invFilePath: invFilePath);
            string[] myArray = new string[2] { outDictionary["nest_number"].ToString(), outDictionary["device_name"].ToString() };
            if (myArray[0] == "")
            {
                throw new Exception(barcode + " not found on system");
            }
            return myArray;
        }

        public static void SetDeviceObject(string invFilePath, Device devicetoSet, string deviceName, Device devicetoReference, out bool error, out Device outDevice)
        {
            Dictionary<string, object> outDictionary = ReadAndWriteInventoryFile("SetDeviceObject", invFilePath: invFilePath, devicetoSet: devicetoSet, deviceName: deviceName, devicetoReference: devicetoReference);
            error = (bool)outDictionary["error"];
            outDevice = outDictionary["out_device"] as Device;
        }

        ////private functions/////////////////////////////////////////////
        ///
        private static Dictionary<string, object> ReadDatafromInventoryFile(string operation, string invFilePath = "", string deviceName = "", int nest_num = 0, string barcode = "", string attributeName = "")
        {
            System.Web.Script.Serialization.JavaScriptSerializer ser = new System.Web.Script.Serialization.JavaScriptSerializer();
            Dictionary<string, object> outDictionary = new Dictionary<string, object>();
            SystemInventory myInv = ser.Deserialize<SystemInventory>(ReadAllText(invFilePath));
            switch (operation)
            {
                case "GetAvailableNest":
                    outDictionary = GetAvailableNestOperation(myInv, deviceName);
                    break;               
                case "GetNextNestPosition":
                    outDictionary = GetNextNestPositionOperation(myInv, deviceName);
                    break;
                case "GetNestAttribute":
                    outDictionary = GetNestAttributeOperation(myInv, attributeName, deviceName, nest_num);
                    break;
                case "GetNestNumAndDeviceFromBarcode":
                    outDictionary = GetNestNumAndDeviceFromBarcodeOperation(myInv, barcode);
                    break;
            }
            return outDictionary;
        }


        private static Dictionary<string, object> ReadAndWriteInventoryFile(string operation, string invFilePath = "", string deviceName = "", int nest_num = 0, string barcode = "", string workflow_id = "", string attributeName = "", string attributeValue = "", Device devicetoSet = null, Device devicetoReference = null)
        {
            System.Web.Script.Serialization.JavaScriptSerializer ser = new System.Web.Script.Serialization.JavaScriptSerializer();           
            Dictionary<string, object> outDictionary = new Dictionary<string, object>();
            int retryCounter = 10;
            bool fileOperationsuccess = false;
            while (retryCounter > 0 && !fileOperationsuccess)
            {
                try
                {
                    using (FileStream fs = File.Open(invFilePath, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
                    {
                        using (StreamReader sr = new StreamReader(fs))
                        {
                            string text = sr.ReadToEnd();
                            SystemInventory myInv = ser.Deserialize<SystemInventory>(text);
                            switch (operation)
                            {
                                case "AddLabwareToNest":
                                    AddLabwareToNestOperation(fs, myInv, deviceName, nest_num, barcode, workflow_id);
                                    break;
                                case "RemoveLabwareFromNest":
                                    outDictionary = RemoveLabwareFromNestOperation(fs, myInv, deviceName, nest_num);
                                    break;
                                case "SetNestAttribute":
                                    SetNestAttributeOperation(fs, myInv, attributeName, attributeValue, deviceName, nest_num);
                                    break;
                                case "SetDeviceObject":
                                    outDictionary = SetDeviceObjectOperation(fs, myInv, devicetoSet, deviceName, devicetoReference);
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
                throw new Exception("Read and write operation with inventory file failed.");
            }
            return outDictionary;
        }

        private static Dictionary<string, object> SetDeviceObjectOperation(FileStream fs, SystemInventory myInv, Device devicetoSet, string deviceName, Device devicetoReference)
        {
            SystemInventory backupCopy = myInv;
            Dictionary<string, object> outDictionary = new Dictionary<string, object>()
            {
                ["error"] = false,
                ["out_device"] = null
            };
            Device currentDevice = new Device();
            int counter = 0;
            int index = -1;
            bool deviceFound = false;
            foreach (Device myDevice in myInv.devices)
            {
                if (myDevice.device_name == deviceName)
                {
                    deviceFound = true;
                    currentDevice = myDevice;
                    index = counter;
                }
                counter++;
            }
            if (!deviceFound)
            {
                throw new ArgumentException("Specified device " + deviceName + " is not found.");
            }
            for (int i = 0; i <= (currentDevice.num_nests - 1); i++)
            {
                if (currentDevice.nests[i].plate_barcode != devicetoReference.nests[i].plate_barcode)
                {
                    outDictionary["error"] = true;
                    outDictionary["out_device"] = currentDevice;
                    return outDictionary;
                }
            }
            myInv.devices[index] = devicetoSet;
            WriteToFileStream(fs, myInv, backupCopy);
            return outDictionary;
        }

        private static Dictionary<string, object> GetNestNumAndDeviceFromBarcodeOperation(SystemInventory myInv, string barcode)
        {
            Dictionary<string, object> outDictionary = new Dictionary<string, object>()
            {
                ["nest_number"] = "",
                ["device_name"] = ""
            };
            foreach (Device myDevice in myInv.devices)
            {
                foreach (Nest myNest in myDevice.nests)
                {
                    if (myNest.plate_barcode == barcode)
                    {
                        outDictionary["nest_number"] = myNest.nest_number.ToString();
                        outDictionary["device_name"] = myDevice.device_name;
                    }
                }
            }
            return outDictionary;
        }

        private static void SetNestAttributeOperation(FileStream fs, SystemInventory myInv, string attributeName, string attributeValue, string deviceName, int nest_num)
        {
            SystemInventory backupCopy = myInv;
            bool deviceFound = false;
            bool nestFound = false;
            foreach (Device myDevice in myInv.devices)
            {
                if (myDevice.device_name == deviceName)
                {
                    deviceFound = true;
                    foreach (Nest myNest in myDevice.nests)
                    {
                        if (myNest.nest_number == nest_num)
                        {
                            nestFound = true;
                            myNest.GetType().GetProperty(attributeName).SetValue(myNest, attributeValue);
                        }
                    }
                    DistributePriorities(myDevice.nests);
                }
            }
            if (!deviceFound)
            {
                throw new ArgumentException("Specified device " + deviceName + " is not found.");
            }
            else if (!nestFound)
            {
                throw new ArgumentException("Specified nest " + nest_num.ToString() + " is not found.");
            }
            WriteToFileStream(fs, myInv, backupCopy);
        }

        private static Dictionary<string, object> GetNestAttributeOperation(SystemInventory myInv, string attributeName, string deviceName, int nest_num)
        {
            Dictionary<string, object> outDictionary = new Dictionary<string, object>()
            {
                ["attribute_out"] = ""
            };
            bool deviceFound = false;
            bool nestFound = false;
            foreach (Device myDevice in myInv.devices)
            {
                if (myDevice.device_name == deviceName)
                {
                    deviceFound = true;
                    foreach (Nest myNest in myDevice.nests)
                    {
                        if (myNest.nest_number == nest_num)
                        {
                            nestFound = true;
                            if (myNest.GetType().GetProperty(attributeName).GetValue(myNest) != null)
                            {
                                outDictionary["attribute_out"] = myNest.GetType().GetProperty(attributeName).GetValue(myNest).ToString();
                            }
                            else
                            {
                                outDictionary["attribute_out"] = "";
                            }
                        }
                    }                   
                }
            }
            if (!deviceFound)
            {
                throw new ArgumentException("Specified device " + deviceName + " is not found.");
            }
            else if (!nestFound)
            {
                throw new ArgumentException("Specified nest " + nest_num.ToString() + " is not found.");
            }
            return outDictionary;
        }

        private static Dictionary<string, object> RemoveLabwareFromNestOperation(FileStream fs, SystemInventory myInv, string deviceName, int nest_num)
        {
            SystemInventory backupCopy = myInv;
            Dictionary<string, object> outDictionary = new Dictionary<string, object>()
            {
                ["barcode_out"] = ""
            };
            bool deviceFound = false;
            bool nestFound = false;
            foreach (Device myDevice in myInv.devices)
            {
                if (myDevice.device_name == deviceName)
                {
                    deviceFound = true;
                    foreach (Nest myNest in myDevice.nests)
                    {
                        if (myNest.nest_number == nest_num)
                        {
                            nestFound = true;
                            outDictionary["barcode_out"] = myNest.plate_barcode;
                            PropertyInfo[] properties = myNest.GetType().GetProperties();
                            foreach (PropertyInfo oneProp in properties)
                            {
                                if (oneProp.Name != "nest_number")
                                {
                                    oneProp.SetValue(myNest, null);
                                }
                            }
                        }
                    }
                    DistributePriorities(myDevice.nests);
                }
            }
            if (!deviceFound)
            {
                throw new ArgumentException("Specified device " + deviceName + " is not found.");
            }
            else if (!nestFound)
            {
                throw new ArgumentException("Specified nest " + nest_num.ToString() + " is not found.");
            }
            WriteToFileStream(fs, myInv, backupCopy);
            return outDictionary;
        }

        private static Dictionary<string, object> GetNextNestPositionOperation(SystemInventory myInv, string deviceName)
        {
            Dictionary<string, object> outDictionary = new Dictionary<string, object>()
            {
                ["barcode_out"] = "",
                ["nest_number"] = -1
            };
            foreach (Device myDevice in myInv.devices)
            {
                if (myDevice.device_name == deviceName)
                {
                    foreach (Nest myNest in myDevice.nests)
                    {
                        if (Convert.ToInt16(myNest.priority) == 1)
                        {
                            outDictionary["barcode_out"] = myNest.plate_barcode;
                            outDictionary["nest_number"] = myNest.nest_number;
                        }
                    }
                }
            }
            return outDictionary;
        }

        private static void AddLabwareToNestOperation(FileStream fs, SystemInventory myInv, string deviceName, int nest_num, string barcode, string workflow_id)
        {
            SystemInventory backupcopy = myInv;
            bool deviceFound = false;
            bool nestFound = false;
            foreach (Device myDevice in myInv.devices)
            {
                if (myDevice.device_name == deviceName)
                {
                    deviceFound = true;
                    foreach (Nest myNest in myDevice.nests)
                    {
                        if (myNest.nest_number == nest_num)
                        {
                            nestFound = true;
                            if (barcode != "")
                            {
                                myNest.plate_barcode = barcode;
                            }
                            if (workflow_id != "")
                            {
                                myNest.workflow_id = workflow_id;
                            }
                            myNest.priority = 9999.ToString();
                        }
                    }
                    DistributePriorities(myDevice.nests);
                }
            }
            if (!deviceFound)
            {
                throw new ArgumentException("Specified device " + deviceName + " is not found.");
            }
            else if (!nestFound)
            {
                throw new ArgumentException("Specified nest " + nest_num.ToString() + " is not found.");
            }
            WriteToFileStream(fs, myInv, backupcopy);
        }

        private static Dictionary<string, object> GetAvailableNestOperation(SystemInventory myInv, string deviceName)
        {
            Dictionary<string, object> outDictionary = new Dictionary<string, object>
            {
                ["found"] = false,
                ["nest_number"] = -1
            };
            foreach (Device myDevice in myInv.devices)
            {
                if (myDevice.device_name == deviceName)
                {
                    foreach (Nest myNest in myDevice.nests)
                    {
                        if (!(bool)outDictionary["found"] && myNest.plate_barcode == null)
                        {
                            outDictionary["nest_number"] = myNest.nest_number;                            
                            outDictionary["found"] = true;
                        }
                    }
                }
            }
            return outDictionary;
        }

        private static void WriteToFileStream(FileStream fs, SystemInventory myInv, SystemInventory backupcopy)
        {
            System.Web.Script.Serialization.JavaScriptSerializer ser = new System.Web.Script.Serialization.JavaScriptSerializer();
            try
            {
                fs.Position = 0;
                fs.SetLength(0);
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    sw.Write(ser.Serialize(myInv));
                }
            }
            catch (Exception)
            {
                fs.Position = 0;
                fs.SetLength(0);
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    sw.Write(ser.Serialize(backupcopy));
                }
                throw new Exception("Write operation with inventory file failed.");
            }
        }

        public static List<Nest> DistributePriorities(List<Nest> myNests)
        {
            SortedDictionary<int, string> myDict = new SortedDictionary<int, string>();
            foreach (Nest _myNest in myNests)
            {
                if (_myNest.plate_barcode != null)
                {
                    myDict.Add(Convert.ToInt16(_myNest.priority), _myNest.plate_barcode);
                }
            }
            int i = 1;
            foreach (KeyValuePair<int, string> pair in myDict)
            {
                foreach (Nest _myNest in myNests)
                {
                    if (_myNest.plate_barcode == pair.Value)
                    {
                        _myNest.priority = i.ToString();
                    }
                }
                i++;
            }
            return myNests;
        }
    }

    public class SystemInventory
    {
        public string system_name { get; set; }
        public string system_lm { get; set; }
        public List<Device> devices { get; set; }
    }

    public class Device
    {
        public string device_name { get; set; }
        public string device_type { get; set; }
        public string lm_number { get; set; }
        public bool enabled { get; set; }
        public int num_nests { get; set; }
        public List<Stack> stacks { get; set; }
        public List<Nest> nests { get; set; }
    }

    public class Stack
    {
        public string stack_num { get; set; }
        public List<Nest> nests { get; set; }
    }

    public class Nest
    {
        public int nest_number { get; set; }
        public string workflow_id { get; set; }
        public string plate_barcode { get; set; }
        public string priority { get; set; }
        public string assay_id { get; set; }
        public string status { get; set; }
    }   
}

