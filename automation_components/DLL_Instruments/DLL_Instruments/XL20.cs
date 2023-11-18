using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace DLL_Instruments
{
    public static class XL20
    {
        public static void AddRackToInventory(string systemFolderPath, string rackBarcode, string rackMetadata, string scanFilename)
        {
            System.Web.Script.Serialization.JavaScriptSerializer ser = new System.Web.Script.Serialization.JavaScriptSerializer();

            //Serialize inventry from inventory file
            string InvFilePath = systemFolderPath + "\\XL20Inventory.json";
            XL20_Inventory myInv = new XL20_Inventory();
            string InvFileText = File.ReadAllText(InvFilePath);
            myInv = ser.Deserialize<XL20_Inventory>(InvFileText);
            //Check if rack is in inventory
            if (Array.Exists(myInv.xl_20s[0].inventory.racks.ToArray(), element => element.barcode == rackBarcode))
            {
                throw new Exception("Rack aleardy present in Inventory");
            }

            //Add rack to inventory
            Rack newRack = new Rack();
            List<Tube> myTubes = new List<Tube>();
            newRack.barcode = rackBarcode;
            newRack.complete = false;
            newRack.tubes = myTubes;
            string[] myArray = rackMetadata.Split(',');
            newRack.position = Convert.ToInt32(myArray[0]);
            newRack.purpose = myArray[1];
            newRack.assay = myArray[2];

            if (scanFilename != "")
            {
                //Serialize scan file 
                string ScanFileText = File.ReadAllText(systemFolderPath + "\\SampleScanFiles\\" + scanFilename + ".json");
                TubeScan myScan = ser.Deserialize<TubeScan>(ScanFileText);

                foreach (ScannedTube oneTube in myScan.scanned_tubes)
                {
                    Tube newTube = new Tube();
                    newTube.barcode = oneTube.barcode;
                    newTube.row = oneTube.row;
                    newTube.column = oneTube.column;
                    myTubes.Add(newTube);
                }
                newRack.tubes = myTubes;
            }
            myInv.xl_20s[0].inventory.racks.Add(newRack);
            File.WriteAllText(InvFilePath, ser.Serialize(myInv));
        }

        public static string removeRackFromInventory(string systemFolderPath, int rackPosition)
        {
            string removedBarcode = "";
            System.Web.Script.Serialization.JavaScriptSerializer ser = new System.Web.Script.Serialization.JavaScriptSerializer();
            string InvFilePath = systemFolderPath + "\\XL20Inventory.json";
            XL20_Inventory myInv = ser.Deserialize<XL20_Inventory>(File.ReadAllText(InvFilePath));

            int i = 0;
            int myIndex = -1;
            foreach (Rack oneRack in myInv.xl_20s[0].inventory.racks)
            {
                if (oneRack.position == rackPosition)
                {
                    myIndex = i;
                    removedBarcode = oneRack.barcode;
                }
                i++;
            }
            if (myIndex != -1)
            {
                myInv.xl_20s[0].inventory.racks.RemoveAt(myIndex);
            }
            File.WriteAllText(InvFilePath, ser.Serialize(myInv));
            return removedBarcode;
        }

        public static int queryRackPosition(string systemFolderPath, string purpose, int maxTubeCount)
        {
            int position = -1;
            System.Web.Script.Serialization.JavaScriptSerializer ser = new System.Web.Script.Serialization.JavaScriptSerializer();
            string XL20FileText = File.ReadAllText(systemFolderPath + "\\XL20Inventory.json");
            XL20_Inventory myInfo_xl20 = ser.Deserialize<XL20_Inventory>(XL20FileText);

            foreach (Rack oneRack in myInfo_xl20.xl_20s[0].inventory.racks)
            {
                if (maxTubeCount == -1)
                {
                    if (oneRack.purpose == purpose && oneRack.complete)
                    {
                        position = oneRack.position;
                    }
                }
                else
                {
                    if (oneRack.purpose == purpose && oneRack.tubes.Count <= maxTubeCount)
                    {
                        position = oneRack.position;
                    }
                }
            }
            return position;
        }

        public static string queryRackFieldFromPosition(string systemFolderPath, int rackPosition, string field)
        {
            string fieldValue = "";
            System.Web.Script.Serialization.JavaScriptSerializer ser = new System.Web.Script.Serialization.JavaScriptSerializer();
            string InvFilePath = systemFolderPath + "\\XL20Inventory.json";
            XL20_Inventory myInv = ser.Deserialize<XL20_Inventory>(File.ReadAllText(InvFilePath));


            foreach (Rack oneRack in myInv.xl_20s[0].inventory.racks)
            {
                if (oneRack.position == rackPosition)
                {
                    switch (field)
                    {
                        case "assay":
                            fieldValue = oneRack.assay;
                            break;
                        case "barcode":
                            fieldValue = oneRack.barcode;
                            break;
                        case "complete":
                            fieldValue = oneRack.complete.ToString();
                            break;
                        case "purpose":
                            fieldValue = oneRack.purpose;
                            break;
                    }
                }
            }
            File.WriteAllText(InvFilePath, ser.Serialize(myInv));
            return fieldValue;
        }

        public static void ReplaceXL20InventoryAfterPick(string systemFolderPath)
        {
            string timeStamp = DateTime.Now.ToString("yyyyMMMd_h-mm-ss");
            string FileToMoveAndDelete = systemFolderPath + "\\XL20Inventory_temp.json";
            string FileToReplace = systemFolderPath + "\\XL20Inventory.json";
            string FileBackup = systemFolderPath + "\\XL20InventoryBackup\\XL20Inventory_" + timeStamp + ".json";
            File.Replace(FileToMoveAndDelete, FileToReplace, FileBackup, true);
        }

    }

    public class XL20_Inventory
    {
        public string automation_system { get; set; }
        public List<Device_XL20> xl_20s { get; set; }
    }

    public class Device_XL20
    {
        public bool enabled { get; set; }
        public OperatingMode operating_mode { get; set; }
        public TubeInventory inventory { get; set; }
        public List<OneMove> tube_moves { get; set; }
    }

    public class OperatingMode
    {
        public List<string> batch { get; set; }
        public bool collect { get; set; }
        public bool sweep { get; set; }
        public bool allow_fillers { get; set; }
    }


public class TubeInventory
    {
        public string location_name { get; set; }
        public List<Rack> racks { get; set; }
    }

    public class Rack
    {
        public string barcode { get; set; }
        public int position { get; set; }
        public string purpose { get; set; }
        public string assay { get; set; }
        public bool complete { get; set; }
        public List<Tube> tubes { get; set; }
    }

    public class Tube
    {
        public string barcode { get; set; }
        public int row { get; set; }
        public int column { get; set; }
    }

    //worklist
    public class OneMove
    {
        public string tube_barcode { get; set; }
        public Motion source { get; set; }
        public Motion destination { get; set; }
    }

    public class Motion
    {
        public string rack_barcode { get; set; }
        public string rack_position { get; set; }
        public string tube_row { get; set; }
        public string tube_column { get; set; }
    }

}
