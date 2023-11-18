using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;


namespace DLL_LIMS_Workflow
{
    public static class LIMS_Workflow
    {
        public static void ReadErrorsInResponse(List<LIMSResponse_Errors> myResponseErrors)
        {
            string errorsOut = "";
            if (myResponseErrors.Count > 0)
            {
                foreach (LIMSResponse_Errors myError in myResponseErrors)
                {
                    errorsOut = errorsOut + myError.field + ": " + myError.message + ": " + myError.error_class + "\r\n";
                }
            }
            if (errorsOut != "")
            {
                throw new Exception("ERRORS: " + errorsOut);
            }
        }

        //public static List<string>[] getHybSetupInventory(string[] incubatorInventoryArray, string baitTubeFilePath_In, int numPlates)
        //{
        //    try
        //    {
        //        List<string>[] _myInventory = new List<string>[4];
        //        List<string> myTubes = new List<string>();
        //        using (StreamReader sr = File.OpenText(baitTubeFilePath_In))
        //        {
        //            string s = "";
        //            while ((s = sr.ReadLine()) != null)
        //            {
        //                if (s.Split(',')[1] != " NO READ")
        //                {
        //                    myTubes.Add(s.Split(',')[1].Trim());
        //                }
        //            }
        //        }
        //        _myInventory[0] = myTubes;
        //        _myInventory[1] = new List<string>();
        //        _myInventory[2] = new List<string>();
        //        _myInventory[3] = new List<string>();
        //        ///////////////////////////////////////////////////
        //        String[] myBarcodesArray_HybPlates = incubatorInventoryArray.Skip(6).Take(numPlates).ToArray();
        //        foreach (string index in myBarcodesArray_HybPlates)
        //        {
        //            _myInventory[1].Add(index);
        //        }
        //        String[] myBarcodesArray_Formamide = incubatorInventoryArray.Take(3).ToArray();
        //        _myInventory[2].Add(myBarcodesArray_Formamide[0]);
        //        if (myBarcodesArray_Formamide[1] != "No Plate" || myBarcodesArray_Formamide[2] != "No Plate")
        //        {
        //            _myInventory[2].Add(myBarcodesArray_Formamide[1]);
        //        }
        //        if (myBarcodesArray_Formamide[2] != "No Plate")
        //        {
        //            _myInventory[2].Add(myBarcodesArray_Formamide[2]);
        //        }
        //        String[] myBarcodesArray_HybBuffer = incubatorInventoryArray.Skip(3).Take(3).ToArray();
        //        _myInventory[3].Add(myBarcodesArray_HybBuffer[0]);
        //        if (myBarcodesArray_HybBuffer[1] != "No Plate" || myBarcodesArray_HybBuffer[2] != "No Plate")
        //        {
        //            _myInventory[3].Add(myBarcodesArray_HybBuffer[1]);
        //        }
        //        if (myBarcodesArray_HybBuffer[2] != "No Plate")
        //        {
        //            _myInventory[3].Add(myBarcodesArray_HybBuffer[2]);
        //        }
        //        return _myInventory;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception("getHybSetupInventory method: " + ex.Message);
        //    }
        //}//RunInfo

        //public static HybSetup_RunInfo getOneHybSetupPlateGroup(HybSetup_RunInfo myInfo_In, int index_In)
        //{
        //    try
        //    {
        //        int index = index_In - 1;
        //        HybSetup_Plates oneHybPlate = myInfo_In.hyb_plates[index];
        //        HybSetup_RunInfo onePlateInfo = new HybSetup_RunInfo();
        //        onePlateInfo.automation_system = myInfo_In.automation_system;
        //        onePlateInfo.module = myInfo_In.module;
        //        onePlateInfo.hyb_plates = new List<HybSetup_Plates>();
        //        onePlateInfo.pooled_ipcr_plates = myInfo_In.pooled_ipcr_plates;
        //        onePlateInfo.bait_tube_scanfile_name = myInfo_In.bait_tube_scanfile_name;
        //        onePlateInfo.hyb_plate_column_qty = myInfo_In.hyb_plate_column_qty;
        //        onePlateInfo.hyb_plates.Add(oneHybPlate);
        //        return onePlateInfo;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception("getOneHybSetupPlateGroup method: " + ex.Message);
        //    }
        //}//RunInfo

        //public static List<Hyb_Plate> CreateHybPlates(string[] myReagents)
        //{
        //    try
        //    {
        //        List<Hyb_Plate> myPlates = new List<Hyb_Plate>();
        //        int numPlates = myReagents.Length / 18;
        //        for (int i = 0; i < numPlates; i++)
        //        {
        //            String[] plate = myReagents.Skip(i * 18).Take(18).ToArray();
        //            Hyb_Plate newPlate = new Hyb_Plate();
        //            newPlate.robot_positional_index = (i + 1).ToString();
        //            newPlate.streptavidin_beads = plate[0];
        //            newPlate.buffer_5 = plate[1];
        //            newPlate.heated_wb_1 = plate[2];
        //            newPlate.wash_buffer_4_1 = plate[3];
        //            newPlate.wash_buffer_4_2 = plate[4];
        //            newPlate.wash_buffer_1 = plate[5];
        //            newPlate.wash_buffer_2 = plate[6];
        //            newPlate.wash_buffer_3 = plate[7];
        //            newPlate.post_cap_eb = plate[8];
        //            newPlate.post_cap_axy_beads = plate[9];
        //            newPlate.post_cap_lib_plate = plate[10];
        //            newPlate.formamide = plate[11];
        //            newPlate.pcr_master_mix = plate[12];
        //            newPlate.post_cap_primer_mix = plate[13];
        //            newPlate.post_cap_etoh = plate[14];
        //            newPlate.bead_wash_buffer = plate[15];
        //            newPlate.post_cap_norm_plate = plate[16];
        //            newPlate.hyb_plate = plate[17];

        //            myPlates.Add(newPlate);
        //        }
        //        return myPlates;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception("CreateHybPlates method: " + ex.Message);
        //    }
        //}//RunInfo

        //public static Hyb_RunInfo getOneHybPlateGroup(Hyb_RunInfo myInfo_In, int index_In)
        //{
        //    try
        //    {
        //        int index = index_In - 1;
        //        Hyb_Plate onePlateGroup = myInfo_In.hyb_plates[index];
        //        Hyb_RunInfo onePlateInfo = new Hyb_RunInfo();
        //        onePlateInfo.automation_system = myInfo_In.automation_system;
        //        onePlateInfo.module = myInfo_In.module;
        //        onePlateInfo.hyb_plates = new List<Hyb_Plate>();
        //        onePlateInfo.hyb_plates.Add(onePlateGroup);
        //        return onePlateInfo;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception("getOneHybPlateGroup method: " + ex.Message);
        //    }
        //}//RunInfo
    }

    //////////////////////// Verso System 
    public class StepPayload_Inputs_InitBatchSelectionWF
    {
        public string assay_id { get; set; }
        public StepPayload_Inputs_TubeMapFile tube_map_file { get; set; }
    }


    public class StepPayload_Inputs_InitStampUNPToTubesWF
    {
        public string unp_plate_id { get; set; }
        public StepPayload_Inputs_TubeMapFile tube_map_file { get; set; }
    }

    public class StepPayload_Inputs_InitRetrieve
    {
        public string rack_id { get; set; }
    }

    public class StepPayload_Inputs_InitStamp
    {
        public string machine_id { get; set; }
        public string eb_buffer { get; set; }
        public string stamp_plate_id { get; set; }
        public StepPayload_Inputs_TubeMapFile tube_map_file { get; set; }
    }

    public class StepPayload_Inputs_TubeMapFile
    {
        public string file_name { get; set; }
        public string file_type { get; set; }
        public string file_data { get; set; }
    }

    // public class GetDilutionWorklistPayload
    public class GetMaterialPayload
    {
        public string workflow { get; set; }
        public string material_name { get; set; }
    }

    public class GetMaterialResponse
    {
        public GetMaterialResponse_Data data { get; set; }
        public List<LIMSResponse_Errors> errors { get; set; }
    }

    public class GetMaterialResponse_Data
    {
        public string file_contents { get; set; }
        public string file_name { get; set; }
        public string workflow_id { get; set; }
        public string material_id { get; set; }
        public string material_name { get; set; }
    }

    public class GetMaterialResponseV2
    {
        public object data { get; set; }
        public List<LIMSResponse_Errors> errors { get; set; }
    }

    public class GetMaterialResponse_Data_AssayWellMap  ///Can also be used for CovarisPlateMap
    {
        public Dictionary<string,string> material { get; set; }
        public string workflow_id { get; set; }
        public string material_name { get; set; }
    }


   //special for lib step 2
    public class GetMaterialResponseAlt
    {
        public List<GetMaterialResponseAlt_Data> data { get; set; }
        public List<LIMSResponse_Errors> errors { get; set; }
    }

    public class GetMaterialResponseAlt_Data
    {
        public string unique_id { get; set; }
        public string spec_name { get; set; }
        public string status { get; set; }
        public Dictionary<string, string> assay_well_map { get; set; }
    }

    public class GetMaterialResponseAlt2
    {
        public GetMaterialResponse_Data_AssayWellMap data { get; set; }
        public List<LIMSResponse_Errors> errors { get; set; }
    }


    //public class StepPayload_Inputs_InitRetrieveAndStampStep2
    //{
    //    public string automation_system { get; set; }
    //    public string assay_plate_id { get; set; }
    //    public StepPayload_Inputs_TubeMapFile tube_map_file { get; set; }
    //}

    //public class StepPayload_Inputs_InitUNPToTubes  ///This is temporary
    //{
    //    public string unp_plate_id { get; set; }
    //    public StepPayload_Inputs_TubeMapFile tube_map_file { get; set; }
    //}

    //////////////////////// Trinean

    public class DQDataMessage
    {
        public string worklist_type { get; set; }
        public string workflow { get; set; }
        public string norm_source_plate { get; set; }
        public string norm_destination_plate { get; set; }
        public string dropquant_data { get; set; }
    }

    public class DQDataResponse
    {
        public DQDataResponse_worklist data { get; set; }
        public List<LIMSResponse_Errors> errors { get; set; }
    }

    public class DQDataResponse_worklist
    {
        public string qm_id { get; set; }
        public string workflow { get; set; }
        public string worklist { get; set; }
    }

    //////////////////////// Something
    
    public class StepPayload
    {
        public string workflow { get; set; }
        public string step_name { get; set; }
        public string user { get; set; }
        public object inputs { get; set; }
    }

    //////////////////////////////////////////////////////////////////////

    public class LIMSResponse
    {
        public List<LIMSResponse_Errors> errors { get; set; }
        public LIMSResponse_Data data { get; set; }
    }

    public class LIMSResponse_Data
    {
        public LIMSResponse_Data_Step step { get; set; }
        public LIMSResponse_Data_Workflow workflow { get; set; }

    }

    public class LIMSResponse_Data_Step
    {
        public string completed_at { get; set; }
        public string initialized_at { get; set; }
        public string name { get; set; }
    }

    public class LIMSResponse_Data_Workflow
    {
        public string status { get; set; }
        public string unique_id { get; set; }
        public List<string> batch_ids { get; set; }
    }

    public class LIMSResponse_Errors
    {
        public string field { get; set; }
        public object error_class { get; set; }
        public string message { get; set; }
    }

    ////////////////////////////////////////////////////////////////////////

    public class StepPayload_Inputs_InitExtractionStep2
    {
        public string unp_plate { get; set; }
        public string automation_system { get; set; }
        public string pq_plate { get; set; }
    }

    public class StepPayload_Inputs_InitExtractionStep2_V2
    {
        public string automation_system { get; set; }
        public string rack_id { get; set; }
        public string pq_plate { get; set; }
        public StepPayload_Inputs_TubeMapFile tube_map_file { get; set; }
    }

    public class StepPayload_Inputs_CompleteExtractionStep2
    {
        public string quant_metric { get; set; }
    }

    public class StepPayload_Inputs_InitExtractionStep3
    {
        public string unp_plate { get; set; }
        public string stamped_unp_1_id { get; set; }
        public string stamped_unp_2_id { get; set; }
    }

    public class StepPayload_Inputs_InitExtractionStep3_V2
    {
        public string assay_plate { get; set; }
        public string rack_id { get; set; }
        public StepPayload_Inputs_TubeMapFile matching_tube_map_file { get; set; }
    }

    public class StepPayload_Inputs_InitLibraryStep1
    {
        public string shearing_instrument { get; set; }
        public string covaris_plate { get; set; }
        public string unp { get; set; }
    }

    public class StepPayload_Inputs_InitLibraryStep1_V2
    {
        public object covaris_plates { get; set; }
        public string sheared_dna_plate_id { get; set; }
    }

    public class StepPayload_Inputs_InitLibraryStep1_V2_CovarisPlates//384
    {
        public string upper_left_plate_id { get; set; }
        public string upper_right_plate_id { get; set; }
        public string lower_left_plate_id { get; set; }
        public string lower_right_plate_id { get; set; }
    }

    public class StepPayload_Inputs_InitLibraryStep1_V2_CovarisPlate
    {

        public string plate_id { get; set; }//For 96 well lib plates
    }

    public class StepPayload_Inputs_InitShearingStep1
    {
        public string shearing_instrument { get; set; }
        public string covaris_plate { get; set; }
        public string assay_plate { get; set; }
    }

    public class StepPayload_Inputs_InitLibraryStep2
    {
        public string covaris_recovery_instrument { get; set; }
        public string covaris_plate { get; set; }
        public string sheared_dna_plate { get; set; }
    }

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
        public string post_shear_ethanol { get; set; }
        public string post_ligation_ethanol { get; set; }
        public string post_ipcr_ethanol { get; set; }
        public string post_shear_eb_buffer { get; set; }
        public string post_ligation_eb_buffer { get; set; }
        public string post_ipcr_eb_buffer { get; set; }

    }

    public class StepPayload_Inputs_CompleteLibraryStep3
    {
        //No Inputs
    }

    public class StepPayload_Inputs_InitLibraryStep4
    {
        public string ipcr_libs_plate { get; set; }
    }

    public class StepPayload_Inputs_PreCapQuant
    {
        public string ipcr_libs_plate { get; set; }
        public string pre_cap_pool_plate { get; set; }
        public string pre_cap_quant_metric { get; set; }
    }

    ///////////////////////////////////////////////////////////////////////

    public class GetQuantMetric
    {
        public string drop_quant_data { get; set; }
        public Dictionary<string,string> assay_well_map { get; set; }
        public string report_name { get; set; }
        public string plate_id { get; set; }
        public string assay_id { get; set; }
        public string lab_workflow_id { get; set; }
    }

    public class GetQuantMetricResponse
    {
        public List<LIMSResponse_Errors> errors { get; set; }
        public GetQuantMetricResponse_Data data { get; set; } ///Why does this have to be an Object?????
    }

    public class GetQuantMetricResponse_Data
    {
        public string quant_metric { get; set; }
    }


    ////////////////////////////////////////////////////////////////////////

    public class WorkflowPayload
    {
        public string request_id { get; set; }
        public string name { get; set; }
        public string spec_name { get; set; }
        public object inputs { get; set; }
    }

    public class WorkflowPayload_Inputs_InitLibBatchSelection
    {
        public string library_prep_plate_format { get; set; }
        public List<string> request_ids { get; set; }
    }

    public class WorkflowPayload_StampToUNP
    {
        public string name { get; set; }
        public string spec_name { get; set; }
        public object inputs { get; set; }
    }

    public class WFQueryResponse
    {
        public List<LIMSResponse_Errors> errors { get; set; }
        public List<WFQueryResponse_Data> data { get; set; }
    }

    public class WFQueryResponse_Data
    {
        public string spec_name { get; set; }
        public string unique_id { get; set; }
        public string assay { get; set; }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ///THESE ARE ALL REDUNDANT !!!!!! Should only be using the TasksQueryResponse object really
    public class ShearingTasksQueryResponse
    {
        public List<LIMSResponse_Errors> errors { get; set; }
        public List<ShearingTasksQueryResponse_Data> data { get; set; }
    }

    public class ShearingTasksQueryResponse_Data
    {
        public List<string> parent_workflow_ids { get; set; }
        public string sent_at { get; set; }
        public string request_id { get; set; }
        public List<ShearingTasksQueryResponse_Data_CurrentSpecVersions> current_spec_versions { get; set; }
        public ShearingTasksQueryResponse_Data_Arguments arguments { get; set; }
        public string spec_type { get; set; }
    }

    public class ShearingTasksQueryResponse_Data_CurrentSpecVersions
    {
        public string versioned_name { get; set; }
        public string display_name { get; set; }
    }

    public class ShearingTasksQueryResponse_Data_Arguments
    {
        public string protocol_type { get; set; }
        public string assay_plate_id { get; set; }
        public string contains_priority { get; set; }
    }
    /// /////////////////////////////////////////////////////////////////////

    public class LibraryTasksQueryResponse
    {
        public List<LIMSResponse_Errors> errors { get; set; }
        public List<LibraryTasksQueryResponse_Data> data { get; set; }
    }

    public class LibraryTasksQueryResponse_Data
    {
        public List<string> parent_workflow_ids { get; set; }
        public string sent_at { get; set; }
        public string request_id { get; set; }
        public List<LibraryTasksQueryResponse_Data_CurrentSpecVersions> current_spec_versions { get; set; }
        public LibraryTasksQueryResponse_Data_Arguments arguments { get; set; }
        public string spec_type { get; set; }
    }

    public class LibraryTasksQueryResponse_Data_CurrentSpecVersions
    {
        public string versioned_name { get; set; }
        public string display_name { get; set; }
    }

    public class LibraryTasksQueryResponse_Data_Arguments
    {
        public string assay_id { get; set; }
        public string uses { get; set; }
        public string protocol_type { get; set; }
        public string unp_id { get; set; }//This is set to go away
        public string contains_priority { get; set; }
        public List<string> sheared_dna_plate_ids { get; set; }
    }

    ////////////////////////////////////////////////////
    public class TasksQueryResponse
    {
        public List<LIMSResponse_Errors> errors { get; set; }
        public List<TasksQueryResponse_Data> data { get; set; }
    }

    public class TasksQueryResponseV2
    {
        public List<LIMSResponse_Errors> errors { get; set; }
        public List<TasksQueryResponse_DataV2> data { get; set; }
    }

    public class TasksQueryResponse_Data//oops not generalized
    {
        public List<string> parent_workflow_ids { get; set; }
        public string sent_at { get; set; }
        public string request_id { get; set; }
        public List<TasksQueryResponse_Data_CurrentSpecVersions> current_spec_versions { get; set; }
        public GatherAndStampTasksQueryResponse_Data_Arguments arguments { get; set; }
        public string spec_type { get; set; }
    }

    public class TasksQueryResponse_DataV2 //generalized
    {
        public List<string> parent_workflow_ids { get; set; }
        public string sent_at { get; set; }
        public string request_id { get; set; }
        public List<TasksQueryResponse_Data_CurrentSpecVersions> current_spec_versions { get; set; }
        public object arguments { get; set; }
        public string spec_type { get; set; }
    }

    public class TasksQueryResponse_Data_CurrentSpecVersions
    {
        public string versioned_name { get; set; }
        public string display_name { get; set; }
    }

    ///////////////////////////////////////////////
    //Only Arguments should ever change for this!!!objec
    public class GatherAndStampTasksQueryResponse_Data_Arguments
    {
        public string platform { get; set; }
        public List<string> batch_ids { get; set; }
        public string workflow_mode { get; set; }
        public string protocol_type { get; set; }
    }

    public class TasksQueryResponse_Data_Arguments
    {
        public string platform { get; set; }
        public List<string> batch_ids { get; set; }
        public string workflow_mode { get; set; }
        public string protocol_type { get; set; }
    }

    public class TasksQueryResponse_Data_Arguments_LibPrepBatchSel
    {
        public string platform { get; set; }
        public string assay_id { get; set; }
        public string batch_info { get; set; }
        public List<string> assay_batch_ids { get; set; }
        public string workflow_mode { get; set; }
        public string protocol_type { get; set; }
        public string sheared_dna_plate_id { get; set; }
    }

    public class TasksQueryResponse_Data_Arguments_LibPrepTaskSel
    {
        public string library_prep_plate_format { get; set; }
        public string platform { get; set; }
        public string batch_info { get; set; }
        public List<string> assay_batch_ids { get; set; }
        public string workflow_mode { get; set; }
        public string protocol_type { get; set; }
        public List<string> sheared_dna_plate_ids { get; set; }
    }


    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////



    //////Library

    public class PlateTypeQuery_Payload
    {
        public List<string> container_ids { get; set; }
    }

    public class PlateTypeQuery_Response
    {
        public List<LIMSResponse_Errors> errors { get; set; }
        public List<PlateTypeQuery_Response_Data> data { get; set; }
    }

    public class PlateTypeQuery_Response_Data
    {
        public string container_id { get; set; }
        public string designation { get; set; }
        public bool validated { get; set; }
        public DateTime expiration { get; set; }
        public bool used { get; set; }
        public string plate_format { get; set; }
    }

    //

  
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
    }

    public class GetPreCapWorklist_Payload_Inputs
    {
        public string pre_cap_pool_plate { get; set; }
        public string ipcr_libs_plate { get; set; }
    }

    public class GetPreCapWorklist_Response
    {
        public List<LIMSResponse_Errors> errors { get; set; }
        public GetPreCapWorklist_Response_Data data { get; set; }
    }

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
    }

    /////Convienience APIs

    public class HybSetup_InventoryMessage
    {
        public string module { get; set; }
        public string automation_system { get; set; }
        public int hyb_plate_column_qty { get; set; }
        public List<string> pooled_ipcr_plates { get; set; }
        public List<string> bait_tubes { get; set; }
        public List<string> formamide_plates { get; set; }
        public List<string> buffer_5_plates { get; set; }
    }


    /////Redundant with Run Output///////////////////////////

    public class HybSetup_RunInfo
    {
        public string automation_system { get; set; }
        public string module { get; set; }
        public string bait_tube_scanfile_name { get; set; }//For Me only!!
        public int hyb_plate_column_qty { get; set; }//Only on Init
        public List<string> pooled_ipcr_plates { get; set; }//For Me only!!
        public List<HybSetup_Plates> hyb_plates { get; set; }
    }

    public class HybSetup_Plates
    {
        public string workflow { get; set; }
        public string hyb_plate { get; set; }//From BC Read
        public string formamide { get; set; }
        public string buffer_5 { get; set; }
        public List<HybSetup_ColumnData> column_data { get; set; }
    }

    public class HybSetup_ColumnData
    {
        public string pooled_ipcr_plate { get; set; }
        public int source_plate_column_qty { get; set; }
        public int source_column_number { get; set; }
        public int target_column_number { get; set; }
        public string bait_tube { get; set; }
    }

    /// 
    public class Hyb_RunInfo
    {
        public string automation_system { get; set; }
        public string module { get; set; }
        public List<Hyb_Plate> hyb_plates { get; set; }
    }

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
    }

    /// 
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

    /// ///////////////////////////////////////////////////////

    public class HybSetup_Response
    {
        public HybSetup_RunInfo data { get; set; }
        public List<LIMSResponse_Errors> errors { get; set; }
    }

    public class Hyb_Response
    {
        public Hyb_RunInfo data { get; set; }
        public List<LIMSResponse_Errors> errors { get; set; }
    }

    public class Library_Response
    {
        public Library_RunInfo data { get; set; }
        public List<LIMSResponse_Errors> errors { get; set; }
    }

    
}
