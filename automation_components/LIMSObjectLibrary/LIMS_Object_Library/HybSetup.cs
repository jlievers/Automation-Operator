using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace LIMSObjectLibrary
{
    public static class HybSetup
    {
        public static List<string>[] getHybSetupInventory(string[] incubatorInventoryArray, string baitTubeFilePath_In, int numPlates)
        {
            try
            {
                List<string>[] _myInventory = new List<string>[4];
                List<string> myTubes = new List<string>();
                using (StreamReader sr = File.OpenText(baitTubeFilePath_In))
                {
                    string s = "";
                    while ((s = sr.ReadLine()) != null)
                    {
                        if (s.Split(',')[1] != " NO READ")
                        {
                            myTubes.Add(s.Split(',')[1].Trim());
                        }
                    }
                }
                _myInventory[0] = myTubes;
                _myInventory[1] = new List<string>();
                _myInventory[2] = new List<string>();
                _myInventory[3] = new List<string>();
                ///////////////////////////////////////////////////
                String[] myBarcodesArray_HybPlates = incubatorInventoryArray.Skip(6).Take(numPlates).ToArray();
                foreach (string index in myBarcodesArray_HybPlates)
                {
                    _myInventory[1].Add(index);
                }
                String[] myBarcodesArray_Formamide = incubatorInventoryArray.Take(3).ToArray();
                _myInventory[2].Add(myBarcodesArray_Formamide[0]);
                if (myBarcodesArray_Formamide[1] != "No Plate" || myBarcodesArray_Formamide[2] != "No Plate")
                {
                    _myInventory[2].Add(myBarcodesArray_Formamide[1]);
                }
                if (myBarcodesArray_Formamide[2] != "No Plate")
                {
                    _myInventory[2].Add(myBarcodesArray_Formamide[2]);
                }
                String[] myBarcodesArray_HybBuffer = incubatorInventoryArray.Skip(3).Take(3).ToArray();
                _myInventory[3].Add(myBarcodesArray_HybBuffer[0]);
                if (myBarcodesArray_HybBuffer[1] != "No Plate" || myBarcodesArray_HybBuffer[2] != "No Plate")
                {
                    _myInventory[3].Add(myBarcodesArray_HybBuffer[1]);
                }
                if (myBarcodesArray_HybBuffer[2] != "No Plate")
                {
                    _myInventory[3].Add(myBarcodesArray_HybBuffer[2]);
                }
                return _myInventory;
            }
            catch (Exception ex)
            {
                throw new Exception("getHybSetupInventory method: " + ex.Message);
            }
        }//RunInfo

        public static HybSetup_RunInfo getOneHybSetupPlateGroup(HybSetup_RunInfo myInfo_In, int index_In)
        {
            try
            {
                int index = index_In - 1;
                HybSetup_Plates oneHybPlate = myInfo_In.hyb_plates[index];
                HybSetup_RunInfo onePlateInfo = new HybSetup_RunInfo();
                onePlateInfo.automation_system = myInfo_In.automation_system;
                onePlateInfo.module = myInfo_In.module;
                onePlateInfo.hyb_plates = new List<HybSetup_Plates>();
                onePlateInfo.pooled_ipcr_plates = myInfo_In.pooled_ipcr_plates;
                onePlateInfo.bait_tube_scanfile_name = myInfo_In.bait_tube_scanfile_name;
                onePlateInfo.hyb_plate_column_qty = myInfo_In.hyb_plate_column_qty;
                onePlateInfo.hyb_plates.Add(oneHybPlate);
                return onePlateInfo;
            }
            catch (Exception ex)
            {
                throw new Exception("getOneHybSetupPlateGroup method: " + ex.Message);
            }
        }//RunInfo
    }

    public class HybSetup_RunInfo
    {
        public string automation_system { get; set; }
        public string module { get; set; }
        public string bait_tube_scanfile_name { get; set; }//For Me only!!
        public int hyb_plate_column_qty { get; set; }//Only on Init
        public List<string> pooled_ipcr_plates { get; set; }//For Me only!!
        public List<HybSetup_Plates> hyb_plates { get; set; }
    }//RunInfo

    public class HybSetup_Plates
    {
        public string workflow { get; set; }
        public string hyb_plate { get; set; }//From BC Read
        public string formamide { get; set; }
        public string buffer_5 { get; set; }
        public List<HybSetup_ColumnData> column_data { get; set; }
    }//RunInfo

    public class HybSetup_ColumnData
    {
        public string pooled_ipcr_plate { get; set; }
        public int source_plate_column_qty { get; set; }
        public int source_column_number { get; set; }
        public int target_column_number { get; set; }
        public string bait_tube { get; set; }
    }//RunInfo

    public class HybSetup_InventoryMessage
    {
        public string module { get; set; }
        public string automation_system { get; set; }
        public int hyb_plate_column_qty { get; set; }
        public List<string> pooled_ipcr_plates { get; set; }
        public List<string> bait_tubes { get; set; }
        public List<string> formamide_plates { get; set; }
        public List<string> buffer_5_plates { get; set; }
    }//RunInfo

    public class HybSetup_Response
    {
        public HybSetup_RunInfo data { get; set; }
        public List<LIMSResponse_Errors> errors { get; set; }
    }//RunInfo
}
