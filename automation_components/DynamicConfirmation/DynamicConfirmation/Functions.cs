using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Windows.Forms;
using System.Threading;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using System.Security.Permissions;


namespace DynamicConfirmation
{
	public static class Globals
	{

		public static string directory = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, @"..\..\..\Json Files\"));		//set the directory based on whichever directory the solution is being run from
		public static string tempJobDirectory = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, @"..\..\..\Temp Job Folder\")); 
		public static string supportingFilesDirectory = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, @"..\..\..\Supporting Files\")); 
		public static string dashboardApplicationDirectory = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, @"..\..\..\DashboardApplication\bin\Debug\"));

		public static string currentJobID;
		public static string systemStatus;
		public static string[] microserveLabwareTypes = { "Bravo_Tips_180uL", "Nimbus_Tips_50uL", "Nimbus_Tips_300uL", "Nunc_1mL", "Nunc_500uL", "Epp_sk_PCR","Dropquant_Labchip" };
		public static string[] reagentTypes = { "Assay Plate", "EB", "AMPure Beads", "EtOH", "PCR Mastermix", "Primers" };
		public static string[] microserveLabware;
		public static string[] reagents;
		public static int maxJobsInProgress = 3;
		public static bool inventory = false;
		public static System.Web.Script.Serialization.JavaScriptSerializer ser = new System.Web.Script.Serialization.JavaScriptSerializer();
	}

	public class Functions
	{
		public static string OpenJson(string path)
		{
			string JSONFileText = "";

			try
			{
				JSONFileText = File.ReadAllText(path);
			}
			catch (FileNotFoundException)
			{
				MessageBox.Show("The JSON file could not be found!" + System.Environment.NewLine + System.Environment.NewLine + "Please abort the Overlord method", "File Open Error", MessageBoxButtons.OK);
			}
			catch (DirectoryNotFoundException)
			{
				MessageBox.Show("The path specified (" + path + ") is invalid - Check that the drive is mapped correctly, and that a Run Folder is being created" + System.Environment.NewLine + System.Environment.NewLine + "Please abort the Overlord method", "File Open Error", MessageBoxButtons.OK);
			}

			return JSONFileText;
		}

		public static void WriteJson(string path, string text)
		{
			int numberOfRetries = 3;

			for(int i = 1; i < numberOfRetries; i++)
			{
				try
				{
					File.WriteAllText(path, text);
					break;
				}
				catch (IOException e) when (i <= numberOfRetries)
				{
					Thread.Sleep(500);
				}
			}
		}

		public static string[][] CsvToArray(string path)
		{
			StreamReader sr = new StreamReader(path);    //open the CSV
			var lines = new List<string[]>();                                   //create a list of lists called lines
			while (!sr.EndOfStream)
			{
				string[] Line = sr.ReadLine().Split(',');                       //create a list called Line with all of the strings in the line of the CSV (separated by ',')
				lines.Add(Line);                                                //add Line to lines
			}
			var data = lines.ToArray();                                         //create an array called data with all of the lines
			sr.Close();
			return data;
		}

		public static void CreateBlankProtocolTaskList()
		{
			ProtocolTaskList newBlankTaskList = new ProtocolTaskList();
			List<Protocol> myBlankProtocols = new List<Protocol>();
			Protocol blankProtocol = new Protocol();
			blankProtocol.protocol_name = "test";

			List<Task> myDemoTaskList = new List<Task>();
			Task demoTask = new Task();

			List<PlateData> myReagentList = new List<PlateData>();
			List<PlateData> myLabwareList = new List<PlateData>();
			PlateData newPlate = new PlateData();
			myReagentList.Add(newPlate);
			myLabwareList.Add(newPlate);

			demoTask.labware_list = myLabwareList;
			demoTask.reagent_list = myReagentList;
			myDemoTaskList.Add(demoTask);
			blankProtocol.tasks = myDemoTaskList;
			myBlankProtocols.Add(blankProtocol);
			newBlankTaskList.protocols = myBlankProtocols;
			//write JSON file               
			string JSONOutput = Globals.ser.Serialize(newBlankTaskList);
			WriteJson(Globals.supportingFilesDirectory + "BlankProtocolTaskList.json", JSONOutput);
		}

		public static void CreateSystemStatus()
		{
			SystemInventory mySystem = new SystemInventory();
			mySystem.system_lm = "LM9999";
			List<Device> myDeviceList = new List<Device>();

			var data = CsvToArray(Globals.supportingFilesDirectory + "instrument_list.csv");

			for (int i = 1; i < data[0].Length; i++)
			{
				Device newDevice = new Device();
				newDevice.device_type = data[0][i].ToString();       //add newDevice type
				newDevice.device_name = data[1][i].ToString();       //add newDevice name
				newDevice.status = "Ready";							 //set newDevice status to ready
				newDevice.lm_number = data[2][i].ToString();         //add newDevice LM
				int stackCount = Convert.ToInt16(data[3][i]);
				List<Stack> myStacks = new List<Stack>();                      //create list of stack objects
				for (int j = 0; j < stackCount; j++)
				{
					Stack newStack = new Stack();                              //for each stack on instrument, create a stack object
					newStack.stack_num = j + 1;                             //set the stack number
					int stackNestCount = Convert.ToInt16(data[4][i]);
					List<Nest> myStackNests = new List<Nest>();              //create list of shelf objects
					for (int t = 0; t < stackNestCount; t++)
					{
						Nest newNest = new Nest();                       //for each shelf on the stack, create a shelf object
						newNest.nest_num = t + 1;                       //set the nest number
						myStackNests.Add(newNest);                            //add the shelf object to the list of shelf objects
					}
					newStack.nests = myStackNests;                            //set the list of shelves to the stack.shelves attribute
					myStacks.Add(newStack);                                   //add the new stack to the list of stacks
				}
				newDevice.stacks = myStacks;                              //set the list of stacks to the device.stacks attribute
				newDevice.num_nests = Convert.ToInt16(data[5][i]);                //get the number of nests on the instrument (typically 1 or 0)
				List<Nest> myDeviceNests = new List<Nest>();
				for (int k = 0; k < newDevice.num_nests; k++)
				{
					Nest myDeviceNest = new Nest();
					myDeviceNest.nest_num = k + 1;
					myDeviceNests.Add(myDeviceNest);
				}
				newDevice.nests = myDeviceNests;
				myDeviceList.Add(newDevice);
			}
			mySystem.devices = myDeviceList;

			//write JSON file               
			string JSONOutput = Globals.ser.Serialize(mySystem);
			WriteJson(Globals.directory + "SystemStatus.json", JSONOutput);
		}

		public static void CreateTaskManager()
		{
			JobQueue myTaskManager = new JobQueue();
			List<Job> myJoblist = new List<Job>();
			myTaskManager.jobs = myJoblist;

			//write JSON file               
			string JSONOutput = Globals.ser.Serialize(myTaskManager);
			WriteJson(Globals.directory + "TaskManager.json", JSONOutput);
		}

		public static void AddJobToTempFolder(string protocol, string user, string workflow, string priority)
		{
			string protocolTaskListText = OpenJson(Globals.supportingFilesDirectory + "ProtocolTaskList.json");
			ProtocolTaskList myMasterProtocolTaskList = Globals.ser.Deserialize<ProtocolTaskList>(protocolTaskListText); //deserialize master protocol task list into ProtocolTaskList object

			Job newJob = new Job();
			newJob.description = protocol;
			newJob.jobID = workflow;
			newJob.userID = user;
			newJob.jobStatus = "Queued";

			List<TaskList> newTaskList = new List<TaskList>();

			foreach (var masterTask in myMasterProtocolTaskList.protocols.Find(p => p.protocol_name == protocol).tasks)
			{
				TaskList newTask = new TaskList();
				newTask.task = masterTask.task;
				newTask.device_name = masterTask.device_name;
				newTask.status = "Queued";

				List<Plate> taskReagentList = new List<Plate>();
				if(masterTask.reagent_list != null)
				{
					foreach (var reagent in masterTask.reagent_list)
					{
						Plate newReagent = new Plate();
						newReagent.type = reagent.type;
						newReagent.targetNest = reagent.nest;
						taskReagentList.Add(newReagent);
					}
				}

				List<Plate> taskLabwareList = new List<Plate>();
				if(masterTask.labware_list != null)
				{
					foreach (var labware in masterTask.labware_list)
					{
						Plate newLabware = new Plate();
						newLabware.type = labware.type;
						newLabware.targetNest = labware.nest;
						taskLabwareList.Add(newLabware);
					}
				}

				newTask.labware = taskLabwareList;
				newTask.reagents = taskReagentList;
				newTaskList.Add(newTask);
			}
			newJob.tasks = newTaskList;

			List<Plate> jobReagents = new List<Plate>();
			newJob.reagents = jobReagents;

			string tempJobFileName;
			tempJobFileName = priority + "_" + workflow + ".json";

			//write JSON file               
			string JSONOutput = Globals.ser.Serialize(newJob);
			WriteJson(Globals.tempJobDirectory + tempJobFileName, JSONOutput);
		}

		public static void AddJobToTaskManager()
		{	
			int numberOfFilesInFolder = Directory.GetFiles(Globals.tempJobDirectory, "*", SearchOption.TopDirectoryOnly).Length;  //Count the number of plate files in the folder
			if (numberOfFilesInFolder != 0)
			{
				System.Threading.Thread.Sleep(500); //wait for half a second

				string taskManagerText = OpenJson(Globals.directory + "TaskManager.json");
				JobQueue myTaskManager = Globals.ser.Deserialize<JobQueue>(taskManagerText); //deserialize task manager into JobQueue object

				//loop over each job in job queue
				for (int i = 1; i <= numberOfFilesInFolder; i++) //for each plate file in the parent reagent folder
				{
					DirectoryInfo di = new DirectoryInfo(Globals.tempJobDirectory);
					string currentTempJob = di.EnumerateFiles().Select(f => f.Name).FirstOrDefault();
					string currentTempJobPath = Globals.tempJobDirectory + currentTempJob; //open the next file in the folder
					string tempJobFileText = File.ReadAllText(currentTempJobPath);
					Job myJob = Globals.ser.Deserialize<Job>(tempJobFileText);  //deserialize job from temp jobs into Job object

					string tempJobType = currentTempJob.Split('_')[0];      //gets substring up to underscore
					string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(currentTempJob);
					string pgSubstring = fileNameWithoutExtension.Substring(fileNameWithoutExtension.LastIndexOf("_") + 1);     //gets substring after underscore

					switch (tempJobType)
					{
						case "priority":
							if (myTaskManager.jobs.Exists(p => p.jobID == pgSubstring) == false) myTaskManager.jobs.Insert(0, myJob);
							else MessageBox.Show("The priority Job you are trying to add already exists in the Task Manager.\n\nPG Number: " + pgSubstring + "\n\nPlease check your Protocol and PG number.", "Error", MessageBoxButtons.OK);
							break;
						case "standard":
							if (myTaskManager.jobs.Exists(p => p.jobID == pgSubstring) == false) myTaskManager.jobs.Add(myJob);
							else MessageBox.Show("The standard Job you are trying to add already exists in the Task Manager.\n\nPG Number: " + pgSubstring + "\n\nPlease check your Protocol and PG number.", "Error", MessageBoxButtons.OK);
							break;
						case "kill":
							if (myTaskManager.jobs.Exists(p => p.jobID == pgSubstring) == true)
							{
								int index = myTaskManager.jobs.FindIndex(p => p.jobID == pgSubstring); //find the jobID that matches the one to be killed
								myTaskManager.jobs[index].jobStatus = "Kill";
							}
							else MessageBox.Show("The Job you are trying to kill does not exist in the Task Manager.\n\nPlease check your Protocol and PG number.", "Error", MessageBoxButtons.OK);
							break;
					}
					//delete the file that you were just working with
					File.Delete(currentTempJobPath);
				}
				MessageBox.Show("Adding job(s) to jobQueue");
				//write JSON file               
				string JSONOutput = Globals.ser.Serialize(myTaskManager);
				WriteJson(Globals.directory + "TaskManager.json", JSONOutput);
			}
		}

		public static int CountJobsInJobQueue()
		{
			string taskManagerText = OpenJson(Globals.directory + "TaskManager.json");
			JobQueue myTaskManager = Globals.ser.Deserialize<JobQueue>(taskManagerText); //deserialize task manager into JobQueue object

			int jobNumber = myTaskManager.jobs.Count();
			return jobNumber;
		}

		public static void QueryAndUpdateSystemStatus()
		{
			string systemStatusText = OpenJson(Globals.directory + "SystemStatus.json");
			SystemInventory mySystemStatus = Globals.ser.Deserialize<SystemInventory>(systemStatusText); //deserialize system status into SystemInventory object

			//Query overlord for instruments status

			//set any instrument that is currently "Loaded" to "Idle" so that the instrument is available to finish its current step
			foreach(var device in mySystemStatus.devices)
			{
				if (device.status == "Loaded" || device.status == "Busy")
				{
					string updateStatus = Microsoft.VisualBasic.Interaction.InputBox("Current Instrument: " + device.device_name + "(" + device.lm_number + ")" +
						Environment.NewLine + "Current Status: " + device.status, "Update Instrument Status", "Idle"); //will be replaced by system query
					device.status = updateStatus;
				}
			}

			//Update instrumentStatus in System Status
			string JSONOutput = Globals.ser.Serialize(mySystemStatus);
			WriteJson(Globals.directory + "SystemStatus.json", JSONOutput);
		}

		public static bool CheckJobStatus(int jobNumber)
		{
			string taskManagerText = OpenJson(Globals.directory + "TaskManager.json");
			JobQueue myTaskManager = Globals.ser.Deserialize<JobQueue>(taskManagerText); //deserialize task manager into JobQueue object
			bool runUpdateTaskManager = false;

			//is jobStatus for the current job Killing Completing?
			if (myTaskManager.jobs[jobNumber].jobStatus == "Killing" || myTaskManager.jobs[jobNumber].jobStatus == "Completing")
			{
				CompleteJob(jobNumber);
			}
			else if (myTaskManager.jobs[jobNumber].jobStatus == "Queued")
			{
				//count the number of jobs in jobQueue where jobStatus == "In Progress" - if less than maxJobsInProgress, set current job to in progress
				if (myTaskManager.jobs.Count(p => p.jobStatus == "In Progress") < Globals.maxJobsInProgress)
				{
					myTaskManager.jobs[jobNumber].jobStatus = "In Progress";
					string JSONOutput = Globals.ser.Serialize(myTaskManager);
					WriteJson(Globals.directory + "TaskManager.json", JSONOutput);  //write JSON file if we're going to set a new job status    
				}
			}
			//if the jobStatus for the current job In Progress run the next task for that job
			if (myTaskManager.jobs[jobNumber].jobStatus == "In Progress") runUpdateTaskManager = true;
			return runUpdateTaskManager;
		}

		public static string UpdateTaskManager(int jobNumber)
		{
			string returnString = ""; //return string - either StartJob, jobID <<<<OR>>>> RunTask, message
			
			//open task manager
			string taskManagerText = OpenJson(Globals.directory + "TaskManager.json");
			JobQueue myTaskManager = Globals.ser.Deserialize<JobQueue>(taskManagerText); //deserialize task manager into JobQueue object

			//get currentTaskIndex and next taskIndex
			int currentTaskIndex = myTaskManager.jobs[jobNumber].tasks.FindIndex(p => p.status == "In Progress");   //find the current task in progress
			int nextTaskIndex = currentTaskIndex +1; //the next task is the one after whatever is 'In Progress';
			//if this is the first task of the job, nothing will be "in progress" yet, so the index will be -1
			if (currentTaskIndex == -1) returnString = "StartJob"; //if there are no tasks "In Progress" run start job
			else
			{
				string currentInstrumentID = myTaskManager.jobs[jobNumber].tasks[currentTaskIndex].instrumentID; //get the LM number for the instrument being used in the current task
				string currentInstrumentStatus = GetInstrumentStatus(currentInstrumentID); //check the status of that instrument
				if (currentInstrumentStatus == "Ready" || currentInstrumentStatus == "Idle")  //if the status of the current instrument is NOT Busy (Ready, Loaded, Busy, Idle)
				{
					////////////////create a quarantine task to insert at nextTaskIndex if needed////////////////
					TaskList newQuarantineTask = new TaskList();
					List<Plate> quarantineTaskReagentList = new List<Plate>();
					List<Plate> quarantineTaskLabwareList = new List<Plate>();
					newQuarantineTask.task = "Quarantine";
					newQuarantineTask.status = "Queued";
					newQuarantineTask.device_name = "Static Stacker";
					Plate newReagent = new Plate();
					newReagent.type = "Assay Plate"; //the plate required in a quarantine step is always the assay plate
					quarantineTaskReagentList.Add(newReagent);	//add the plate to the reagent list
					newQuarantineTask.reagents = quarantineTaskReagentList; //add the reagent list to the reagents attribute of the quarantine task
					newQuarantineTask.labware = quarantineTaskLabwareList;	//add the empty labware list to the labware attribute of the quarantine task
					////////////////
					
					if (myTaskManager.jobs[jobNumber].jobStatus == "Kill") //check if we're killing this job, if so, we need to add a task to quarantine the plate
					{
						myTaskManager.jobs[jobNumber].jobStatus = "Killing";
						myTaskManager.jobs[jobNumber].tasks.Insert(nextTaskIndex, newQuarantineTask); //insert the quarantine task as the next task queued for the current job
					}
					if (myTaskManager.jobs[jobNumber].tasks.Count(p => p.status == "Queued") == 0) //check if there are any queued tasks, if not, we can add a task to quarantine the plate
					{
						myTaskManager.jobs[jobNumber].jobStatus = "Completing";
						myTaskManager.jobs[jobNumber].tasks.Insert(nextTaskIndex, newQuarantineTask); //insert the quarantine task as the next task queued for the current job
						MessageBox.Show("just set the job status to Completing. Will not run Complete Job until next job loop");
					}

					//check if there's an available instrument for the next task - if the next instrument has stacks, figure out which shelf to put plate on
					bool runTask = true; //only switched to false if plate is already in quarantine and next instrument is still not available
					string nextInstrumentType = myTaskManager.jobs[jobNumber].tasks[currentTaskIndex+1].device_name;
					//MessageBox.Show("Quarantine Debug! the next instrument type is: " + nextInstrumentType);

					if (GetAvailableInstrument(nextInstrumentType) == "")    //if there are none available, and you're not currently in quarantine, add a quarantine task
					{
						MessageBox.Show("Quarantine: no devices available");
						if (myTaskManager.jobs[jobNumber].tasks[currentTaskIndex].task != "Quarantine")
						{
							myTaskManager.jobs[jobNumber].tasks.Insert(nextTaskIndex, newQuarantineTask); //insert the quarantine task as the next task queued for the current job
							nextInstrumentType = "Static Stacker";
							runTask = true;
						}
						else runTask = false;
					}

					if (runTask == true)
					{
						//update taskStatus of current task and next task
						myTaskManager.jobs[jobNumber].tasks[currentTaskIndex].status = "Complete";
						myTaskManager.jobs[jobNumber].tasks[nextTaskIndex].status = "In Progress";
						myTaskManager.jobs[jobNumber].tasks[nextTaskIndex].instrumentID = GetAvailableInstrument(nextInstrumentType);	//set the InstrumentID (LM Number) of the next task to the next available instrument LM
						returnString = myTaskManager.jobs[jobNumber].jobID;
					}
					//write JSON file               
					string JSONOutput = Globals.ser.Serialize(myTaskManager);
					WriteJson(Globals.directory + "TaskManager.json", JSONOutput);
				}
			}
			return returnString;
		}

		public static bool StartJob(int jobNumber, string[] reagentBarcodes)
		{
			string taskManagerText = OpenJson(Globals.directory + "TaskManager.json");
			JobQueue myTaskManager = Globals.ser.Deserialize<JobQueue>(taskManagerText); //deserialize task manager into JobQueue object
																						 //open System Status
			string systemStatusText = OpenJson(Globals.directory + "SystemStatus.json");
			SystemInventory mySystemStatus = Globals.ser.Deserialize<SystemInventory>(systemStatusText); //deserialize system status into SystemInventory object

			string jobID = myTaskManager.jobs[jobNumber].jobID;
			var scanTask = myTaskManager.jobs[jobNumber].tasks.Find(p => p.task == "Scan Input Buffer");
			scanTask.instrumentID = mySystemStatus.devices.Find(p => p.device_name == "Input Buffer").lm_number;

			bool limsApproves;
			MessageBox.Show("Send reagentBarcode array data to LIMS, get types and goAhead from LIMS");
			
			string limsResponse = Microsoft.VisualBasic.Interaction.InputBox("Does LIMS approve these reagents?", "LIMS Response", "yes"); //reagentBarcode JSON message, send to LIMS, get types and goAhead
			if (limsResponse == "yes")
			{
				string[] reagentTypes = Globals.reagents.ToArray();  //TESTING ONLY!  LIMS will supply reagent type from barcode data

				var device = mySystemStatus.devices.Find(p => p.lm_number == scanTask.instrumentID);
				for (int i = 0; i < reagentBarcodes.Length; i++)
				{
					device.stacks[0].nests[i].plate_barcode = reagentBarcodes[i];  //add barcode data to input buffer stack
					device.stacks[0].nests[i].type = reagentTypes[i];  //add reagent type data to input buffer stack (System Status)

					Plate newReagent = new Plate();
					newReagent.barcode = reagentBarcodes[i];
					newReagent.type = reagentTypes[i];
					myTaskManager.jobs[jobNumber].reagents.Add(newReagent); //add reagent data to job reagent list (Task Manager)
				}
				scanTask.status = "Complete";
				var storeTask = myTaskManager.jobs[jobNumber].tasks.Find(p => p.task == "Store Plates");
				storeTask.status = "In Progress";
				storeTask.instrumentID = mySystemStatus.devices.Find(p => p.device_name == "Liconic").lm_number;
				
				//write JSON file               
				string JSONOutput = Globals.ser.Serialize(myTaskManager);
				WriteJson(Globals.directory + "TaskManager.json", JSONOutput);
				Thread.Sleep(200);
				JSONOutput = Globals.ser.Serialize(mySystemStatus);
				WriteJson(Globals.directory + "SystemStatus.json", JSONOutput);
				limsApproves = true;
			}
			else
			{
				MessageBox.Show("There was an issue validating reagents for your Job (" + jobID + "). Reagents will be rescanned");
				limsApproves = false;
			}
			return limsApproves;
		}

		public static void CompleteJob(int jobNumber)
		{
			//complete job, move out of job queue, and send message to LIMS
			MessageBox.Show("Write JSON output file for Job, Remove Job from jobs, Send Run Summary for LIMS");
			Job myJob = new Job();

			//open task manager
			string taskManagerText = OpenJson(Globals.directory + "TaskManager.json");
			JobQueue myTaskManager = Globals.ser.Deserialize<JobQueue>(taskManagerText); //deserialize task manager into JobQueue object

			//change job status to killed if current status is killing
			if (myTaskManager.jobs[jobNumber].jobStatus == "Killing") myTaskManager.jobs[jobNumber].jobStatus = "Killed";

			//change job status to Complete if current status is completing
			if (myTaskManager.jobs[jobNumber].jobStatus == "Completing")
			{
				myTaskManager.jobs[jobNumber].jobStatus = "Complete";
				int index = myTaskManager.jobs[jobNumber].tasks.FindIndex(p => p.status == "In Progress");
				myTaskManager.jobs[jobNumber].tasks[index].status = "Complete";
				myJob = myTaskManager.jobs[jobNumber];
			}

			//write completed job JSON file               
			string JSONOutput = Globals.ser.Serialize(myJob);
			WriteJson("O:\\DynamicConfirmation\\Completed Jobs\\" + myJob.jobStatus + "_" + myJob.description + "_" + myJob.jobID + ".json", JSONOutput);
			Thread.Sleep(200);
			JSONOutput = Globals.ser.Serialize(myTaskManager);
			WriteJson(Globals.directory + "TaskManager.json", JSONOutput);
		}

		public static void RemoveCompletedJobsFromQueue()
		{
			//open task manager
			string taskManagerText = OpenJson(Globals.directory + "TaskManager.json");
			JobQueue myTaskManager = Globals.ser.Deserialize<JobQueue>(taskManagerText); //deserialize task manager into JobQueue object

			myTaskManager.jobs.RemoveAll(p => p.jobStatus == "Complete");

			string JSONOutput = Globals.ser.Serialize(myTaskManager);
			WriteJson(Globals.directory + "TaskManager.json", JSONOutput);
		}

		public static string GetCurrentJobID (int jobNumber)
		{
			string taskManagerText = OpenJson(Globals.directory + "TaskManager.json");
			JobQueue myTaskManager = Globals.ser.Deserialize<JobQueue>(taskManagerText); //deserialize task manager into JobQueue object

			string currentJobID = myTaskManager.jobs[jobNumber].jobID;
			return currentJobID;
		}

		public static string GetInstrumentStatus(string targetInstrumentLm)
		{
			//open System Status
			string systemStatusText = OpenJson(Globals.directory + "SystemStatus.json");
			SystemInventory mySystemStatus = Globals.ser.Deserialize<SystemInventory>(systemStatusText); //deserialize task manager into JobQueue object

			string instrumentStatus = mySystemStatus.devices.Find(p => p.lm_number == targetInstrumentLm).status;
			return instrumentStatus;
		}

		public static void SetInstrumentStatus(string targetInstrumentLm, string status)
		{
			//open System Status
			string systemStatusText = OpenJson(Globals.directory + "SystemStatus.json");
			SystemInventory mySystemStatus = Globals.ser.Deserialize<SystemInventory>(systemStatusText); //deserialize task manager into JobQueue object
			
			mySystemStatus.devices.Find(p => p.lm_number == targetInstrumentLm).status = status; //find instrument, then update status
																											               
			string JSONOutput = Globals.ser.Serialize(mySystemStatus);  //write JSON file
			WriteJson(Globals.directory + "SystemStatus.json", JSONOutput);
		}

		public static string GetAvailableInstrument(string deviceName)
		{
			//open System Status
			string systemStatusText = OpenJson(Globals.directory + "SystemStatus.json");
			SystemInventory mySystemStatus = Globals.ser.Deserialize<SystemInventory>(systemStatusText); //deserialize system status into SystemInventory object
			bool locationFound = false;
			string instrumentID = "";

			if (deviceName == "Static Stacker" || deviceName == "Liconic")
			{
				foreach (var device in mySystemStatus.devices.Where(p => p.device_name == deviceName))
				{
					foreach (var stack in device.stacks)
					{
						foreach (var nest in stack.nests)
						{
							if (nest.plate_barcode == null)
							{
								locationFound = true;
								instrumentID = device.lm_number;
								break;
							}
						}
						if (locationFound == true) break;
					}
					if (locationFound == true) break;
				}
			}
			else
			{
				foreach (var device in mySystemStatus.devices.Where(p => p.device_name == deviceName))
				{
					if (device.status == "Ready")
					{
						locationFound = true;
						instrumentID = device.lm_number;
						break;
					}
				}
			}
			return instrumentID;
		}

		public static object PutPlateOnSystem(string jobID, string plateBarcode)
		{
			//open task manager
			string taskManagerText = OpenJson(Globals.directory + "TaskManager.json");
			JobQueue myTaskManager = Globals.ser.Deserialize<JobQueue>(taskManagerText); //deserialize task manager into JobQueue object
			var currentTask = myTaskManager.jobs.Find(p => p.jobID == jobID).tasks.Find(a => a.status == "In Progress");

			//open System Status
			string systemStatusText = OpenJson(Globals.directory + "SystemStatus.json");
			SystemInventory mySystemStatus = Globals.ser.Deserialize<SystemInventory>(systemStatusText); //deserialize system status into SystemInventory object

			SystemAction myAction = new SystemAction();
			myAction.isValid = false;   //set to true when plateBarcode is found
			myAction.action = "putPlate";

			var targetDevice = mySystemStatus.devices.Find(p => p.lm_number == currentTask.instrumentID);
			string targetDeviceType = targetDevice.device_type;      //get the instrument type for the current instrument name
			var myRobotNest = mySystemStatus.devices.Find(p => p.device_type == "Robot").nests[0];

			bool transferBreak = false;
			if (targetDeviceType == "Random Access")                                //if the instrument is random access, we know to look at stacks
			{
				foreach (var stack in targetDevice.stacks)					
				{
					foreach (var nest in stack.nests)
					{
						if (nest.plate_barcode == null)
						{
							myAction.isValid = true;                                    //myAction is returned from function with details from mySystemStatus.instrument
							myAction.device_name = targetDevice.device_name;
							myAction.lm_number = targetDevice.lm_number;
							myAction.stack_num = stack.stack_num;
							myAction.plate_type = myRobotNest.type;
							myAction.plate_barcode = myRobotNest.plate_barcode;
							myAction.nest_num = nest.nest_num;

							nest.plate_barcode = myRobotNest.plate_barcode;                          //update shelf information
							nest.type = myRobotNest.type;
							currentTask.instrumentID = targetDevice.lm_number;

							myRobotNest.plate_barcode = null;                            //update robot nest with null values
							myRobotNest.type = null;

							transferBreak = true;
							break;
						}
					}
					if (transferBreak == true) break;
				}
			}			
			else
			{
				int targetNestNum = currentTask.reagents.Find(j => j.barcode == plateBarcode).targetNest;
				var targetNest = targetDevice.nests.Find(k => k.nest_num == targetNestNum);
				if(targetNest.plate_barcode == null)      //UpdateTaskManager has already checked that there is an instrument available with status "Ready"
				{
					myAction.isValid = true;                                        //myAction is returned from function with details from mySystemStatus.instrument
					myAction.device_name = targetDevice.device_name;
					myAction.lm_number = targetDevice.lm_number;
					myAction.plate_barcode = myRobotNest.plate_barcode;
					myAction.plate_type = myRobotNest.type;
					myAction.nest_num = targetNest.nest_num;

					targetNest.plate_barcode = myRobotNest.plate_barcode;                  //update nest information
					targetNest.type = myRobotNest.type;
					currentTask.instrumentID = targetDevice.lm_number;       //set the taskManager instrument ID to wherever the plate was just put
					currentTask.reagents.Find(l => l.barcode == plateBarcode).consumed = true;      //signifies that the reagent was actually put on the instrument deck

					myRobotNest.plate_barcode = null;                                //update robot nest with null values
					myRobotNest.type = null;
				}
			}
				
			//write JSON file                
			string JSONOutput = Globals.ser.Serialize(mySystemStatus);
			WriteJson(Globals.directory + "SystemStatus.json", JSONOutput);

			//write JSON file               
			JSONOutput = Globals.ser.Serialize(myTaskManager);
			WriteJson(Globals.directory + "TaskManager.json", JSONOutput);

			return myAction;
		}

		public static object GetPlateOnSystem(string jobID, string plateBarcode)
		{
			string systemStatusText = OpenJson(Globals.directory + "SystemStatus.json");
			SystemInventory mySystemStatus = Globals.ser.Deserialize<SystemInventory>(systemStatusText); //deserialize system status into SystemInventory object
			SystemAction myAction = new SystemAction();
			myAction.isValid = false;   //set to true when plateBarcode is found
			myAction.action = "getPlate";

			var myRobotNest = mySystemStatus.devices.Find(p => p.device_type == "Robot").nests[0];

			foreach (var device in mySystemStatus.devices)
			{
				bool transferBreak = false;
				if (device.device_type == "Random Access")					//if the instrument is random access, we know to look at racks
				{
					foreach (var stack in device.stacks)
					{
						foreach (var nest in stack.nests)
						{
							if (nest.plate_barcode == plateBarcode)
							{
								myAction.isValid = true;							//myAction is returned from function with details from mySystemStatus.instrument
								myAction.device_name = device.device_name;
								myAction.lm_number = device.lm_number;
								myAction.stack_num = stack.stack_num;
								myAction.nest_num = nest.nest_num;
								myAction.plate_type = nest.type;
								myAction.plate_barcode = plateBarcode;

								myRobotNest.plate_barcode = nest.plate_barcode;			//update robot nest with current plate information
								myRobotNest.type = nest.type;

								nest.plate_barcode = null;							//update shelf information
								nest.type = null;

								transferBreak = true;
								break;
							}
						}
						if (transferBreak == true) break;
					}
					if (transferBreak == true) break;
				}
				else
				{
					foreach (var nest in device.nests)                              //if the instrument is NOT random access, we know to look at nests
					{
						if (nest.plate_barcode == plateBarcode && device.status == "Idle")
						{
							myAction.isValid = true;									//myAction is returned from function with details from mySystemStatus.instrument
							myAction.device_name = device.device_name;
							myAction.lm_number = device.lm_number;
							myAction.nest_num = nest.nest_num;
							myAction.plate_type = nest.type;
							myAction.plate_barcode = plateBarcode;

							myRobotNest.plate_barcode = nest.plate_barcode;               //update robot nest with current plate information
							myRobotNest.type = nest.type;

							nest.plate_barcode = null;									//update shelf information
							nest.type = null;

							transferBreak = true;
							break;
						}
					}
				}
				if (transferBreak == true) break;
			}
			//write JSON file               
			string JSONOutput = Globals.ser.Serialize(mySystemStatus);
			WriteJson(Globals.directory + "SystemStatus.json", JSONOutput);

			return myAction;
		}

		public static string[] GetMicroserveStackToInventory(int stack)
		{
			string[] microserveInventory = new string[3];		//create an array
			microserveInventory[0] = "skip";		//set first index to 'skip'
			string labwareType = Globals.microserveLabware[stack];		//get the labware type from the global array (set from the inventory microserve dialog
			if (labwareType != "None")		//if the labware type is not 'none' update the three index positions of the microserveInventory array
			{
				microserveInventory[0] = (stack + 1).ToString();		//stack number
				microserveInventory[1] = labwareType;		//labware type
				microserveInventory[2] = "0";		//gets updated from 0 to the actual count after the microserve counts the labware in the stack
			}
			return microserveInventory;
		}

		public static string[] ReagentsNeededForTask(string jobID)
		{
			//open task manager
			string taskManagerText = OpenJson(Globals.directory + "TaskManager.json");
			JobQueue myTaskManager = Globals.ser.Deserialize<JobQueue>(taskManagerText); //deserialize task manager into JobQueue object

			List<string> reagentBarcodeList = new List<string>();
			var currentTask = myTaskManager.jobs.Find(p => p.jobID == jobID).tasks.Find(a => a.status == "In Progress");
			foreach(var reagent in currentTask.reagents)
			{
				foreach (var validatedReagent in myTaskManager.jobs.Find(p => p.jobID == jobID).reagents.Where(a => a.type == reagent.type))
				{
					if (validatedReagent.consumed != true) //if it hasn't been consumed, add it to the barcode list for the get and put step
					{
						reagent.barcode = validatedReagent.barcode;
						reagentBarcodeList.Add(validatedReagent.barcode);
						if(validatedReagent.type != "Assay Plate") validatedReagent.consumed = true;   //mark the job's reagent as consumed so that it isn't used again for another task
						break;
					}
				}
			}
			//write JSON file               
			string JSONOutput = Globals.ser.Serialize(myTaskManager);
			WriteJson(Globals.directory + "TaskManager.json", JSONOutput);

			string[] reagentsNeeded = reagentBarcodeList.ToArray();
			return reagentsNeeded;
		}

		public static string[] LabwareNeededForTask(string jobID)
		{
			//open task manager
			string taskManagerText = OpenJson(Globals.directory + "TaskManager.json");
			JobQueue myTaskManager = Globals.ser.Deserialize<JobQueue>(taskManagerText); //deserialize task manager into JobQueue object

			List<string> labwareTypeList = new List<string>();
			var currentTask = myTaskManager.jobs.Find(p => p.jobID == jobID).tasks.Find(a => a.status == "In Progress");
			foreach (var labware in currentTask.labware) labwareTypeList.Add(labware.type);

			string[] labwareNeeded = labwareTypeList.ToArray();
			return labwareNeeded;
		}

		public static string[] LabwareNeededForJob(string jobID)
		{
			//open task manager
			string taskManagerText = OpenJson(Globals.directory + "TaskManager.json");
			JobQueue myTaskManager = Globals.ser.Deserialize<JobQueue>(taskManagerText); //deserialize task manager into JobQueue object

			List<string> labwareTypeList = new List<string>();
			var currentJob = myTaskManager.jobs.Find(p => p.jobID == jobID);
			foreach (var task in currentJob.tasks.Where(p => p.status != "Complete"))	//check all tasks that are not complete (any that are 'In Progress' or 'Queued')
			{
				foreach (var labware in task.labware) labwareTypeList.Add(labware.type);		//add their labware to the array
			}

			string[] labwareNeeded = labwareTypeList.ToArray();
			return labwareNeeded;
		}

		public static void InventoryMicroserveStack(string[] stackData)
		{
			//open System Status
			string systemStatusText = OpenJson(Globals.directory + "SystemStatus.json");
			SystemInventory mySystemStatus = Globals.ser.Deserialize<SystemInventory>(systemStatusText); //deserialize system status into SystemInventory object

			Stack myStack = new Stack();
			myStack.stack_num = Convert.ToInt16(stackData[0]);
			myStack.type = stackData[1];
			myStack.count = Convert.ToInt16(stackData[2]);

			var myMicroserve = mySystemStatus.devices.Find(p => p.device_name == "Microserve");
			int currentLabwareStackIndex = myMicroserve.stacks.FindIndex(p => p.stack_num == myStack.stack_num);	//see if the target stack already exists in the inventory (if it doesn't, index will be -1)
			Console.WriteLine("currentLabwareStackIndex: " + currentLabwareStackIndex);
			if (currentLabwareStackIndex != -1) myMicroserve.stacks[currentLabwareStackIndex] = myStack;		//replace the stack data at that index position with the new data 
			else myMicroserve.stacks.Add(myStack);  //put the stack at the end of the inventory list

			//write JSON file               
			string JSONOutput = Globals.ser.Serialize(mySystemStatus);
			WriteJson(Globals.directory + "SystemStatus.json", JSONOutput);
		}

		public static object GetLabwareFromMicroserve(string jobID, string labwareType)
		{
			string systemStatusText = OpenJson(Globals.directory + "SystemStatus.json");
			SystemInventory mySystemStatus = Globals.ser.Deserialize<SystemInventory>(systemStatusText); //deserialize system status into SystemInventory object
			SystemAction myAction = new SystemAction();
			myAction.isValid = false;   //set to true if there is labware of the targetType in the microserve, else, false will trigger a new microserve inventory
			myAction.action = "getLabwareFromMicroserve";

			var myRobotNest = mySystemStatus.devices.Find(p => p.device_type == "Robot").nests[0];      //select the robot nest
			var targetDevice = mySystemStatus.devices.Find(p => p.device_name == "Microserve");			//select the target device
			var myMicroserveStack = targetDevice.stacks.Find(k => k.type == labwareType);       //select the microserve stack for the target labware

			myAction.device_name = targetDevice.device_name;
			myAction.lm_number = targetDevice.lm_number;
			myAction.plate_type = labwareType;

			if (myMicroserveStack != null && myMicroserveStack.count > 0)		//if the microserve stack exists and has more than 0 count
			{
				myAction.isValid = true;                            //myAction is returned from function with details from mySystemStatus.instrument
				myAction.stack_num = myMicroserveStack.stack_num;

				myRobotNest.type = labwareType;         //update robot nest with current plate information
				myMicroserveStack.count--;                          //update plate count information

				//write JSON file               
				string JSONOutput = Globals.ser.Serialize(mySystemStatus);
				WriteJson(Globals.directory + "SystemStatus.json", JSONOutput);
			}
			return myAction;
		}

		public static object PutLabwareOnSystem(string jobID, string labwareType)
		{
			//open task manager
			string taskManagerText = OpenJson(Globals.directory + "TaskManager.json");
			JobQueue myTaskManager = Globals.ser.Deserialize<JobQueue>(taskManagerText); //deserialize task manager into JobQueue object
			var currentTask = myTaskManager.jobs.Find(p => p.jobID == jobID).tasks.Find(a => a.status == "In Progress");

			//open System Status
			string systemStatusText = OpenJson(Globals.directory + "SystemStatus.json");
			SystemInventory mySystemStatus = Globals.ser.Deserialize<SystemInventory>(systemStatusText); //deserialize system status into SystemInventory object

			SystemAction myAction = new SystemAction();
			myAction.isValid = false;   //set to true when plateBarcode is found
			myAction.action = "putLabware";

			var targetDevice = mySystemStatus.devices.Find(p => p.lm_number == currentTask.instrumentID);
			var myRobotNest = mySystemStatus.devices.Find(p => p.device_type == "Robot").nests[0];

			int targetNestNum = currentTask.labware.Find(j => j.type == labwareType).targetNest;
			var targetNest = targetDevice.nests.Find(k => k.nest_num == targetNestNum);
			if (targetNest.plate_barcode == null && targetDevice.status == "Ready")
			{
				myAction.isValid = true;                                        //myAction is returned from function with details from mySystemStatus.instrument
				myAction.device_name = targetDevice.device_name;
				myAction.lm_number = targetDevice.lm_number;
				myAction.plate_type = myRobotNest.type;
				myAction.nest_num = targetNest.nest_num;

				targetNest.type = myRobotNest.type;

				currentTask.instrumentID = targetDevice.lm_number;       //set the taskManager instrument ID to wherever the plate was just put
				currentTask.labware.Find(l => l.type == labwareType).consumed = true;      //signifies that the labware was actually put on the instrument deck

				myRobotNest.plate_barcode = null;                                //update robot nest with null values
				myRobotNest.type = null;
			}

			//write JSON file                
			string JSONOutput = Globals.ser.Serialize(mySystemStatus);
			WriteJson(Globals.directory + "SystemStatus.json", JSONOutput);

			//write JSON file               
			JSONOutput = Globals.ser.Serialize(myTaskManager);
			WriteJson(Globals.directory + "TaskManager.json", JSONOutput);

			return myAction;
		}

		public static object SetupTask(string jobID)
		{
			//open task manager
			string taskManagerText = OpenJson(Globals.directory + "TaskManager.json");
			JobQueue myTaskManager = Globals.ser.Deserialize<JobQueue>(taskManagerText); //deserialize task manager into JobQueue object
			SystemAction myAction = new SystemAction();
			myAction.isValid = false;   //set to true when plateBarcode is found
			myAction.action = "setup";

			var targetTask = myTaskManager.jobs.Find(p => p.jobID == jobID).tasks.Find(a => a.status == "In Progress"); //find the task object that is in progress for the current job
			myAction.task = targetTask.task;
			myAction.device_name = targetTask.device_name;
			myAction.lm_number = targetTask.instrumentID;

			return myAction;
		}

		public static object TeardownTask(string jobID)
		{
			//open System Status
			string systemStatusText = OpenJson(Globals.directory + "SystemStatus.json");
			SystemInventory mySystemStatus = Globals.ser.Deserialize<SystemInventory>(systemStatusText); //deserialize system status into SystemInventory object

			//open task manager
			string taskManagerText = OpenJson(Globals.directory + "TaskManager.json");
			JobQueue myTaskManager = Globals.ser.Deserialize<JobQueue>(taskManagerText); //deserialize task manager into JobQueue object
			SystemAction myAction = new SystemAction();

			myAction.isValid = false;   //set to true if there is a plate to teardown
			myAction.action = "teardown";

			int index = myTaskManager.jobs.Find(p => p.jobID == jobID).tasks.FindIndex(a => a.status == "In Progress"); //find the index of the task object that is in progress for the current 
			string targetDevice = myTaskManager.jobs.Find(p => p.jobID == jobID).tasks[index - 1].instrumentID;   //get the device of the task just before the one in progress
			

			var loadedNest = mySystemStatus.devices.Find(p => p.lm_number == targetDevice).nests.Find(k => k.type != null);
			if (loadedNest != null)
			{
				myAction.isValid = true;
				myAction.lm_number = targetDevice;
				myAction.nest_num = loadedNest.nest_num;
				myAction.plate_barcode = loadedNest.plate_barcode;
				myAction.plate_type = loadedNest.type;

				loadedNest.plate_barcode = null;
				loadedNest.type = null;
				loadedNest.workflow_id = null;
			}
			else
			{
				mySystemStatus.devices.Find(p => p.lm_number == targetDevice).status = "Ready";
				myAction.isValid = false;
			}

			//write JSON file               
			string JSONOutput = Globals.ser.Serialize(mySystemStatus);
			WriteJson(Globals.directory + "SystemStatus.json", JSONOutput);

			return myAction;
		}

		public static object RunTask(string jobID)
		{
			//open task manager
			string taskManagerText = OpenJson(Globals.directory + "TaskManager.json");
			JobQueue myTaskManager = Globals.ser.Deserialize<JobQueue>(taskManagerText); //deserialize task manager into JobQueue object
			SystemAction myAction = new SystemAction();
			myAction.isValid = false;   //set to true when plateBarcode is found
			myAction.action = "run";

			var targetTask = myTaskManager.jobs.Find(p => p.jobID == jobID).tasks.Find(a => a.status == "In Progress"); //find the task object that is in progress for the current job
			myAction.task = targetTask.task;
			myAction.device_name = targetTask.device_name;
			myAction.lm_number = targetTask.instrumentID;

			return myAction;
		}
	}
}
