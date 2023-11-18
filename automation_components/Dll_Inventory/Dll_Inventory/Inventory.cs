using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Web.Script.Serialization;
using System.Windows.Forms;
using System.Data;
using System.Threading;
using System.Reflection;

namespace Dll_Inventory
{
    public static class Inventory
    {
        public static int GetAvailableNest(string invFilePath, string deviceName) //Get open nest with lowest nest_num
        {
            System.Web.Script.Serialization.JavaScriptSerializer ser = new System.Web.Script.Serialization.JavaScriptSerializer();
            string JSONFileText = readInventoryFile(invFilePath);
            SystemInventory myInv = ser.Deserialize<SystemInventory>(JSONFileText);
            int openNestNum = -1;
            bool found = false;
            foreach (Device myDevice in myInv.devices)
            {
                if (myDevice.device_name == deviceName)
                {
                    foreach (Nest myNest in myDevice.nests)
                    {
                        if (!found && myNest.plate_barcode == null)
                        {
                            openNestNum = myNest.nest_number;
                            found = true;
                        }
                    }
                }
            }
            if (!found)
            {
                //throw new Exception(deviceName + " Is Full!");
            }
            return openNestNum;
        }

        public static void AddLabwareToNest(string invFilePath, string deviceName, int nest_num, string barcode, string workflow_id)
        {
            System.Web.Script.Serialization.JavaScriptSerializer ser = new System.Web.Script.Serialization.JavaScriptSerializer();
            string JSONFileText = readInventoryFile(invFilePath);
            SystemInventory myInv = ser.Deserialize<SystemInventory>(JSONFileText);
            foreach (Device myDevice in myInv.devices)
            {
                if (myDevice.device_name == deviceName)
                {
                    foreach (Nest myNest in myDevice.nests)
                    {
                        if (myNest.nest_number == nest_num)
                        {
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
            File.WriteAllText(invFilePath, ser.Serialize(myInv));
        }

        public static int GetNextNestPosition(string invFilePath, string deviceName, out string barcodeOut) //Get occupied nest_num with highest priority
        {
            System.Web.Script.Serialization.JavaScriptSerializer ser = new System.Web.Script.Serialization.JavaScriptSerializer();
            string JSONFileText = readInventoryFile(invFilePath);
            SystemInventory myInv = ser.Deserialize<SystemInventory>(JSONFileText);
            int nestNumber = -1;
            barcodeOut = "";
            foreach (Device myDevice in myInv.devices)
            {
                if (myDevice.device_name == deviceName)
                {
                    foreach (Nest myNest in myDevice.nests)
                    {
                        if (Convert.ToInt16(myNest.priority) == 1)
                        {
                            barcodeOut = myNest.plate_barcode;
                            nestNumber = myNest.nest_number;
                        }
                    }
                }
            }
            if (nestNumber == -1)
            {
                throw new Exception("No Available Nests");
            }
            return nestNumber;
        }

        public static string RemoveLabwareFromNest(string invFilePath, string deviceName, int nest_num)
        {
            System.Web.Script.Serialization.JavaScriptSerializer ser = new System.Web.Script.Serialization.JavaScriptSerializer();
            string JSONFileText = readInventoryFile(invFilePath);
            SystemInventory myInv = ser.Deserialize<SystemInventory>(JSONFileText);
            string barcodeOut = "";
            foreach (Device myDevice in myInv.devices)
            {
                if (myDevice.device_name == deviceName)
                {
                    foreach (Nest myNest in myDevice.nests)
                    {
                        if (myNest.nest_number == nest_num)
                        {
                            barcodeOut = myNest.plate_barcode;
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
            File.WriteAllText(invFilePath, ser.Serialize(myInv));
            return barcodeOut;
        }

        public static string GetNestAttribute(string attributeName,string invFilePath, string deviceName, int nest_num)
        {
            System.Web.Script.Serialization.JavaScriptSerializer ser = new System.Web.Script.Serialization.JavaScriptSerializer();
            string JSONFileText = readInventoryFile(invFilePath);
            SystemInventory myInv = ser.Deserialize<SystemInventory>(JSONFileText);
            string attributeOut = "";
            foreach (Device myDevice in myInv.devices)
            {
                if (myDevice.device_name == deviceName)
                {
                    foreach (Nest myNest in myDevice.nests)
                    {
                        if (myNest.nest_number == nest_num)
                        {
                            if (myNest.GetType().GetProperty(attributeName).GetValue(myNest) != null)
                            {
                                attributeOut = myNest.GetType().GetProperty(attributeName).GetValue(myNest).ToString();
                            }
                            else
                            {
                                attributeOut = "";
                            }
                        }
                    }
                   // DistributePriorities(myDevice.nests);
                }
            }
           // File.WriteAllText(invFilePath, ser.Serialize(myInv));
            return attributeOut;
        }

        public static void SetNestAttribute(string attributeName, string attributeValue, string invFilePath, string deviceName, int nest_num)
        {
            System.Web.Script.Serialization.JavaScriptSerializer ser = new System.Web.Script.Serialization.JavaScriptSerializer();
            string JSONFileText = readInventoryFile(invFilePath);
            SystemInventory myInv = ser.Deserialize<SystemInventory>(JSONFileText);
            foreach (Device myDevice in myInv.devices)
            {
                if (myDevice.device_name == deviceName)
                {
                    foreach (Nest myNest in myDevice.nests)
                    {
                        if (myNest.nest_number == nest_num)
                        {

                            myNest.GetType().GetProperty(attributeName).SetValue(myNest, attributeValue);
                        }
                    }
                    DistributePriorities(myDevice.nests);
                }
            }
            File.WriteAllText(invFilePath, ser.Serialize(myInv));
        }

        public static string[] GetNestNumAndDeviceFromBarcode(string barcode, string invFilePath)
        {
            System.Web.Script.Serialization.JavaScriptSerializer ser = new System.Web.Script.Serialization.JavaScriptSerializer();
            string JSONFileText = readInventoryFile(invFilePath);
            SystemInventory myInv = ser.Deserialize<SystemInventory>(JSONFileText);
            string[] myArray = new string[2];
            foreach (Device myDevice in myInv.devices)
            {
                foreach (Nest myNest in myDevice.nests)
                {
                    if (myNest.plate_barcode == barcode)
                    {
                        myArray[0] = myNest.nest_number.ToString();
                        myArray[1] = myDevice.device_name;
                        
                    }
                }
            }
            if (myArray[0] == null)
            {
                throw new Exception(barcode + " not found on system");
            }
            return myArray;
        }

        ////private functions/////////////////////////////////////////////
        ///
        private static string readInventoryFile(string InvFileLoc)
        {
            int i = 0;
            while (!IsFileReady(InvFileLoc) && i < 10)
            {
                i++;
                Thread.Sleep(1000);
            }
            string FileText = File.ReadAllText(InvFileLoc);
            return FileText;
        }

        private static bool IsFileReady(string sFilename)
        {
            try
            {
                using (FileStream inputStream = File.Open(sFilename, FileMode.Open, FileAccess.Read, FileShare.None))
                    return inputStream.Length > 0;
            }
            catch (Exception)
            {
                return false;
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

