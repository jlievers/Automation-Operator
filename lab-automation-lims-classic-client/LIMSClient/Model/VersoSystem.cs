using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com.Invitae.LabAutomation.Model
{
    class VersoSystem
    {
        
    }

    public class RosalindWorklistResponse//LIMS_Workflow 
    {
        public List<LIMSResponse_Errors> errors { get; set; }
        public Device_XL20 data { get; set; }
    }

    //RunInfo Objects

    public class RunInfo_VersoSystem
    {
        public string automation_system { get; set; }
        public List<OperationData_Norm> operation_data_norm { get; set; }
        public List<OperationData_Stamp> operation_data_stamp { get; set; }
    }//RunInfo

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
        public string status { get; set; }
        public string assay_plate_id { get; set; }
        public string stamped_unp_1_id { get; set; }

    }//RunInfo

    public class OperationData_Stamp
    {
        public string assay_plate_id { get; set; }
        public string retrieve_and_stamp_workflow { get; set; }
        public string tube_rack_id { get; set; }
        public string scanfile_name { get; set; }
        public string spec_name { get; set; }                
        public string assay_id { get; set; }
        public string timestamp_enter { get; set; }
        public string timestamp_exit { get; set; }
        public string status { get; set; }
        public string stamped_unp_1_id { get; set; }
    }//RunInfo

    //////Rack Scan Objects/////////////////////////////

    public class TubeScan
    {
        public string rack_barcode { get; set; }
        public List<ScannedTube> scanned_tubes { get; set; }
    }//DLL_Instruments

    public class ScannedTube
    {
        public string barcode { get; set; }
        public int row { get; set; }
        public int column { get; set; }
        public int status { get; set; }
    }//DLL_Instruments

    ///////// WorkFlow Objects////////////////////////////////////////////

    public class StepPayload_Inputs_InitRetrieveAndStampStep2
    {
        public string automation_system { get; set; }
        public string assay_plate_id { get; set; }
        public StepPayload_Inputs_TubeMapFile tube_map_file { get; set; }
    } //LIMS_Workflow 
    
    public class VersoSystem_Inventory
    {
        public string automation_system { get; set; }
        public List<Device_StaticStacker> stackers { get; set; }
        public Device_Verso verso { get; set; }
        public List<Device_Centrifuge> centrifuges { get; set; }
    }//OUTDATED. USE DLL_IInventory

    public class Device_StaticStacker
    {
        public bool enabled { get; set; }
        public string device_name { get; set; }
        public int num_nests { get; set; }
        public List<Nest> nests { get; set; }
    }//OUTDATED. USE DLL_IInventory

    public class Device_Covaris
    {
        public bool enabled { get; set; }
        public string device_name { get; set; }
        public Nest nest { get; set; }
    }//OUTDATED. USE DLL_IInventory

    public class Device_TransportRail
    {
        public bool enabled { get; set; }
        public string device_name { get; set; }
        public string companion_system { get; set; }
        public Nest nest { get; set; }
    }//OUTDATED. USE DLL_IInventory

    public class Device_Verso
    {
        public bool enabled { get; set; }
        public string device_name { get; set; }
        public Nest nest { get; set; }
    }//OUTDATED. USE DLL_IInventory

    public class Device_Centrifuge
    {
        public bool enabled { get; set; }
        public string device_name { get; set; }
        public Nest nest { get; set; }
    }//OUTDATED. USE DLL_IInventory


    #region XL20

    public class XL20_Inventory
    {
        public string automation_system { get; set; }
        public List<Device_XL20> xl_20s { get; set; }
    }//DLL_Instruments

    public class Device_XL20
    {
        public bool enabled { get; set; }
        public OperatingMode operating_mode { get; set; }
        public TubeInventory inventory { get; set; }
        public List<OneMove> tube_moves { get; set; }
    }//DLL_Instruments

    public class OperatingMode
    {
        public string name { get; set; }
        public List<string> batch { get; set; }
        public List<string> collect { get; set; }
    }//DLL_Instruments

    public class TubeInventory
    {
        public string location_name { get; set; }
        public List<Rack> racks { get; set; }
    }//DLL_Instruments

    public class Rack
    {
        public string barcode { get; set; }
        public int position { get; set; }
        public string purpose { get; set; }
        public string assay { get; set; }
        public bool complete { get; set; }
        public List<Tube> tubes { get; set; }
    }//DLL_Instruments

    public class Tube
    {
        public string barcode { get; set; }
        public int row { get; set; }
        public int column { get; set; }
    }//DLL_Instruments

    //worklist
    public class OneMove
    {
        public string tube_barcode { get; set; }
        public Motion source { get; set; }
        public Motion destination { get; set; }
    }//DLL_Instruments

    public class Motion
    {
        public string rack_barcode { get; set; }
        public string rack_position { get; set; }
        public string tube_row { get; set; }
        public string tube_column { get; set; }
    }//DLL_Instruments

    #endregion

}
