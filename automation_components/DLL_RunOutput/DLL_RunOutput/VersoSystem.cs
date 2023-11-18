using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLL_RunOutput
{
    public static class VersoSystem
    {
    }

    public class RunInfo_VersoSystem
    {
        public string automation_system { get; set; }
        public List<OperationData_Norm> operation_data_norm { get; set; }
        public List<OperationData_Stamp> operation_data_stamp { get; set; }
    }

    public class OperationData_Norm
    {
        public string pq_id { get; set; }
        public string tube_rack_id { get; set; }
        public string scanfile_name { get; set; }
        public string extraction_workflow { get; set; }
        public string assay_id { get; set; }
        public string process_type { get; set; }// This is for "AndStamping"
        public string timestamp_enter { get; set; }
        public string timestamp_exit { get; set; }
        public string quant_metric { get; set; }
        public string gdna_norm_dq1_plate_id { get; set; }
        public string gdna_norm_dq2_plate_id { get; set; }
        public string combined_trinean_file_name { get; set; }
        public string post_extraction_norm_worklist_filename { get; set; }
        public string status { get; set; }
        public string assay_plate_id { get; set; }
        public string stamped_unp_1_id { get; set; }

    }

    public class OperationData_Stamp
    {
        public string verso_pickjob_id { get; set; }//verso stamp only
        public string task_batch_id { get; set; }//retrieve and stamp task_id
        public string tube_rack_id { get; set; }
        public string assay_id { get; set; }
        public string assay_plate_id { get; set; }
        public string batch_selection_workflow { get; set; }//XL20 stamp only
        public string retrieve_and_stamp_workflow { get; set; }
        public string scanfile_name { get; set; }
        public string spec_name { get; set; }//WTF
        public string stamped_unp_1_id { get; set; }//For FMR1plate
        public string retrieve_and_stamp_request_id { get; set; }
        public string retrieve_and_stamp_spec_type { get; set; }
        public string retrieve_and_stamp_versioned_name { get; set; }
        public string dilution_worklist_name { get; set; }
        public string timestamp_enter { get; set; }
        public string timestamp_exit { get; set; }
        public string purpose { get; set; }
        public string status { get; set; }
    }
}
