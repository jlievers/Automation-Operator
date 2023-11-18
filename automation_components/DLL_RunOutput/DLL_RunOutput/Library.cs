using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using DLL_LIMS_Workflow;

namespace DLL_RunOutput
{
    public static class Library
    {
        public static List<ReagentIndexSet> CreatePlateGroups(List<DLL_LIMS_Workflow.PlateTypeQuery_Response_Data> myPlateInfo, string incubatorInventoryString, int numPlates)
        {
            try
            {
                string[] myBarcodesArray = incubatorInventoryString.Split(',');
                string[] myBarcodesArray_ER_AT = myBarcodesArray.Take(numPlates).ToArray();
                string[] myBarcodesArray_Ligation = myBarcodesArray.Skip(numPlates).Take(numPlates).ToArray();
                string[] myBarcodesArray_YAdapter = myBarcodesArray.Skip(numPlates * 2).Take(numPlates).ToArray();
                string[] myBarcodesArray_Indexes = myBarcodesArray.Skip(numPlates * 3).Take(numPlates).ToArray();
                string[] myBarcodesArray_PCRMM = myBarcodesArray.Skip(numPlates * 4).Take(numPlates).ToArray();
                string[] myBarcodesArray_EtOH = myBarcodesArray.Skip(numPlates * 5).Take(numPlates).ToArray();
                string[] myBarcodesArray_EB = myBarcodesArray.Skip(numPlates * 6).Take(numPlates).ToArray();
                string[] myBarcodesArray_Beads = myBarcodesArray.Skip(numPlates * 7).Take(numPlates).ToArray();
                List<string> allErrors = wrongPlatetypeList(myBarcodesArray_ER_AT, "ER/AT Mix", myPlateInfo);
                allErrors.Concat(allErrors = wrongPlatetypeList(myBarcodesArray_Ligation, "Ligase Mix", myPlateInfo)).ToArray();
                allErrors.Concat(allErrors = wrongPlatetypeList(myBarcodesArray_YAdapter, "96 Well Y Adapter", myPlateInfo)).ToArray();
                allErrors.Concat(allErrors = wrongPlatetypeList(myBarcodesArray_Indexes, "Sequence Index Stock Plate", myPlateInfo)).ToArray();
                allErrors.Concat(allErrors = wrongPlatetypeList(myBarcodesArray_PCRMM, "Kapa PCR Master Mix", myPlateInfo)).ToArray();
                allErrors.Concat(allErrors = wrongPlatetypeList(myBarcodesArray_EtOH, "80% EtOH", myPlateInfo)).ToArray();
                allErrors.Concat(allErrors = wrongPlatetypeList(myBarcodesArray_EB, "EB Buffer", myPlateInfo)).ToArray();
                allErrors.Concat(allErrors = wrongPlatetypeList(myBarcodesArray_Beads, "Axygen Beads 240uL", myPlateInfo)).ToArray();
                if (allErrors.Count() != 0)
                {
                    string errorMessage = "";
                    foreach (string oneError in allErrors)
                    {
                        errorMessage = errorMessage + "\r\n" + oneError;
                    }
                    throw new Exception(errorMessage);
                }

                List<ReagentIndexSet> myPlateGroupList = new List<ReagentIndexSet>();
                for (int i = 0; i < numPlates; i++)
                {
                    ReagentIndexSet newPlateGroup = new ReagentIndexSet();
                    newPlateGroup.er_at_plate = myBarcodesArray_ER_AT[i];
                    newPlateGroup.ligase_mix_plate = myBarcodesArray_Ligation[i];
                    newPlateGroup.y_adapter_plate = myBarcodesArray_YAdapter[i];
                    newPlateGroup.sequence_index_plate = myBarcodesArray_Indexes[i];
                    newPlateGroup.kapa_pcr_mm_plate = myBarcodesArray_PCRMM[i];
                    newPlateGroup.ethanol = myBarcodesArray_EtOH[i];
                    newPlateGroup.eb_buffer = myBarcodesArray_EB[i];
                    newPlateGroup.axygen_plate = myBarcodesArray_Beads[i];

                    myPlateGroupList.Add(newPlateGroup);
                }
                return myPlateGroupList;
            }
            catch (Exception ex)
            {
                throw new Exception("CreatePlateGroups method: " + ex.Message);
            }
        }

        public static List<string> wrongPlatetypeList(String[] myList, string plateType, List<DLL_LIMS_Workflow.PlateTypeQuery_Response_Data> _platesInfo)
        {
            List<string> errors = new List<string>();
            foreach (string myBarcode in myList)
            {
                foreach (DLL_LIMS_Workflow.PlateTypeQuery_Response_Data plateInfo in _platesInfo)
                {
                    if (plateInfo.container_id == myBarcode)
                    {
                        if (plateInfo.designation != plateType)
                        {
                            errors.Add("Wrong Plate Type. Barcode: " + myBarcode + ". System Designation: " + plateType + ". LIMS Designation: " + plateInfo.designation);
                        }
                        if (plateInfo.expiration < DateTime.Now)
                        {
                            errors.Add("Expired Plate. Barcode: " + plateInfo.container_id + ". LIMS Designation: " + plateInfo.designation);
                        }
                        if (plateInfo.used == true)
                        {
                            errors.Add("Plate Marked As Used. Barcode: " + plateInfo.container_id + ". LIMS Designation: " + plateInfo.designation);
                        }
                    }
                }
            }
            return errors;
        }

        public static void getIncInvStringErrors(string[] myBarcodeInputArray)
        {
            try
            {
                string allErrors = "";
                int i = 0;
                string NoPlateErrors = "";
                string NoValueErrors = "";
                string MisreadErrors = "";
                foreach (string oneSlot in myBarcodeInputArray)
                {
                    if (oneSlot == "No Plate")
                    {
                        NoPlateErrors = NoPlateErrors + i.ToString() + ",";
                    }
                    if (oneSlot == "")
                    {
                        NoValueErrors = NoValueErrors + i.ToString() + ",";
                    }
                    if (oneSlot == "Loaded")
                    {
                        MisreadErrors = MisreadErrors + i.ToString() + ",";
                    }
                    i++;
                }
                if (NoPlateErrors != "") allErrors = allErrors + " Missing Plate In Incubator Position(s)" + ":" + NoPlateErrors;
                if (NoValueErrors != "") allErrors = allErrors + " Misread Barcode In Incubator Position(s)" + ":" + NoValueErrors;
                if (MisreadErrors != "") allErrors = allErrors + " Empty Entry For Incubator Position(s)" + ":" + MisreadErrors;
                if (allErrors != "")
                {
                    throw new Exception("INVENTORY ERRORS: " + allErrors);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("getIncInvStringErrors method: " + ex.Message);
            }
        }
    }

    public class Library_RunInfo
    {
        public string automation_system { get; set; }
        public string module { get; set; }
        public List<Library_WorkFlowPlateGroup> plate_data { get; set; }
    }

    public class Library_WorkFlowPlateGroup
    {
        public string workflow { get; set; }
        public string assay { get; set; }
        public int plate_transition_group { get; set; }
        public int rgt_set_index { get; set; }
        public string sheared_dna_plate { get; set; }
        public string er_at_plate { get; set; }
        public string ligase_mix_plate { get; set; }
        public string y_adapter_plate { get; set; }
        public string sequence_index_plate { get; set; }
        public string kapa_pcr_mm_plate { get; set; }
        public string ipcr_libs_plate { get; set; }
        public string pre_cap_pool_plate { get; set; }
        public string pre_cap_normpool_combined_filename { get; set; }
        public string pre_cap_normpool_worklist_filename { get; set; }
        public string pre_cap_norm_dq1_plate_id { get; set; }
        public string pre_cap_norm_dq2_plate_id { get; set; }
        public string ethanol { get; set; }
        public string eb_buffer { get; set; }
        public string axygen_plate { get; set; }
        public string status { get; set; } //active or complete
        public string timestamp_enter { get; set; }
        public string timestamp_exit { get; set; }

    }

    public class Library_RunInfo_V1
    {
        public string automation_system { get; set; }
        public string module { get; set; }
        public List<Library_WorkFlowPlateGroup_V1> plate_data { get; set; }
    }   //Need while non rolling still exists

    public class Library_WorkFlowPlateGroup_V1
    {
        public string workflow_id { get; set; }
        public int plate_transition_group { get; set; }
        public int rgt_set_index { get; set; }
        public string sample_plate_id { get; set; }
        public string er_at_plate_id { get; set; }
        public string ligation_plate_id { get; set; }
        public string y_adapter_plate_id { get; set; }
        public string index_plate_id { get; set; }
        public string pcr_master_mix_plate_id { get; set; }
        public string ipcr_plate_id { get; set; }
        public string pre_cap_normpool_worklist_filename { get; set; }
        public string pre_cap_normpool_source_plate { get; set; }
        public string pre_cap_normpool_destination_plate { get; set; }
        public string pre_cap_norm_dq1_plate_id { get; set; }
        public string pre_cap_norm_dq2_plate_id { get; set; }
        public string shramp_plate_id { get; set; }
        public string module_2_output_plate_id { get; set; }

        public string ethanol { get; set; }
        public string eb_buffer { get; set; }
        public string axygen_100ul_plate { get; set; }
        public string axygen_50ul_plate1 { get; set; }
        public string axygen_50ul_plate2 { get; set; }

        public string status { get; set; } //active or complete
        public string timestamp_enter { get; set; }
        public string timestamp_exit { get; set; }
    }  //Need while non rolling still exists

    public class libraryInventory
    {
        public List<ReagentIndexSet> reagentIndexSets { get; set; }
    }

    public class ReagentIndexSet
    {
        public string er_at_plate { get; set; }
        public string ligase_mix_plate { get; set; }
        public string y_adapter_plate { get; set; }
        public string sequence_index_plate { get; set; }
        public string kapa_pcr_mm_plate { get; set; }
        public string ethanol { get; set; }
        public string eb_buffer { get; set; }
        public string axygen_plate { get; set; }
    }


    public class RunInfo_384Library
    {
        public string automation_system { get; set; }
        public List<OperationData_384Library> operation_data_library { get; set; }
    }

    public class OperationData_384Library
    {
        public string plate_group_name { get; set; }
        public string workflow_id { get; set; }
        public string assay_id { get; set; }
        public int reagent_set_index { get; set; }
        public string sample_plate_id { get; set; }
        public string er_at_plate_id { get; set; }
        public string ligation_plate_id { get; set; }
        public string y_adapter_plate_id { get; set; }
        public string index_plate_id { get; set; }
        public string pcr_master_mix_plate_id { get; set; }
        public string ethanol_1 { get; set; }
        public string ethanol_2 { get; set; }
        public string ethanol_3 { get; set; }
        public string eb_buffer_1 { get; set; }
        public string eb_buffer_2 { get; set; }
        public string eb_buffer_3 { get; set; }
        public string axygen_240ul_plate { get; set; }
        public string ipcr_libs_plate { get; set; }
        public string pre_cap_pool_plates { get; set; }
        public string pre_cap_normpool_worklist_filename { get; set; }
        public string pre_cap_norm_dq1_plate_id { get; set; }
        public string pre_cap_norm_dq2_plate_id { get; set; }
        public string pre_cap_norm_dq3_plate_id { get; set; }
        public string pre_cap_norm_dq4_plate_id { get; set; }
        public string status { get; set; } //active or complete
        public string timestamp_enter { get; set; }
        public string timestamp_exit { get; set; }

    }

}


