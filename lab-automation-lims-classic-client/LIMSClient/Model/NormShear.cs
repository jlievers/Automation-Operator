using System.Collections.Generic;

namespace Com.Invitae.LabAutomation.Model
{
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

}
