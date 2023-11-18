using System.Collections.Generic;

namespace Com.Invitae.LabAutomation
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
        
    }
}
