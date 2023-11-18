using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.Windows.Forms;
//using System.Threading.Tasks;

namespace LIMSObjectLibrary
{
    #region 'LIMS OBJECTS'

    public class StepPayload
    {
        public string workflow { get; set; }
        public string step_name { get; set; }
        public string user { get; set; }
        public object inputs { get; set; }
    }////LIMS_Workflow 

    //////////////////////////////////////////////////////////////////////

    public class LIMSResponse
    {
        public List<LIMSResponse_Errors> errors { get; set; }
        public LIMSResponse_Data data { get; set; }
    }//LIMS_Workflow 

    public class LIMSResponse_Data
    {
        public LIMSResponse_Data_Step step { get; set; }
        public LIMSResponse_Data_Workflow workflow { get; set; }
    }//LIMS_Workflow 

    public class LIMSResponse_Data_Step
    {
        public string completed_at { get; set; }
        public string initialized_at { get; set; }
        public string name { get; set; }
    }//LIMS_Workflow 

    public class LIMSResponse_Data_Workflow
    {
        public string status { get; set; }
        public string unique_id { get; set; }
    }//LIMS_Workflow 

    public class LIMSResponse_Errors
    {
        public string field { get; set; }
        public object error_class { get; set; }
        public string message { get; set; }
    }//LIMS_Workflow 

    ////////////////////////////////////////////////////////////////////////

    public class StepPayload_Inputs_InitExtractionStep2
    {
        public string unp_plate { get; set; }
        public string automation_system { get; set; }
        public string pq_plate { get; set; }
    }//LIMS_Workflow 

    public class StepPayload_Inputs_InitExtractionStep2_V2
    {
        public string automation_system { get; set; }
        public string rack_id { get; set; }
        public string pq_plate { get; set; }
        public StepPayload_Inputs_TubeMapFile tube_map_file { get; set; }
    }//LIMS_Workflow 

    public class StepPayload_Inputs_CompleteExtractionStep2
    {
        public string quant_metric { get; set; }
    }//LIMS_Workflow 

    public class StepPayload_Inputs_InitExtractionStep3
    {
        public string unp_plate { get; set; }
        public string stamped_unp_1_id { get; set; }
        public string stamped_unp_2_id { get; set; }
    }//LIMS_Workflow 

    public class StepPayload_Inputs_InitLibraryStep1
    {
        public string shearing_instrument { get; set; }
        public string covaris_plate { get; set; }
        public string unp { get; set; }
    }//LIMS_Workflow 

    public class StepPayload_Inputs_InitLibraryStep2
    {
        public string covaris_recovery_instrument { get; set; }
        public string covaris_plate { get; set; }
        public string sheared_dna_plate { get; set; }
    }//LIMS_Workflow 

    ///////////////////////////////////////////////////////////////////////

    public class GetQuantMetric
    {
        public string drop_quant_data { get; set; }
        public string report_name { get; set; }
        public string plate_id { get; set; }
        public string assay_id { get; set; }
        public string lab_workflow_id { get; set; }
    }//LIMS_Workflow 

    public class GetQuantMetricResponse
    {
        public List<LIMSResponse_Errors> errors { get; set; }
        public GetQuantMetricResponse_Data data { get; set; } ///Why does this have to be an Object?????
    }//LIMS_Workflow 

    public class GetQuantMetricResponse_Data
    {
        public string quant_metric { get; set; }
    }//LIMS_Workflow 

    ////////////////////////////////////////////////////////////////////////

    public class WorkflowPayload
    {
        public string request_id { get; set; }
        public string name { get; set; }
        public string spec_name { get; set; }
        public object inputs { get; set; }
    }//LIMS_Workflow 

    public class WFQueryResponse
    {
        public List<LIMSResponse_Errors> errors { get; set; }
        public List<WFQueryResponse_Data> data { get; set; }
    }//LIMS_Workflow 

    public class WFQueryResponse_Data
    {
        public string spec_name { get; set; }
        public string unique_id { get; set; }
        public string assay { get; set; }
    }//LIMS_Workflow 

    /////////////////////////////////////////////////////////////////////

    public class LibraryTasksQueryResponse
    {
        public List<LIMSResponse_Errors> errors { get; set; }
        public List<LibraryTasksQueryResponse_Data> data { get; set; }
    }//LIMS_Workflow 

    public class LibraryTasksQueryResponse_Data
    {
        public List<string> parent_workflow_ids { get; set; }
        public string sent_at { get; set; }
        public string request_id { get; set; }
        public List<LibraryTasksQueryResponse_Data_CurrentSpecVersions> current_spec_versions { get; set; }
        public LibraryTasksQueryResponse_Data_Arguments arguments { get; set; }
        public string spec_type { get; set; }
    }//LIMS_Workflow 

    public class LibraryTasksQueryResponse_Data_CurrentSpecVersions
    {
        public string versioned_name { get; set; }
        public string display_name { get; set; }
    }//LIMS_Workflow 

    public class LibraryTasksQueryResponse_Data_Arguments
    {
        public string assay_id { get; set; }
        public string uses { get; set; }
        public string protocol_type { get; set; }
        public string unp_id { get; set; }
        public string contains_priority { get; set; }
    }//LIMS_Workflow 

    #endregion

    #region 'LABORATORY OBJECTS'

    public class NormShear_RunInfo
    {
        public string automation_system { get; set; }
        public string module { get; set; }
        public List<NormShear_Plates> normshear_plates { get; set; }
    }//RunInfo

    public class NormShear_Plates
    {
        public string process_type { get; set; }                //run either stamping or shearing on robot
        public string extraction_workflow { get; set; }
        public string library_workflow { get; set; }
        public string assay_id { get; set; }
        public string timestamp_enter { get; set; }
        public string timestamp_exit { get; set; }
        public string pq_id { get; set; }
        public string unp_id { get; set; }
        public string quant_metric { get; set; }
        public string covaris_id { get; set; }
        public string shearing_instrument { get; set; }
        public string gdna_norm_dq1_plate_id { get; set; }
        public string gdna_norm_dq2_plate_id { get; set; }
        public string stamped_unp_1_id { get; set; }
        public string stamped_unp_2_id { get; set; }
        public string combined_trinean_file_name { get; set; }
        public string sheared_dna_id { get; set; }
        public string lib_request_id { get; set; }              //from get library prep tasks step, for Init step
        public string lib_spec_type { get; set; }               //from get library prep tasks step, for Init step
        public string lib_versioned_name { get; set; }          //from get library prep tasks step, for Init step
        public string status { get; set; }
    }//RunInfo

    #endregion
}
