using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Data;
using Com.Invitae.LabAutomation.Model;
using Newtonsoft.Json;

namespace Com.Invitae.LabAutomation.Client
{
    public static class LIMSCalls
    {
        public static void InitializeExtractionBatch(string LIMSEndpoint, string systemFolderPath, string runID,
            string barcode1, string barcode2)
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
                string LIMSReturnText = HttpWrapper.HttpGet(apiAddress, runID, runOutputFolder, "msg",
                    "5213155e-87af-4db1-96f7-47fd89f96c57", 120000, "Get Pending Extraction Batch Selction Tasks");
                TasksQueryResponseV2 responseObj =
                    JsonConvert.DeserializeObject<TasksQueryResponseV2>(LIMSReturnText);
                LIMS_Workflow.ReadErrorsInResponse(responseObj.errors);

                int found = 0;
                foreach (TasksQueryResponse_DataV2 myData in responseObj.data)
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

        private static string InitExtractionBatching_backup(string LIMSEndpoint, string SystemFolderPath, string RunID, string tubeRackID, string LMNumber, string dataTable)
        {
            string runOutputFolder = SystemFolderPath + "\\RunOutputFiles\\";
            List<string> myTubeBarcodes = new List<string>();
            List<string> controls = new List<string>();
            List<string> fillers = new List<string>();

            string datafile = "";
            try
            {
                //Get tube lists, combine, upload combined data file(source_id,rack_id,tube_id,well,volume)" 
                DataTable myScan = ReadCSV(SystemFolderPath + "\\SampleScanFiles\\" + tubeRackID + ".csv");
                DataTable myWorklist = ReadCSV2(SystemFolderPath + "\\DataTables\\" + dataTable + ".csv");
                foreach (DataRow oneTableRow in myWorklist.Rows)
                {
                    switch (oneTableRow["source_type"].ToString())
                    {
                        case "sample":
                            myTubeBarcodes.Add(oneTableRow["source_id"].ToString());
                            break;
                        case "control":
                            controls.Add(oneTableRow["source_id"].ToString());
                            break;
                        case "filler":
                            fillers.Add(oneTableRow["source_id"].ToString());
                            break;
                    }
                    foreach (DataRow oneScanRow in myScan.Rows)
                    {
                        string myWell = oneTableRow["well"].ToString();
                        if (myWell.Length < 3)
                        {
                            myWell = myWell.Insert(1, "0");
                        }
                        if (oneScanRow["TUBE"].ToString() == myWell)
                        {
                            oneTableRow["tube_id"] = oneScanRow["BARCODE"].ToString();
                        }
                    }
                }
                string myFile = DataTableToCommaDelimitedString(myWorklist);
                File.WriteAllText(SystemFolderPath + "\\DataTables\\" + dataTable + "_ExtractionTubeMap.csv", myFile);
                DatafileUploadPayload myPayload = new DatafileUploadPayload();
                myPayload.file_name = dataTable + "_Extraction_Tube_map.csv";
                myPayload.file_data = myFile;

                string apiAddress = "https://" + LIMSEndpoint + "/api/lab_workflow/v1/upload-datafile/";
                DatafileUploadResponse responseObj = JsonConvert.DeserializeObject<DatafileUploadResponse>(
                    HttpWrapper.SendJSONToAPI(JsonConvert.SerializeObject(myPayload), RunID, runOutputFolder, apiAddress, "msg",
                        "5213155e-87af-4db1-96f7-47fd89f96c57", 120000, "Upload Extraction Tube Map"));
                LIMS_Workflow.ReadErrorsInResponse(responseObj.errors);
                datafile = responseObj.data.data_file;
            }
            catch (Exception ex)
            {
                throw new Exception("Init Extraction error uploading tube data file: " + ex.Message);
            }

            List<string> myRequestIds = new List<string>();
            try
            {
                //Get pending extraction batching tasks
                string apiAddress = "https://" + LIMSEndpoint + "/api/lab_workflow/v1/tasks/?spec_type=RNA Extraction Batch Selection";
                string LIMSReturnText = HttpWrapper.HttpGet(apiAddress, RunID, runOutputFolder, "msg", "5213155e-87af-4db1-96f7-47fd89f96c57", 120000, "Get Pending Extraction Batch Selction Tasks");
                TasksQueryResponseV2 responseObj = JsonConvert.DeserializeObject<TasksQueryResponseV2>(LIMSReturnText);
                LIMS_Workflow.ReadErrorsInResponse(responseObj.errors);

                int found = 0;
                foreach (TasksQueryResponse_DataV2 myData in responseObj.data)
                {
                    Dictionary<string, object> myDict = (Dictionary<string, object>)myData.arguments;
                    if (myTubeBarcodes.Contains(myDict["specimen_id"].ToString()))
                    {
                        found++;
                        myRequestIds.Add(myData.request_id);
                    }
                }
                if (found < myTubeBarcodes.Count())
                {
                    throw new Exception("All pending extraction batching tasks not yet available in LIMS");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Init Extraction Error getting extraction batching tasks: " + ex.Message);
            }
            string extraction_batching_workflow = "";
            try
            {
                //Init extraction batching workflow
                WorkflowPayload myPayload = new WorkflowPayload();
                WorkflowPayload_Inputs_InitRNAExtractionBatchSel myInput = new WorkflowPayload_Inputs_InitRNAExtractionBatchSel();
                myInput.assay_id = "AY65";
                myInput.request_ids = myRequestIds;
                myInput.control_ids = controls;
                myInput.filler_ids = fillers;
                myPayload.name = "RNA Extraction Batch Selection";
                myPayload.spec_name = "rna_extraction_batch_selection_v1.0";
                myPayload.inputs = myInput;

                string apiAddress = "https://" + LIMSEndpoint + "/api/lab_workflow/v1/initialize-batch-selection/";
                LIMSResponse responseObj = JsonConvert.DeserializeObject<LIMSResponse>(HttpWrapper.SendJSONToAPI(JsonConvert.SerializeObject(myPayload), RunID, runOutputFolder, apiAddress, "msg", "5213155e-87af-4db1-96f7-47fd89f96c57", 120000, "Initialize Extraction Batch Selection Workflow"));
                LIMS_Workflow.ReadErrorsInResponse(responseObj.errors);
                extraction_batching_workflow = responseObj.data.workflow.unique_id;
            }
            catch (Exception ex)
            {
                throw new Exception("Init Extraction Error initializing extraction batching workflow: " + ex.Message);
            }

            string request_id = "";
            try
            {
                //Get pending extraction tasks 
                string apiAddress = "https://" + LIMSEndpoint + "/api/lab_workflow/v1/tasks/?spec_type=RNA Extraction";
                TasksQueryResponseV2 responseObj = JsonConvert.DeserializeObject<TasksQueryResponseV2>(HttpWrapper.HttpGet(apiAddress, RunID, runOutputFolder, "msg", "5213155e-87af-4db1-96f7-47fd89f96c57", 120000, "Get Pending Extraction Tasks"));
                LIMS_Workflow.ReadErrorsInResponse(responseObj.errors);
                foreach (TasksQueryResponse_DataV2 myData in responseObj.data)
                {
                    if (myData.parent_workflow_ids.Contains(extraction_batching_workflow))
                    {
                        request_id = myData.request_id;
                    }
                }
                if (request_id == "")
                {
                    throw new Exception("Pending extraction task not yet available in LIMS");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Init Extraction Error getting pending extraction tasks: " + ex.Message);
            }

            string extraction_workflow = "";
            try
            {
                //Init extraction workflow
                WorkflowPayload myPayload = new WorkflowPayload();
                myPayload.name = "RNA Extraction";
                myPayload.spec_name = "rna_extraction_v1.0";
                myPayload.request_id = request_id;

                string apiAddress = "https://" + LIMSEndpoint + "/api/lab_workflow/v1/initialize-workflow/";
                LIMSResponse responseObj = JsonConvert.DeserializeObject<LIMSResponse>(HttpWrapper.SendJSONToAPI(JsonConvert.SerializeObject(myPayload), RunID, runOutputFolder, apiAddress, "msg", "5213155e-87af-4db1-96f7-47fd89f96c57", 360000, "Initialize Extraction Workflow"));
                LIMS_Workflow.ReadErrorsInResponse(responseObj.errors);
                extraction_workflow = responseObj.data.workflow.unique_id;
            }
            catch (Exception ex)
            {
                throw new Exception("Init Extraction Error initializing extraction workflow: " + ex.Message);
            }

            try
            {
                //Init extraction step 1 
                StepPayload myPayload = new StepPayload();
                StepPayload_Inputs_InitExtractionStep1 myInput = new StepPayload_Inputs_InitExtractionStep1();
                myInput.machine = LMNumber;
                myInput.rack_id = tubeRackID;
                myInput.tube_map_file = datafile;
                myPayload.inputs = myInput;
                myPayload.workflow = extraction_workflow;
                myPayload.step_name = "RNA Extraction";
                myPayload.user = "extraction@invitae.com";

                string apiAddress = "https://" + LIMSEndpoint + "/api/lab_workflow/v1/initialize-step/";
                LIMSResponse responseObj = JsonConvert.DeserializeObject<LIMSResponse>(HttpWrapper.SendJSONToAPI(JsonConvert.SerializeObject(myPayload), RunID, runOutputFolder, apiAddress, "msg", "5213155e-87af-4db1-96f7-47fd89f96c57", 120000, "Initialize Extraction Step: RNA Extraction"));
                LIMS_Workflow.ReadErrorsInResponse(responseObj.errors);
            }
            catch (Exception ex)
            {
                throw new Exception("Init Extraction Error initializing extraction step 1: " + ex.Message);
            }
            return extraction_workflow;
        }

        public static void voidvalidateIBs(string LIMSEndpoint, string SystemFolderPath, string RunID, string[] myIBs, out string[] invalidIBs)
        {
            string runOutputFolder = SystemFolderPath + "\\RunOutputFiles\\";
            ListOfIB myPayload = new ListOfIB();
            myPayload.specimen_ids = myIBs.ToList();
            string apiAddress = "https://" + LIMSEndpoint + "/api/verify_rna_extraction_batch_tasks_exist_for_specimens/";
            SpecimenResponse responseObj = JsonConvert.DeserializeObject<SpecimenResponse>(HttpWrapper.SendJSONToAPI(JsonConvert.SerializeObject(myPayload), RunID, runOutputFolder, apiAddress, "weblims", "206b5ebc-ce82-4bd0-ae9d-b0b943b5fa1a", 120000, "Validate Tube Barcodes For Extraction"));
            invalidIBs =  responseObj.data.invalid_specimens.ToArray();

        }

        public static string InitExtractionBatching(string LIMSEndpoint, string SystemFolderPath, string RunID, string tubeRackID, string LMNumber, string dataTable)
        {
            string runOutputFolder = SystemFolderPath + "\\RunOutputFiles\\";
            List<string> myTubeBarcodes = new List<string>();
            List<string> controls = new List<string>();
            List<string> fillers = new List<string>();
            List<string> myRequestIds = new List<string>();

            try
            {
                DataTable myWorklist = ReadCSV2(SystemFolderPath + "\\DataTables\\" + dataTable + ".csv");
                foreach (DataRow oneTableRow in myWorklist.Rows)
                {
                    switch (oneTableRow["source_type"].ToString())
                    {
                        case "sample":
                            myTubeBarcodes.Add(oneTableRow["source_id"].ToString());
                            break;
                        case "control":
                            controls.Add(oneTableRow["source_id"].ToString());
                            break;
                        case "filler":
                            fillers.Add(oneTableRow["source_id"].ToString());
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error parsing datatable: " + ex.Message);
            }

            try
            {
                //Get pending extraction batching tasks
                string apiAddress = "https://" + LIMSEndpoint + "/api/lab_workflow/v1/tasks/?spec_type=RNA Extraction Batch Selection";
                string LIMSReturnText = HttpWrapper.HttpGet(apiAddress, RunID, runOutputFolder, "msg", "5213155e-87af-4db1-96f7-47fd89f96c57", 120000, "Get Pending Extraction Batch Selction Tasks");
                TasksQueryResponseV2 responseObj = JsonConvert.DeserializeObject<TasksQueryResponseV2>(LIMSReturnText);
                LIMS_Workflow.ReadErrorsInResponse(responseObj.errors);

                int found = 0;
                foreach (TasksQueryResponse_DataV2 myData in responseObj.data)
                {
                    Dictionary<string, object> myDict = (Dictionary<string, object>)myData.arguments;
                    if (myTubeBarcodes.Contains(myDict["specimen_id"].ToString()))
                    {
                        found++;
                        myRequestIds.Add(myData.request_id);
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
            string extraction_batching_workflow = "";
            try
            {
                //Init extraction batching workflow
                WorkflowPayload myPayload = new WorkflowPayload();
                WorkflowPayload_Inputs_InitRNAExtractionBatchSel myInput = new WorkflowPayload_Inputs_InitRNAExtractionBatchSel();
                myInput.assay_id = "AY65";
                myInput.request_ids = myRequestIds;
                myInput.control_ids = controls;
                myInput.filler_ids = fillers;
                myPayload.name = "RNA Extraction Batch Selection";
                myPayload.spec_name = "rna_extraction_batch_selection_v1.0";
                myPayload.inputs = myInput;
                
                string apiAddress = "https://" + LIMSEndpoint + "/api/lab_workflow/v1/initialize-batch-selection/";
                LIMSResponse responseObj = JsonConvert.DeserializeObject<LIMSResponse>(HttpWrapper.SendJSONToAPI(JsonConvert.SerializeObject(myPayload), RunID, runOutputFolder, apiAddress, "msg", "5213155e-87af-4db1-96f7-47fd89f96c57", 120000, "Initialize Extraction Batch Selection Workflow"));
                LIMS_Workflow.ReadErrorsInResponse(responseObj.errors);
                extraction_batching_workflow = responseObj.data.workflow.unique_id;
            }
            catch (Exception ex)
            {
                throw new Exception("Init Extraction Error initializing extraction batching workflow: " + ex.Message);
            }
            return extraction_batching_workflow;
        }

        public static string InitExtraction(string LIMSEndpoint, string SystemFolderPath, string RunID, string batchingWorkflowID, string tubeRackID, string LMNumber, string dataTable, string extraction_workflow, string nuclease_free_ethanol, string dnasei_mix, string rbb_buffer, string bbd_buffer, string isopropanol, string wbe_buffer, string ibf_buffer, string proK, string nuclease_free_water)
        {
            string runOutputFolder = SystemFolderPath + "\\RunOutputFiles\\";
            List<string> myTubeBarcodes = new List<string>();
            List<string> controls = new List<string>();
            List<string> fillers = new List<string>();

            string datafile = "";
            try
            {
                //Get tube lists, combine, upload combined data file(source_id,rack_id,tube_id,well,volume)" 
                DataTable myScan = ReadCSV(SystemFolderPath + "\\SampleScanFiles\\" + tubeRackID + ".csv");
                DataTable myWorklist = ReadCSV2(SystemFolderPath + "\\DataTables\\" + dataTable + ".csv");
                foreach (DataRow oneTableRow in myWorklist.Rows)
                {
                    switch (oneTableRow["source_type"].ToString())
                    {
                        case "sample":
                            myTubeBarcodes.Add(oneTableRow["source_id"].ToString());
                            break;
                        case "control":
                            controls.Add(oneTableRow["source_id"].ToString());
                            break;
                        case "filler":
                            fillers.Add(oneTableRow["source_id"].ToString());
                            break;
                    }
                    foreach (DataRow oneScanRow in myScan.Rows)
                    {
                        string myWell = oneTableRow["well"].ToString();
                        if (myWell.Length < 3)
                        {
                            myWell = myWell.Insert(1, "0");
                        }
                        if (oneScanRow["TUBE"].ToString() == myWell)
                        {
                            oneTableRow["tube_id"] = oneScanRow["BARCODE"].ToString();
                        }
                    }
                }
                string myFile = DataTableToCommaDelimitedString(myWorklist);
                File.WriteAllText(SystemFolderPath + "\\DataTables\\" + dataTable + "_ExtractionTubeMap.csv", myFile);
                DatafileUploadPayload myPayload = new DatafileUploadPayload();
                myPayload.file_name = dataTable + "_Extraction_Tube_map.csv";
                myPayload.file_data = myFile;

                string apiAddress = "https://" + LIMSEndpoint + "/api/lab_workflow/v1/upload-datafile/";
                DatafileUploadResponse responseObj = JsonConvert.DeserializeObject<DatafileUploadResponse>(HttpWrapper.SendJSONToAPI(JsonConvert.SerializeObject(myPayload), RunID, runOutputFolder, apiAddress, "msg", "5213155e-87af-4db1-96f7-47fd89f96c57", 120000, "Upload Extraction Tube Map"));
                LIMS_Workflow.ReadErrorsInResponse(responseObj.errors);
                datafile = responseObj.data.data_file;
            }
            catch (Exception ex)
            {
                throw new Exception("Init Extraction error uploading tube data file: " + ex.Message);
            }

            string request_id = "";
            try
            {
                //Get pending extraction tasks 
                string apiAddress = "https://" + LIMSEndpoint + "/api/lab_workflow/v1/tasks/?spec_type=RNA Extraction";
                TasksQueryResponseV2 responseObj = JsonConvert.DeserializeObject<TasksQueryResponseV2>(HttpWrapper.HttpGet(apiAddress, RunID, runOutputFolder, "msg", "5213155e-87af-4db1-96f7-47fd89f96c57", 120000, "Get Pending Extraction Tasks"));
                LIMS_Workflow.ReadErrorsInResponse(responseObj.errors);
                foreach (TasksQueryResponse_DataV2 myData in responseObj.data)
                {
                    if (myData.parent_workflow_ids.Contains(batchingWorkflowID))
                    {
                        request_id = myData.request_id;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Init Extraction error getting pending extraction tasks: " + ex.Message);
            }

            //extraction_workflow = "";
            try
            {
                //If you failed to init Step 1, but already initialized the workflow, skip trying to init workflow
                //If the tasks are truly not available, Step 1 init will fail
                if (request_id != "")
                {
                    //Init extraction workflow
                    WorkflowPayload myPayload = new WorkflowPayload();
                    myPayload.name = "RNA Extraction";
                    myPayload.spec_name = "rna_extraction_v1.0";
                    myPayload.request_id = request_id;

                    string apiAddress = "https://" + LIMSEndpoint + "/api/lab_workflow/v1/initialize-workflow/";
                    LIMSResponse responseObj = JsonConvert.DeserializeObject<LIMSResponse>(HttpWrapper.SendJSONToAPI(JsonConvert.SerializeObject(myPayload), RunID, runOutputFolder, apiAddress, "msg", "5213155e-87af-4db1-96f7-47fd89f96c57", 360000, "Initialize Extraction Workflow"));
                    LIMS_Workflow.ReadErrorsInResponse(responseObj.errors);
                    extraction_workflow = responseObj.data.workflow.unique_id;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Init Extraction error initializing extraction workflow: " + ex.Message);
            }

            try
            {
                //Init extraction step 1 
                StepPayload myPayload = new StepPayload();
                StepPayload_Inputs_InitExtractionStep1 myInput = new StepPayload_Inputs_InitExtractionStep1();
                myInput.machine = LMNumber;
                myInput.rack_id = tubeRackID;
                myInput.tube_map_file = datafile;
                myInput.nuclease_free_ethanol_buffer = nuclease_free_ethanol;
                myInput.dnasei_mix = dnasei_mix;
                myInput.rbb_buffer = rbb_buffer;
                myInput.bbd_buffer = bbd_buffer;
                myInput.isopropanol = isopropanol;
                myInput.wbe_buffer = wbe_buffer;
                myInput.lbf_lysis_buffer = ibf_buffer;
                myInput.proteinase_k = proK;
                myInput.nuclease_free_water = nuclease_free_water;

                myPayload.inputs = myInput;
                myPayload.workflow = extraction_workflow;
                myPayload.step_name = "RNA Extraction";
                myPayload.user = "extraction@invitae.com";

                string apiAddress = "https://" + LIMSEndpoint + "/api/lab_workflow/v1/initialize-step/";
                LIMSResponse responseObj = JsonConvert.DeserializeObject<LIMSResponse>(HttpWrapper.SendJSONToAPI(JsonConvert.SerializeObject(myPayload), RunID, runOutputFolder, apiAddress, "msg", "5213155e-87af-4db1-96f7-47fd89f96c57", 120000, "Initialize Extraction Step: RNA Extraction"));
                LIMS_Workflow.ReadErrorsInResponse(responseObj.errors);
            }
            catch (Exception ex)
            {
                
                if (ex.Message.Contains("NotFoundError: [\u0027Entity/unique_id\u0027, \u0027\u0027]\n"))
                {
                    //This would happen if you were attempting to retry on step 1 init, when the workflow was already initialized
                    //throw new Exception("Pending RNA extraction task not yet available in LIMS");
                    extraction_workflow += ", Pending RNA extraction task not yet available in LIMS";
                }
                else
                {
                    //throw new Exception("Init Extraction Error initializing extraction step 1: " + ex.Message);
                    extraction_workflow += ", Init Extraction Error initializing extraction step 1: " + ex.Message;
                }
            }
            return extraction_workflow;
        }

        public static void CompleteExtraction(string LIMSEndpoint, string SystemFolderPath, string RunID, string extractionWorkflowID)
        {
            string runOutputFolder = SystemFolderPath + "\\RunOutputFiles\\";

            try
            {
                //Complete RNA Extraction Workflow Step 1
                StepPayload myPayload = new StepPayload();

                myPayload.inputs = new object();
                myPayload.step_name= "RNA Extraction";
                myPayload.user = "Test";
                myPayload.workflow = extractionWorkflowID;

                string apiAddress = "https://" + LIMSEndpoint + "/api/lab_workflow/v1/complete-step/";
                LIMSResponse responseObj = JsonConvert.DeserializeObject<LIMSResponse>(HttpWrapper.SendJSONToAPI(JsonConvert.SerializeObject(myPayload), RunID, runOutputFolder, apiAddress, "msg", "5213155e-87af-4db1-96f7-47fd89f96c57", 120000, "Complete Extraction Step: RNA Extraction"));
                LIMS_Workflow.ReadErrorsInResponse(responseObj.errors);
            }
            catch (Exception ex)
            {
                throw new Exception("Complete Extraction Step1 error: " + ex.Message);
            }
        }

        public static string InitQuantification(string LIMSEndpoint, string SystemFolderPath, string RunID, string extractionWorkflowID, string tubeRackID, string LMNumber, string picoDyePlate, string picoStandardPlate, string quantification_workflow)
        {
            string runOutputFolder = SystemFolderPath + "\\RunOutputFiles\\";

            string mapfile = "";
            try
            {
                //Upload tube data file" 
                DataTable myScan = ReadCSV(SystemFolderPath + "\\SampleScanFiles\\" + tubeRackID + ".csv");
                //myScan.Columns.Remove("STATUS");
                myScan.Columns["RACK"].ColumnName = "rack";
                myScan.Columns["TUBE"].ColumnName = "position";
                myScan.Columns["BARCODE"].ColumnName = "tube";

                string myFile = DataTableToCommaDelimitedString(myScan);
                DatafileUploadPayload myPayload = new DatafileUploadPayload();
                myPayload.file_name = tubeRackID + "_Quant_Tube_Map.csv";
                myPayload.file_data = myFile;

                string apiAddress = "https://" + LIMSEndpoint + "/api/lab_workflow/v1/upload-datafile/";
                DatafileUploadResponse responseObj = JsonConvert.DeserializeObject<DatafileUploadResponse>(HttpWrapper.SendJSONToAPI(JsonConvert.SerializeObject(myPayload), RunID, runOutputFolder, apiAddress, "msg", "5213155e-87af-4db1-96f7-47fd89f96c57", 120000, "Upload Quant Tube Map"));
                LIMS_Workflow.ReadErrorsInResponse(responseObj.errors);
                mapfile = responseObj.data.data_file;
            }
            catch (Exception ex)
            {
                throw new Exception("Error quantification uploading tube map file: " + ex.Message);
            }

            string request_id = "";
            try
            {
                //Get pending quant tasks 
                string apiAddress = "https://" + LIMSEndpoint + "/api/lab_workflow/v1/tasks/?spec_type=RNA Quant";
                TasksQueryResponseV2 responseObjExtractionTasks = JsonConvert.DeserializeObject<TasksQueryResponseV2>(HttpWrapper.HttpGet(apiAddress, RunID, runOutputFolder, "msg", "5213155e-87af-4db1-96f7-47fd89f96c57", 120000, "Get Pending Quant Tasks"));
                LIMS_Workflow.ReadErrorsInResponse(responseObjExtractionTasks.errors);
                foreach (TasksQueryResponse_DataV2 myData in responseObjExtractionTasks.data)
                {
                    if (myData.parent_workflow_ids.Contains(extractionWorkflowID))
                    {
                        request_id = myData.request_id;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error getting pending quantification tasks: " + ex.Message);
            }

            //string quantification_workflow = "";
            try
            {
                //If you failed to init Step 1, but already initialized the workflow, skip trying to init workflow
                //If the tasks are truly not available, Step 1 init will fail
                if (request_id != "")
                {
                    //Init quantification workflow
                    WorkflowPayload myPayload = new WorkflowPayload();
                    myPayload.name = "RNA Quant";
                    myPayload.spec_name = "rna_quant_v1.0";
                    myPayload.request_id = request_id;

                    string apiAddress = "https://" + LIMSEndpoint + "/api/lab_workflow/v1/initialize-workflow/";
                    LIMSResponse responseObj = JsonConvert.DeserializeObject<LIMSResponse>(HttpWrapper.SendJSONToAPI(JsonConvert.SerializeObject(myPayload), RunID, runOutputFolder, apiAddress, "msg", "5213155e-87af-4db1-96f7-47fd89f96c57", 120000, "Initialize Quantification Workflow"));
                    LIMS_Workflow.ReadErrorsInResponse(responseObj.errors);
                    quantification_workflow = responseObj.data.workflow.unique_id;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error initializing quantification workflow: " + ex.Message);
            }

            try
            {
                //Init quantification step 1 
                StepPayload myPayload = new StepPayload();
                StepPayload_Inputs_InitQuantificationStep1 myInput = new StepPayload_Inputs_InitQuantificationStep1();

                myInput.quanting_machine = LMNumber;
                myInput.rack_id = tubeRackID;
                myInput.rna_quant_it_dye_plate = picoDyePlate;
                myInput.rna_quant_xr_standards_plate = picoStandardPlate;
                myInput.tube_map_file = mapfile;

                myPayload.inputs = myInput;
                myPayload.workflow = quantification_workflow;
                myPayload.step_name = "RNA Quantification";
                myPayload.user = "extraction@invitae.com";

                string apiAddress = "https://" + LIMSEndpoint + "/api/lab_workflow/v1/initialize-step/";
                LIMSResponse responseObj = JsonConvert.DeserializeObject<LIMSResponse>(HttpWrapper.SendJSONToAPI(JsonConvert.SerializeObject(myPayload), RunID, runOutputFolder, apiAddress, "msg", "5213155e-87af-4db1-96f7-47fd89f96c57", 120000, "Initialize Quantification Step: RNA Quantification"));
                LIMS_Workflow.ReadErrorsInResponse(responseObj.errors);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("NotFoundError: [\u0027Entity/unique_id\u0027, \u0027\u0027]\n"))
                {
                    //This would happen if you were attempting to retry on step 1 init, when the workflow was already initialized
                    quantification_workflow += ", Pending RNA quantification task not yet available in LIMS";
                }
                else
                {
                    quantification_workflow += ", Error initializing quantification step 1: " + ex.Message;
                }
            }
            return quantification_workflow;
        }

        public static void CompleteQuantification(string LIMSEndpoint, string SystemFolderPath, string RunID, string quantificationWorkflowID, string readerFileName)
        {
            string runOutputFolder = SystemFolderPath + "\\RunOutputFiles\\";

            try
            {
                //Complete RNA Quantification Step 1
                StepPayload myPayload = new StepPayload();
                StepPayload_Inputs_CompleteQuantificationStep1 myInput = new StepPayload_Inputs_CompleteQuantificationStep1();
                // List<RNA_ExtractionQuantData> myData = new List<RNA_ExtractionQuantData>();

                string quantFilePath = SystemFolderPath + "\\FluostarOutputFiles\\" + readerFileName  +".csv"; 
                FormatFlourostarData.QuantPlate fData = new FormatFlourostarData.QuantPlate(quantFilePath);
                myInput.r_squared= fData.R2.ToString();
                myInput.slope = fData.Slope.ToString();
                myInput.quant_data= fData.FormattedJSON;

                myPayload.inputs = myInput;
                myPayload.step_name = "RNA Quantification";
                myPayload.user = "Test";
                myPayload.workflow = quantificationWorkflowID;

                string apiAddress = "https://" + LIMSEndpoint + "/api/lab_workflow/v1/complete-step/";
                LIMSResponse responseObj = JsonConvert.DeserializeObject<LIMSResponse>(HttpWrapper.SendJSONToAPI(JsonConvert.SerializeObject(myPayload), RunID, runOutputFolder, apiAddress, "msg", "5213155e-87af-4db1-96f7-47fd89f96c57", 120000, "Complete Quantification Step: RNA Quantification"));
                LIMS_Workflow.ReadErrorsInResponse(responseObj.errors);
            }
            catch (Exception ex)
            {
                throw new Exception("Error completing quantification step 1: " + ex.Message);
            }
        }

        public static void CheckPendingNormTask(string LIMSEndpoint, string SystemFolderPath, string RunID, string quantificationWorkflowID)
        {
            string apiAddress = "https://" + LIMSEndpoint + "/api/lab_workflow/v1/tasks/?spec_type=RNA Norm";
            string runOutputFolder = SystemFolderPath + "\\RunOutputFiles\\";
            string request_id = "";
            try
            {
                //Get pending norm tasks 
                TasksQueryResponseV2 responseObjExtractionTasks = JsonConvert.DeserializeObject<TasksQueryResponseV2>(HttpWrapper.HttpGet(apiAddress, RunID, runOutputFolder, "msg", "5213155e-87af-4db1-96f7-47fd89f96c57", 120000, "Get Pending Normalization Tasks"));
                LIMS_Workflow.ReadErrorsInResponse(responseObjExtractionTasks.errors);
                foreach (TasksQueryResponse_DataV2 myData in responseObjExtractionTasks.data)
                {
                    if (myData.parent_workflow_ids.Contains(quantificationWorkflowID))
                    {
                        request_id = myData.request_id;
                    }

                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error getting pending RNA normilization tasks: " + ex.Message);
            }
        }
        public static string InitNormalization(string LIMSEndpoint, string SystemFolderPath, string RunID, string quantificationWorkflowID, string tubeRackID, string LMNumber, string normalization_workflow, string nucleaseFreeWater)
        {
            string runOutputFolder = SystemFolderPath + "\\RunOutputFiles\\";

            string mapfile = "";
            try
            {
                //Upload tube data file" 
                DataTable myScan = ReadCSV(SystemFolderPath + "\\SampleScanFiles\\" + tubeRackID + ".csv");
                //myScan.Columns.Remove("STATUS");
                myScan.Columns["RACK"].ColumnName = "rack";
                myScan.Columns["TUBE"].ColumnName = "position";
                myScan.Columns["BARCODE"].ColumnName = "tube";

                string myFile = DataTableToCommaDelimitedString(myScan);
                DatafileUploadPayload myPayload = new DatafileUploadPayload();
                myPayload.file_name = tubeRackID + "_Norm_Tube_Map.csv";
                myPayload.file_data = myFile;

                string apiAddress = "https://" + LIMSEndpoint + "/api/lab_workflow/v1/upload-datafile/";
                DatafileUploadResponse responseObj = JsonConvert.DeserializeObject<DatafileUploadResponse>(HttpWrapper.SendJSONToAPI(JsonConvert.SerializeObject(myPayload), RunID, runOutputFolder, apiAddress, "msg", "5213155e-87af-4db1-96f7-47fd89f96c57", 120000, "Upload Normalization Tube Map"));
                LIMS_Workflow.ReadErrorsInResponse(responseObj.errors);
                mapfile = responseObj.data.data_file;
            }
            catch (Exception ex)
            {
                throw new Exception("Init normalization error uploading tube map file: " + ex.Message);
            }

            string request_id = "";
            try
            {
                //Get pending norm tasks 
                string apiAddress = "https://" + LIMSEndpoint + "/api/lab_workflow/v1/tasks/?spec_type=RNA Norm";
                TasksQueryResponseV2 responseObjExtractionTasks = JsonConvert.DeserializeObject<TasksQueryResponseV2>(HttpWrapper.HttpGet(apiAddress, RunID, runOutputFolder, "msg", "5213155e-87af-4db1-96f7-47fd89f96c57", 120000, "Get Pending Normalization Tasks"));
                LIMS_Workflow.ReadErrorsInResponse(responseObjExtractionTasks.errors);
                foreach (TasksQueryResponse_DataV2 myData in responseObjExtractionTasks.data)
                {
                    if (myData.parent_workflow_ids.Contains(quantificationWorkflowID))
                    {
                        request_id = myData.request_id;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Init Normalization error getting pending RNA normilization tasks: " + ex.Message);
            }

            //string normalization_workflow = "";
            try
            {
                if (request_id != "")
                {
                    //Init normalization workflow
                    WorkflowPayload myPayload = new WorkflowPayload();
                    myPayload.name = "RNA Norm";
                    myPayload.spec_name = "rna_norm_v1.0";
                    myPayload.request_id = request_id;

                    string apiAddress = "https://" + LIMSEndpoint + "/api/lab_workflow/v1/initialize-workflow/";
                    LIMSResponse responseObj = JsonConvert.DeserializeObject<LIMSResponse>(HttpWrapper.SendJSONToAPI(JsonConvert.SerializeObject(myPayload), RunID, runOutputFolder, apiAddress, "msg", "5213155e-87af-4db1-96f7-47fd89f96c57", 120000, "Initialize RNA Normalization Workflow"));
                    LIMS_Workflow.ReadErrorsInResponse(responseObj.errors);
                    normalization_workflow = responseObj.data.workflow.unique_id;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("InitNormalization error initializing normalization workflow: " + ex.Message);
            }

            try
            {
                //Init normalization step 1 
                StepPayload myPayload = new StepPayload();
                StepPayload_Inputs_InitNormilizationStep1 myInput = new StepPayload_Inputs_InitNormilizationStep1();

                myInput.norm_machine = LMNumber;
                myInput.rack_id = tubeRackID;
                myInput.tube_map_file = mapfile;
                myInput.nuclease_free_water_plate = nucleaseFreeWater;

                myPayload.inputs = myInput;
                myPayload.workflow = normalization_workflow;
                myPayload.step_name = "Normalization";
                myPayload.user = "extraction@invitae.com";

                string apiAddress = "https://" + LIMSEndpoint + "/api/lab_workflow/v1/initialize-step/";
                LIMSResponse responseObj = JsonConvert.DeserializeObject<LIMSResponse>(HttpWrapper.SendJSONToAPI(JsonConvert.SerializeObject(myPayload), RunID, runOutputFolder, apiAddress, "msg", "5213155e-87af-4db1-96f7-47fd89f96c57", 120000, "Initialize Workflow Step: RNA Normalization"));
                LIMS_Workflow.ReadErrorsInResponse(responseObj.errors);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("NotFoundError: [\u0027Entity/unique_id\u0027, \u0027\u0027]\n"))
                {
                    //This would happen if you were attempting to retry on step 1 init, when the workflow was already initialized
                  normalization_workflow += ", Pending RNA normilization task not yet available in LIMS";
                }
                else
                {
                  normalization_workflow += ", InitNormalization error initializing RNA normalization step 1: " + ex.Message;
                }
            }
            return normalization_workflow;
        }

        public static string GetNormalizationWorklist(string LIMSEndpoint, string SystemFolderPath, string RunID, string normalization_workflow)
        {
            string runOutputFolder = SystemFolderPath + "\\RunOutputFiles\\";
            string pre_cap_normpool_worklist_filename = "";
            try
            {
                //Get Norm Worklist
                GetMaterialPayload myPayload = new GetMaterialPayload();
                myPayload.material_name = "norm_worklist";
                myPayload.workflow = normalization_workflow;

                string apiAddress = "https://" + LIMSEndpoint + "/api/lab_workflow/v1/material/";
                GetMaterialResponse responseObj = JsonConvert.DeserializeObject<GetMaterialResponse>(HttpWrapper.SendJSONToAPI(JsonConvert.SerializeObject(myPayload), RunID, runOutputFolder, apiAddress, "msg", "5213155e-87af-4db1-96f7-47fd89f96c57", 120000, "Get RNA Normalization Worklist"));
                LIMS_Workflow.ReadErrorsInResponse(responseObj.errors);
                pre_cap_normpool_worklist_filename = responseObj.data.file_name;//OUT VAR! This is the path to the worklist for Venus
                File.WriteAllText(SystemFolderPath + "\\Extraction_Norm_Worklists\\" + responseObj.data.file_name, responseObj.data.file_contents);
            }
            catch (Exception ex)
            {
                throw new Exception("InitNormalization error getting norm worklist: " + ex.Message);
            }
            return pre_cap_normpool_worklist_filename;
        }

        public static void CompleteNormalization(string LIMSEndpoint, string SystemFolderPath, string RunID, string normilizationWorkflowID)
        {
            string runOutputFolder = SystemFolderPath + "\\RunOutputFiles\\";

            try
            {
               //Complete RNA Normalization Step1
                StepPayload myPayload = new StepPayload();

                myPayload.inputs = new object();
                myPayload.step_name = "Normalization";
                myPayload.user = "Test";
                myPayload.workflow = normilizationWorkflowID;

                string apiAddress = "https://" + LIMSEndpoint + "/api/lab_workflow/v1/complete-step/";
                LIMSResponse responseObj = JsonConvert.DeserializeObject<LIMSResponse>(HttpWrapper.SendJSONToAPI(JsonConvert.SerializeObject(myPayload), RunID, runOutputFolder, apiAddress, "msg", "5213155e-87af-4db1-96f7-47fd89f96c57", 120000, "Complete RNA Normalization Workflow Step: Normalization"));
                LIMS_Workflow.ReadErrorsInResponse(responseObj.errors);
            }
            catch (Exception ex)
            {
                throw new Exception("Error completing RNA normilization workflow step1: " + ex.Message);
            }
        }

        public static string InitRetrieveAndStamp(string LIMSEndpoint, string SystemFolderPath, string RunID, string normilizationWorkflowID, string tubeRackID, string LMNumber,string assay_plate_id, string retrieve_and_stamp_workflow)
        {
            string runOutputFolder = SystemFolderPath + "\\RunOutputFiles\\";

            string mapfile = "";
            try
            {
                //Upload tube data file" 
                DataTable myScan = ReadCSV(SystemFolderPath + "\\SampleScanFiles\\" + tubeRackID + ".csv");
                //myScan.Columns.Remove("STATUS");
                myScan.Columns["RACK"].ColumnName = "rack";
                myScan.Columns["TUBE"].ColumnName = "position";
                myScan.Columns["BARCODE"].ColumnName = "tube";

                string myFile = DataTableToCommaDelimitedString(myScan);
                DatafileUploadPayload myPayload = new DatafileUploadPayload();
                myPayload.file_name = tubeRackID + "_Norm_Tube_Map.csv";
                myPayload.file_data = myFile;

                string apiAddress = "https://" + LIMSEndpoint + "/api/lab_workflow/v1/upload-datafile/";
                DatafileUploadResponse responseObj = JsonConvert.DeserializeObject<DatafileUploadResponse>(HttpWrapper.SendJSONToAPI(JsonConvert.SerializeObject(myPayload), RunID, runOutputFolder, apiAddress, "msg", "5213155e-87af-4db1-96f7-47fd89f96c57", 120000, "Upload RNA Retrieve And Stamp Tube Map"));
                LIMS_Workflow.ReadErrorsInResponse(responseObj.errors);
                mapfile = responseObj.data.data_file;
            }
            catch (Exception ex)
            {
                throw new Exception("Init RetrieveAndStamp Error uploading tube map file: " + ex.Message);
            }

            string request_id = "";
            try
            {
                //Get pending retrieve and stamp tasks 
                string apiAddress = "https://" + LIMSEndpoint + "/api/lab_workflow/v1/tasks/?spec_type=RNA Retrieve and Stamp";
                TasksQueryResponseV2 responseObjExtractionTasks = JsonConvert.DeserializeObject<TasksQueryResponseV2>(HttpWrapper.HttpGet(apiAddress, RunID, runOutputFolder, "msg", "5213155e-87af-4db1-96f7-47fd89f96c57", 120000, "Get Pending RNA Retrieve And Stamp Tasks"));
                LIMS_Workflow.ReadErrorsInResponse(responseObjExtractionTasks.errors);
                foreach (TasksQueryResponse_DataV2 myData in responseObjExtractionTasks.data)
                {
                    if (myData.parent_workflow_ids.Contains(normilizationWorkflowID))
                    {
                        request_id = myData.request_id;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("RNA Retrieve and Stamp error getting pending RetrieveAndStamp tasks: " + ex.Message);
            }

            //string retrieve_and_stamp_workflow = "";
            try
            {
                if (request_id != "")
                {
                    //Init retrieve and stamp workflow
                    WorkflowPayload myPayload = new WorkflowPayload();
                    myPayload.name = "RNA Retrieve And Stamp";
                    myPayload.spec_name = "rna_retrieve_and_stamp_v1.0";
                    myPayload.request_id = request_id;

                    string apiAddress = "https://" + LIMSEndpoint + "/api/lab_workflow/v1/initialize-workflow/";
                    LIMSResponse responseObj = JsonConvert.DeserializeObject<LIMSResponse>(HttpWrapper.SendJSONToAPI(JsonConvert.SerializeObject(myPayload), RunID, runOutputFolder, apiAddress, "msg", "5213155e-87af-4db1-96f7-47fd89f96c57", 120000, "Initialize RNA Retrieve And Stamp Workflow"));
                    LIMS_Workflow.ReadErrorsInResponse(responseObj.errors);
                    retrieve_and_stamp_workflow = responseObj.data.workflow.unique_id;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("RNA Retrieve And Stamp Error initializing RetrieveAndStamp workflow: " + ex.Message);
            }

            try
            {
                //Init RNA retrieve and stamp step 1 
                StepPayload myPayload = new StepPayload();
                StepPayload_Inputs_InitRetrieveAndStampStep1 myInput = new StepPayload_Inputs_InitRetrieveAndStampStep1();

                myInput.machine_id = LMNumber;
                myInput.rack_id = tubeRackID;
                myInput.assay_plate_id = assay_plate_id;
                myInput.tube_map_file = mapfile;

                myPayload.inputs = myInput;
                myPayload.workflow = retrieve_and_stamp_workflow;
                myPayload.step_name = "Retrieve and Stamp";
                myPayload.user = "extraction@invitae.com";

                string apiAddress = "https://" + LIMSEndpoint + "/api/lab_workflow/v1/initialize-step/";
                LIMSResponse responseObj = JsonConvert.DeserializeObject<LIMSResponse>(HttpWrapper.SendJSONToAPI(JsonConvert.SerializeObject(myPayload), RunID, runOutputFolder, apiAddress, "msg", "5213155e-87af-4db1-96f7-47fd89f96c57", 120000, "Initialize Workflow Step: RNA Retrieve And Stamp"));
                LIMS_Workflow.ReadErrorsInResponse(responseObj.errors);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("NotFoundError: [\u0027Entity/unique_id\u0027, \u0027\u0027]\n"))
                {
                    //This would happen if you were attempting to retry on step 1 init, when the workflow was already initialized
                    retrieve_and_stamp_workflow += ", Pending RNA Retrieve And Stamp task not yet available in LIMS";
                }
                else
                {
                    retrieve_and_stamp_workflow += ", RNA Retrieve And Stamp error initializing RNA normalization step 1: " + ex.Message;
                }
            }
            return retrieve_and_stamp_workflow;
        }

        public static void CompleteRetrieveAndStamp(string LIMSEndpoint, string SystemFolderPath, string RunID, string retrieveAndStampWorkflowID)
        {
            string runOutputFolder = SystemFolderPath + "\\RunOutputFiles\\";

            try
            {
                //Complete RNA Retrieve And Stamp Step1
                StepPayload myPayload = new StepPayload();

                myPayload.inputs = new object();
                myPayload.step_name = "Retrieve and Stamp";
                myPayload.user = "Test";
                myPayload.workflow = retrieveAndStampWorkflowID;

                string apiAddress = "https://" + LIMSEndpoint + "/api/lab_workflow/v1/complete-step/";
                LIMSResponse responseObj = JsonConvert.DeserializeObject<LIMSResponse>(HttpWrapper.SendJSONToAPI(JsonConvert.SerializeObject(myPayload), RunID, runOutputFolder, apiAddress, "msg", "5213155e-87af-4db1-96f7-47fd89f96c57", 120000, "Complete RNA Retrieve And Stamp step1"));
                LIMS_Workflow.ReadErrorsInResponse(responseObj.errors);
            }
            catch (Exception ex)
            {
                throw new Exception("Error completing RNA Retrieve And Stamp workflow step1: " + ex.Message);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////

        private static DataTable ReadCSV(string path)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("RACK", typeof(string));
            dt.Columns.Add("TUBE", typeof(string));
            dt.Columns.Add("BARCODE", typeof(string));
            string[] myLines = File.ReadAllLines(path);

            for (int j = 0; j < myLines.Length; j++)
            {
                if (j == 0)
                {
                    //skip header
                }
                else
                {
                    string[] oneLine = myLines[j].Split(',');
                    if (oneLine[0] != "" && oneLine[2] != "")
                    {
                        DataRow myRow = dt.NewRow();
                        myRow["RACK"] = oneLine[0];
                        myRow["TUBE"] = oneLine[1];
                        myRow["BARCODE"] = oneLine[2];
                        dt.Rows.Add(myRow);
                    }
                }
            }
            dt.AcceptChanges();
            return dt;
        }

        private static DataTable ReadCSV2(string path)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("source_id", typeof(string));
            dt.Columns.Add("rack_id", typeof(string));
            dt.Columns.Add("tube_id", typeof(string));
            dt.Columns.Add("well", typeof(string));
            dt.Columns.Add("volume", typeof(string));
            dt.Columns.Add("source_type", typeof(string));
            string[] myLines = File.ReadAllLines(path);

            for (int j = 0; j < myLines.Length; j++)
            {
                if (j == 0)
                {
                    //skip header
                }
                else
                {
                    string[] oneLine = myLines[j].Split(',');
                    if (oneLine[0] != "")
                    {
                        DataRow myRow = dt.NewRow();
                        myRow["source_id"] = oneLine[0];
                        myRow["rack_id"] = oneLine[1];
                        myRow["well"] = oneLine[3];
                        myRow["volume"] = oneLine[4];
                        myRow["source_type"] = oneLine[5];
                        dt.Rows.Add(myRow);
                    }
                }
            }
            dt.AcceptChanges();
            return dt;
        }

        private static string DataTableToCommaDelimitedString(DataTable myOutTable)
        {
            string strRowCommaSeparated = "";
            for (int i = 0; i < myOutTable.Columns.Count; i++)//Create Header Row
            {
                if (i != 0)
                {
                    strRowCommaSeparated = strRowCommaSeparated + ",";
                }
                strRowCommaSeparated = strRowCommaSeparated + myOutTable.Columns[i].ColumnName.ToString();
            }
            strRowCommaSeparated = strRowCommaSeparated + "\n";
            foreach (DataRow dr in myOutTable.Rows)
            {
                for (int i = 0; i < dr.ItemArray.Length; i++)
                {
                    if (i != 0)
                    {
                        strRowCommaSeparated = strRowCommaSeparated + ",";
                    }
                    strRowCommaSeparated = strRowCommaSeparated + dr.ItemArray[i].ToString();
                }
                strRowCommaSeparated = strRowCommaSeparated + "\n";
            }
            return strRowCommaSeparated;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////

        private class RunInfo_RNAExtraction
        {
            public string automation_system { get; set; }
            public List<OperationData_RNAExtraction> operation_data_rna { get; set; }
        }

        private class OperationData_RNAExtraction
        {
            public string plate_id { get; set; }
            public string tube_rack_id { get; set; }
            public string scanfile_name { get; set; }
            public string extraction_batching_workflow { get; set; }
            public string status { get; set; }
        }

        private class LoggerClass
        {
            public string sender { get; set; }
            public string reason { get; set; }
            public string endpoint { get; set; }
            public string timestamp { get; set; }
            public object message { get; set; }
        }

        private class ListOfIB
        {
            public List<string> specimen_ids { get; set; }
        }

        private class SpecimenResponse
        {
            public SpecimenResponse_data data { get; set; }
            public List<LIMSResponse_Errors> errors { get; set; }
        }

        private class SpecimenResponse_data
        {
            public string result { get; set; }
            public List<string> invalid_specimens { get; set; }
        }

        private class TasksQueryResponse_Data_Arguments_ExtractionBatchSel
        {
            public string specimen_id { get; set; }
            public string platform { get; set; }
            public string workflow_mode { get; set; }
            public string protocol_type { get; set; }
            public string assay_batch_ids { get; set; }
        }

        private class WorkflowPayload_Inputs_InitRNAExtractionBatchSel
        {
            public string assay_id { get; set; }
            public List<string> request_ids { get; set; }
            public List<string> control_ids { get; set; }
            public List<string> filler_ids { get; set; }
        }

        private class StepPayload_Inputs_InitExtractionStep1
        {
            public string machine { get; set; }
            public string rack_id { get; set; }
            public string tube_map_file { get; set; }
            public string nuclease_free_ethanol_buffer { get; set; }
            public string dnasei_mix { get; set; }
            public string rbb_buffer { get; set; }
            public string bbd_buffer { get; set; }
            public string isopropanol { get; set; }
            public string wbe_buffer { get; set; }
            public string lbf_lysis_buffer { get; set; }
            public string proteinase_k { get; set; }
            public string nuclease_free_water { get; set; }

        }

        private class StepPayload_Inputs_InitQuantificationStep1
        {
            public string quanting_machine { get; set; }
            public string rack_id { get; set; }
            public string rna_quant_xr_standards_plate { get; set; }
            public string rna_quant_it_dye_plate { get; set; }
            public string tube_map_file { get; set; }
        }

        private class StepPayload_Inputs_InitNormilizationStep1
        {
            public string norm_machine { get; set; }
            public string rack_id { get; set; }
            public string tube_map_file { get; set; }
            public string nuclease_free_water_plate { get; set; }
        }

        private class StepPayload_Inputs_InitRetrieveAndStampStep1
        {
            public string machine_id { get; set; }
            public string rack_id { get; set; }
            public string assay_plate_id { get; set; }
            public string tube_map_file { get; set; }

        }

        private class StepPayload_Inputs_CompleteExtractionStep1
        {
            public string tube_map_file { get; set; }
        }

        private class StepPayload_Inputs_CompleteQuantificationStep1
        {
            public string r_squared { get; set; }
            public string slope { get; set; }
            public List<FormatFlourostarData.QuantData> quant_data { get; set; }
        }

        private class DatafileUploadPayload
        {
            public string file_name { get; set; }
            public string file_data { get; set; }
        }

        private class DatafileUploadResponse
        {
            public DatafileUploadResponse_data data { get; set; }
            public List<LIMSResponse_Errors> errors { get; set; }
        }

        private class DatafileUploadResponse_data
        {
            public string data_file { get; set; }
        }

       


    }
}

