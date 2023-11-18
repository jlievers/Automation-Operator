using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LIMSObjectLibrary
{
    public static class Library
    {
        //public  List<Library_WorkFlowPlateGroup> CreatePlateGroups(string[] myBarcodesArray, int _incubatorZoneSize)
        //{
        //    try
        //    {
        //        String[] myBarcodesArray_Samples = myBarcodesArray.Take(_incubatorZoneSize).ToArray();
        //        String[] myBarcodesArray_ER_AT = myBarcodesArray.Skip(_incubatorZoneSize).Take(_incubatorZoneSize).ToArray();
        //        String[] myBarcodesArray_Ligation = myBarcodesArray.Skip(_incubatorZoneSize * 2).Take(_incubatorZoneSize).ToArray();
        //        String[] myBarcodesArray_YAdapter = myBarcodesArray.Skip(_incubatorZoneSize * 3).Take(_incubatorZoneSize).ToArray();
        //        String[] myBarcodesArray_Indexes = myBarcodesArray.Skip(_incubatorZoneSize * 4).Take(_incubatorZoneSize).ToArray();
        //        String[] myBarcodesArray_PCRMM = myBarcodesArray.Skip(_incubatorZoneSize * 5).Take(_incubatorZoneSize).ToArray();
        //        List<Library_WorkFlowPlateGroup> myPlateGroupList = new List<Library_WorkFlowPlateGroup>();

        //        for (int i = 0; i < _incubatorZoneSize; i++)
        //        {
        //            Library_WorkFlowPlateGroup newPlateGroup = new Library_WorkFlowPlateGroup();
        //            newPlateGroup.plate_transition_group = (i + 1);
        //            newPlateGroup.sheared_dna_plate = myBarcodesArray_Samples[i];
        //            newPlateGroup.er_at_plate_id = myBarcodesArray_ER_AT[i];
        //            newPlateGroup.ligation_plate_id = myBarcodesArray_Ligation[i];
        //            newPlateGroup.y_adapter_plate_id = myBarcodesArray_YAdapter[i];
        //            newPlateGroup.index_plate_id = myBarcodesArray_Indexes[i];
        //            newPlateGroup.pcr_master_mix_plate_id = myBarcodesArray_PCRMM[i];
        //            if (!IgnoreIncIndexGroup(newPlateGroup))
        //            {
        //                myPlateGroupList.Add(newPlateGroup);
        //            }
        //        }
        //        return myPlateGroupList;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception("CreatePlateGroups method: " + ex.Message);
        //    }
        //}

        public static List<ReagentIndexSet> CreatePlateGroups(List<PlateTypeQuery_Response_Data> myPlateInfo, string incubatorInventoryString, int numPlates)
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
        }//RunInfo

        public static List<string> wrongPlatetypeList(String[] myList, string plateType, List<PlateTypeQuery_Response_Data> _platesInfo)
        {
            List<string> errors = new List<string>();
            foreach (string myBarcode in myList)
            {
                foreach (PlateTypeQuery_Response_Data plateInfo in _platesInfo)
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
        }//RunInfo

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
        } ///NOT USED HERE! DELETE FROM UTILITIES//RunInfo

        //public bool IgnoreIncIndexGroup(Library_WorkFlowPlateGroup myPlateGroup)
        //{
        //    try
        //    {
        //        bool ignore = false;
        //        Dictionary<string, string> myProperties = new Dictionary<string, string>();
        //        myProperties.Add("sample_plate_id", myPlateGroup.sheared_dna_plate);
        //        myProperties.Add("er_at_plate_id", myPlateGroup.er_at_plate);
        //        myProperties.Add("ligation_plate_id", myPlateGroup.ligase_mix_plate);
        //        myProperties.Add("y_adapter_plate_id", myPlateGroup.y_adapter_plate);
        //        myProperties.Add("index_plate_id", myPlateGroup.sequence_index_plate);
        //        myProperties.Add("pcr_master_mix_plate_id", myPlateGroup.kapa_pcr_mm_plate);

        //        if (myProperties.Values.Contains("_"))//Added by Overlord when past plateNumber but not incZoneSize
        //        {
        //            if (myProperties.Values.Distinct().Count() == 1)//all underscores = no plates for Incubator PlateGroup Index 
        //            {
        //                ignore = true;
        //            }
        //        }
        //        return ignore;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception("IgnoreIncIndexGroup method: " + ex.Message);
        //    }
        //}

        //    public Library_RunInfo getOneLibPlateGroup(LIMSObjectLibrary.Library_RunInfo myInfo_In, int index_In)
        //    {
        //        try
        //        {
        //            int index = index_In - 1;
        //            Library_WorkFlowPlateGroup oneLiblate = myInfo_In.plate_data[index];
        //            Library_RunInfo onePlateInfo = new Library_RunInfo();
        //            onePlateInfo.automation_system = myInfo_In.automation_system;
        //            onePlateInfo.module = myInfo_In.module;
        //            onePlateInfo.plate_data = new List<Library_WorkFlowPlateGroup>();
        //            onePlateInfo.plate_data.Add(oneLiblate);
        //            return onePlateInfo;
        //        }
        //        catch (Exception ex)
        //        {
        //            throw new Exception("getOneLibPlateGroup method: " + ex.Message);
        //        }
        //    }

    }

    public class Library_RunInfo
    {
        public string automation_system { get; set; }
        public string module { get; set; }
        public List<Library_WorkFlowPlateGroup> plate_data { get; set; }
    }//RunInfo

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

    }  //RunInfo

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

    public class Library_Response
    {
        public Library_RunInfo data { get; set; }
        public List<LIMSResponse_Errors> errors { get; set; }
    }  //RunInfo

    /////

    public class libraryInventory
    {
        public List<ReagentIndexSet> reagentIndexSets { get; set; }
    }//RunInfo

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
    }//RunInfo

    /////

    public class PlateTypeQuery_Payload
    {
        public List<string> container_ids { get; set; }
    }//LIMS_Workflow 

    public class PlateTypeQuery_Response
    {
        public List<LIMSResponse_Errors> errors { get; set; }
        public List<PlateTypeQuery_Response_Data> data { get; set; }
    }//LIMS_Workflow 

    public class PlateTypeQuery_Response_Data
    {
        public string container_id { get; set; }
        public string designation { get; set; }
        public DateTime expiration { get; set; }
        public bool used { get; set; }
    }//LIMS_Workflow 

    /////

    public class StepPayload_Inputs_InitLibraryStep3
    {
        public string automation_system { get; set; }
        public string sheared_dna_plate { get; set; }
        public string sequence_index_plate { get; set; }
        public string er_at_plate { get; set; }
        public string ligase_mix_plate { get; set; }
        public string y_adapter_plate { get; set; }
        public string kapa_pcr_mm_plate { get; set; }
        public string axygen_plate { get; set; }
        public string ethanol { get; set; }
        public string eb_buffer { get; set; }
    }//LIMS_Workflow 

    public class StepPayload_Inputs_CompleteLibraryStep3
    {
        //No Inputs
    }//LIMS_Workflow 

    public class StepPayload_Inputs_InitLibraryStep4
    {
        public string ipcr_libs_plate { get; set; }
    }//LIMS_Workflow 

    ////

    public class GePreCaptWorklist_Payload
    {
        public string user { get; set; }
        public string workflow { get; set; }
        public string step_name { get; set; }
        public string report_name { get; set; }
        public string plate_id { get; set; }
        public string assay_id { get; set; }
        public string drop_quant_data { get; set; }
        public string material_name { get; set; }
        public GetPreCapWorklist_Payload_Inputs step_inputs { get; set; }
    }//LIMS_Workflow 

    public class GetPreCapWorklist_Payload_Inputs
    {
        public string pre_cap_pool_plates { get; set; }
        public string ipcr_libs_plate { get; set; }
    }//LIMS_Workflow 

    public class GetPreCapWorklist_Response
    {
        public List<LIMSResponse_Errors> errors { get; set; }
        public GetPreCapWorklist_Response_Data data { get; set; }
    }//LIMS_Workflow 

    public class GetPreCapWorklist_Response_Data
    {
        public List<string> errors { get; set; }
        public string file_name { get; set; }
        public string file_contents { get; set; }
        public string material_id { get; set; }
        public string workflow_id { get; set; }
        public string material_name { get; set; }
        public LIMSResponse_Data_Step step { get; set; }
        public LIMSResponse_Data_Workflow workflow { get; set; }
    }//LIMS_Workflow 

}
