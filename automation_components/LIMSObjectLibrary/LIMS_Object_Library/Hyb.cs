using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LIMSObjectLibrary
{
    public static class Hyb
    {
        public static List<Hyb_Plate> CreateHybPlates(string[] myReagents)
        {
            try
            {
                List<Hyb_Plate> myPlates = new List<Hyb_Plate>();
                int numPlates = myReagents.Length / 18;
                for (int i = 0; i < numPlates; i++)
                {
                    String[] plate = myReagents.Skip(i * 18).Take(18).ToArray();
                    Hyb_Plate newPlate = new Hyb_Plate();
                    newPlate.robot_positional_index = (i + 1).ToString();
                    newPlate.streptavidin_beads = plate[0];
                    newPlate.buffer_5 = plate[1];
                    newPlate.heated_wb_1 = plate[2];
                    newPlate.wash_buffer_4_1 = plate[3];
                    newPlate.wash_buffer_4_2 = plate[4];
                    newPlate.wash_buffer_1 = plate[5];
                    newPlate.wash_buffer_2 = plate[6];
                    newPlate.wash_buffer_3 = plate[7];
                    newPlate.post_cap_eb = plate[8];
                    newPlate.post_cap_axy_beads = plate[9];
                    newPlate.post_cap_lib_plate = plate[10];
                    newPlate.formamide = plate[11];
                    newPlate.pcr_master_mix = plate[12];
                    newPlate.post_cap_primer_mix = plate[13];
                    newPlate.post_cap_etoh = plate[14];
                    newPlate.bead_wash_buffer = plate[15];
                    newPlate.post_cap_norm_plate = plate[16];
                    newPlate.hyb_plate = plate[17];

                    myPlates.Add(newPlate);
                }
                return myPlates;
            }
            catch (Exception ex)
            {
                throw new Exception("CreateHybPlates method: " + ex.Message);
            }
        }//RunInfo

        public static Hyb_RunInfo getOneHybPlateGroup(Hyb_RunInfo myInfo_In, int index_In)
        {
            try
            {
                int index = index_In - 1;
                Hyb_Plate onePlateGroup = myInfo_In.hyb_plates[index];
                Hyb_RunInfo onePlateInfo = new Hyb_RunInfo();
                onePlateInfo.automation_system = myInfo_In.automation_system;
                onePlateInfo.module = myInfo_In.module;
                onePlateInfo.hyb_plates = new List<Hyb_Plate>();
                onePlateInfo.hyb_plates.Add(onePlateGroup);
                return onePlateInfo;
            }
            catch (Exception ex)
            {
                throw new Exception("getOneHybPlateGroup method: " + ex.Message);
            }
        }//RunInfo
    }

    public class Hyb_RunInfo
    {
        public string automation_system { get; set; }
        public string module { get; set; }
        public List<Hyb_Plate> hyb_plates { get; set; }
    }//RunInfo

    public class Hyb_Plate
    {
        public string robot_positional_index { get; set; }
        public string workflow { get; set; }//comes back with init response
        public string hyb_plate { get; set; }//Barcode Read? Manual Entry?
        public string bead_wash_buffer { get; set; }
        public string heated_wb_1 { get; set; }
        public string wash_buffer_1 { get; set; }
        public string wash_buffer_2 { get; set; }
        public string wash_buffer_3 { get; set; }
        public string wash_buffer_4_1 { get; set; }
        public string wash_buffer_4_2 { get; set; }
        public string post_cap_eb { get; set; }
        public string formamide { get; set; }
        public string buffer_5 { get; set; }
        public string streptavidin_beads { get; set; }
        public string post_cap_primer_mix { get; set; }
        public string pcr_master_mix { get; set; }
        public string post_cap_axy_beads { get; set; }
        public string post_cap_etoh { get; set; }
        public string post_cap_lib_plate { get; set; }
        public string post_cap_norm_plate { get; set; }
        public string post_cap_norm_worklist_filename { get; set; } //written by trinaen
        public string post_cap_norm_dq1_plate_id { get; set; } //written by this from [Barcode], read by trinean
        public string post_cap_norm_dq2_plate_id { get; set; } //written by this from [Barcode], read by trinean
    }//RunInfo

    public class Hyb_Response
    {
        public Hyb_RunInfo data { get; set; }
        public List<LIMSResponse_Errors> errors { get; set; }
    }//RunInfo

}
