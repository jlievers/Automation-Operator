﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLL_RunOutput
{
    class Dart_PostProcesses
    {
    }

    public class RunInfo_DartPostProcesses
    {
        //public string automation_system { get; set; }
        public List<OperationData_DartPostProcesses> operation_data_dart_post_processes { get; set; }
    }

    public class OperationData_DartPostProcesses
    {
        public string workflow_id { get; set; }
        public int reagent_set_index { get; set; }
        public string plate_group_name { get; set; }
        public string post_pcr_plate_id { get; set; }
        public string exo1_plate { get; set; }
        public string lambda_exo1_plate { get; set; }
        public string cleanup_elution_plate { get; set; }
        public string cleanup_peg_plate { get; set; }
        public string cleanup_ethanol_plate { get; set; }
        public string cleanup_lte_plate { get; set; }
        public string post_pcr_quant_dq_plate_id1 { get; set; }
        public string post_pcr_quant_dq_plate_id2 { get; set; }
        public string norm_hyb_plate1 { get; set; }
        public string norm_hyb_plate2 { get; set; }
        public string current_odtc_program { get; set; }
        public string status { get; set; } //active or complete
        public string timestamp_enter { get; set; }
        public string timestamp_exit { get; set; }
    }

    public class RunInfo_ArrayProcessing
    {
        //public string automation_system { get; set; }
        public List<OperationData_DartArrayProcessing> operation_data_dart_array_processing { get; set; }
    }

    public class OperationData_DartArrayProcessing
    {
        public string workflow_id { get; set; }
        public string plate_group_name { get; set; }
        public string hybwash_plate { get; set; }
        public string sister_plate { get; set; }
        public List<string> chips { get; set; }
        public string status { get; set; } //active or complete
        public string timestamp_enter { get; set; }
        public string timestamp_exit { get; set; }
    }
}
