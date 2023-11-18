using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLL_RunOutput
{
    public static class NormShear
    {
    }

    public class Shearing_RunInfo
    {
        public string automation_system { get; set; }
        public string module { get; set; }
        public List<OperationData_Shearing> normshear_plates { get; set; }
    }

    public class OperationData_Shearing
    {
       //public string process_type { get; set; }                //run either stamping or shearing on robot. Still necessary??
        public string shearing_workflow { get; set; }
        public string library_workflow { get; set; }            //This will break current Shearbot system was: 'workflow'
        public string assay_plate_id { get; set; }
        public string covaris_plate { get; set; }                  //Covaris plate barcode
        public string shearing_instrument { get; set; }         //LM#

        public string request_id { get; set; }                  //shearing get tasks step -dont need if combine scripts
        public string spec_type { get; set; }                   //shearing get tasks step -dont need if combine scripts
        public string versioned_name { get; set; }              //shearing get tasks step -dont need if combine scripts

        //lib Batch selection
        public string sheared_dna_id { get; set; }             
        public string batch_selection_workflow { get; set; }
        public string batch_selection_plate_format { get; set; }
        public string batched_plate_upper_left { get; set; }  
        public string batched_plate_upper_right { get; set; }
        public string batched_plate_lower_left { get; set; }
        public string batched_plate_lower_right { get; set; }

        public string timestamp_enter { get; set; }
        public string timestamp_exit { get; set; }
        public string status { get; set; }

        //public string pq_id { get; set; }                       //DELETE
        //public string unp_id { get; set; }                      //DELETE
        //public string quant_metric { get; set; }                //DELETE
        //public string gdna_norm_dq1_plate_id { get; set; }      //DELETE
        //public string gdna_norm_dq2_plate_id { get; set; }      //DELETE
        //public string stamped_unp_1_id { get; set; }            //DELETE
        //public string stamped_unp_2_id { get; set; }            //DELETE
        //public string combined_trinean_file_name { get; set; }  //DELETE

    }
}
