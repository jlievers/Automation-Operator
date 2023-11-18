using System;
using System.Collections.Generic;
using System.Linq;

namespace Com.Invitae.LabAutomation.Model
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

}
