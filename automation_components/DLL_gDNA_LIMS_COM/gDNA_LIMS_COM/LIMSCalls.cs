using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Newtonsoft.Json;

namespace gDNA_LIMS_COM
{
    public static class LIMSCalls
    {
        private class ListOfIB
        {
            public List<string> specimen_ids { get; set; }
        }
        private class SpecimenResponse_data
        {
            public string result { get; set; }
            public List<string> invalid_specimens { get; set; }
        }
        private class SpecimenResponse
        {
            public SpecimenResponse_data data { get; set; }
            public List<DLL_LIMS_Workflow.LIMSResponse_Errors> errors { get; set; }
        }
        /*
        public static void validateIBS(string LIMSEndpoint, string systemFolderPath, string runID, string[] myIBs, out string[] invalidIBs)
        {
            JavaScriptSerializer ser = new JavaScriptSerializer();
            string runOutputFolder = systemFolderPath+"\\RunoutputFiles\\";
            ListOfIB myPayload = new ListOfIB();
            myPayload.specimen_ids = myIBs.ToList();
            string apiAddress = "https://" + LIMSEndpoint + "/api/lab_workflow/v2/tasks/?spec_type=assayless%20star%20extraction%20batch%20selection";
            SpecimenResponse responseObj = ser.Deserialize<SpecimenResponse>(DLL_Http.Http.SendJSONToAPI(ser.Serialize(myPayload), runID, runOutputFolder, apiAddress, "weblims", "206b5ebc-ce82-4bd0-ae9d-b0b943b5fa1a", 120000, "Validate Sample Tube Barcodes For Extraction"));
            invalidIBs = responseObj.data.invalid_specimens.ToArray();
                if (invalidIBs == null)
                {
                invalidIBs = new string[1] { "0" };
                }
        }

        public static void validateAdditionalSpecimens(string LIMSEndpoint, string systemFolderPath, string runID, string[] myIBs, out string[] invalidIBs)
        {
            JavaScriptSerializer ser = new JavaScriptSerializer();
            string runOutputFolder = systemFolderPath + "\\RunoutputFiles\\";
            ListOfIB myPayload = new ListOfIB();
            myPayload.specimen_ids = myIBs.ToList();
            string apiAddress = "https://" + LIMSEndpoint + "/api/lab_workflow/v2/check-additional-specimens-for-extraction";
            SpecimenResponse responseObj = ser.Deserialize<SpecimenResponse>(DLL_Http.Http.SendJSONToAPI(ser.Serialize(myPayload), runID, runOutputFolder, apiAddress, "weblims", "206b5ebc-ce82-4bd0-ae9d-b0b943b5fa1a", 120000, "Validate Additional Tube Barcodes For Extraction"));
            invalidIBs = responseObj.data.invalid_specimens.ToArray();
        }
        public static void validateControls(string LIMSEndpoint, string systemFolderPath, string runID, string[] myIBs, out string[] invalidIBs)
        {
            JavaScriptSerializer ser = new JavaScriptSerializer();
            string runOutputFolder = systemFolderPath + "\\RunoutputFiles\\";
            ListOfIB myPayload = new ListOfIB();
            myPayload.specimen_ids = myIBs.ToList();
            string apiAddress = "https://" + LIMSEndpoint + "/api/lab_workflow/v2/controls";
            SpecimenResponse responseObj = ser.Deserialize<SpecimenResponse>(DLL_Http.Http.SendJSONToAPI(ser.Serialize(myPayload), runID, runOutputFolder, apiAddress, "weblims", "206b5ebc-ce82-4bd0-ae9d-b0b943b5fa1a", 120000, "Validate Control Tube Barcodes For Extraction"));
            invalidIBs = responseObj.data.invalid_specimens.ToArray();
        }
        */
        public static void initializeExtractionBatch(string LIMSEndpoint, string systemFolderPath, string runID, string barcode1, string barcode2)
        {
            string runOutputFolder = systemFolderPath + "\\RunOutputFiles\\";
            List<string> myTubeBarcodes = new List<string>();
            List<string> controls = new List<string>();
            List<string> additionalSpecimens = new List<string>();
            List<string> myRequestIDs = new List<string>();

            myTubeBarcodes.Add(barcode1);
            myTubeBarcodes.Add(barcode2);
            //Prepare data?
            //Get pending extraction batching tasks
            try
            {
                string apiAddress = "https://" + LIMSEndpoint + "/api/lab_workflow/v1/tasks/?spec_type=RNA Extraction";
                string LIMSReturnText = DLL_Http.Http.HttpGet(apiAddress, runID, runOutputFolder, "msg", "5213155e-87af-4db1-96f7-47fd89f96c57", 120000, "Get Pending Extraction Batch Selction Tasks");
                DLL_LIMS_Workflow.TasksQueryResponseV2 responseObj = JsonConvert.DeserializeObject<DLL_LIMS_Workflow.TasksQueryResponseV2>(LIMSReturnText);
                DLL_LIMS_Workflow.LIMS_Workflow.ReadErrorsInResponse(responseObj.errors);
                
                int found = 0;
                foreach (DLL_LIMS_Workflow.TasksQueryResponse_DataV2 myData in responseObj.data)
                {
                    Dictionary<string, object> myDict = (Dictionary<string, object>)myData.arguments;
                    if (myTubeBarcodes.Contains(myDict["specimen_id"].ToString()))
                    {
                        found++;
                        myRequestIDs.Add(myData.request_id);
                    }
                }
                if (found < myTubeBarcodes.Count())
                {
                    throw new Exception("All pending extraction batching tasks not yet available in LIMS");
                }
            }
            catch (Exception ex)
            { 
                throw new Exception("Error getting extraction batching tasks: " + ex.Message);
            }
        }
            

            //Initialize extraction batching workflow
            //Return worklist, data?
    }
        //public static void initializeExtraction()
        //{
            //Prepare data?
            //Get Pending Extraction tasks (request ID)
            //Init extraction workflow
            //Init extraction step 1
        //}

        //public static void completeExtraction()
        //{
            //Complete extraction step 1
        //}
    
}
