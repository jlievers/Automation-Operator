using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicConfirmation;
using System.Diagnostics;
using System.Threading;

namespace TestConsole
{
	class ConfirmationTestConsole
	{
		static void Main(string[] args)
		{
			Globals.systemStatus = "Idle";
			bool runMethod = true;
			bool goAhead;
			int jobLoop;			
			string[] reagentBarcodes;
			string[] labware;
			string[] microserveStackData;

			Functions.CreateBlankProtocolTaskList();		//only needed if you want a blank template for protocol task list

			DynamicConfirmation.Functions.CreateSystemStatus();
			DynamicConfirmation.Functions.CreateTaskManager();

			Process.Start(Globals.dashboardApplicationDirectory + "DashboardApplication.exe");

			while (runMethod == true)
			{
				Thread.Sleep(200);
				Console.WriteLine(Globals.systemStatus);

				DynamicConfirmation.Functions.AddJobToTaskManager();    //add any jobs from temp folder

				if (DynamicConfirmation.Functions.CountJobsInJobQueue() != 0)
				{
					Globals.systemStatus = "Running";
					Globals.inventory = true;
				}
				else Globals.systemStatus = "Idle";

				while (Globals.systemStatus == "Running")
				{
					Thread.Sleep(200);
					if (Globals.inventory == true)
					{
						InventoryMicroserve myMicroserveLabware = new InventoryMicroserve();	//instantiate the microserve labware type form and display
						myMicroserveLabware.ShowDialog();

						for(int i = 0; i < 14; i++)
						{
							microserveStackData = Functions.GetMicroserveStackToInventory(i);
							if (microserveStackData[0] != "skip")
							{
								//run microserve to count stack # microserveStackData[0] for labware type microserveStackData[1]
								string count = Microsoft.VisualBasic.Interaction.InputBox("Inventory Microserve:" + Environment.NewLine + "Rack: " + microserveStackData[0] + Environment.NewLine + "Labware: " + microserveStackData[1], "Stack Count", "0");
								microserveStackData[2] = count;
								Functions.InventoryMicroserveStack(microserveStackData); //add stack data to SystemStatus microserve inventory
							}
						}

						Globals.inventory = false;
					}

					DynamicConfirmation.Functions.AddJobToTaskManager();    //add any jobs from temp folder
					jobLoop = DynamicConfirmation.Functions.CountJobsInJobQueue();  //count number of jobs in job queue

					for (int i = 0; i < jobLoop; i++)
					{
						Console.WriteLine("checking job status on job: " + i);
						Globals.currentJobID = Functions.GetCurrentJobID(i);
						Functions.QueryAndUpdateSystemStatus();

						goAhead = Functions.CheckJobStatus(i);
						if (goAhead == true)
						{
							string actionString = Functions.UpdateTaskManager(i);
							if (actionString == "StartJob")
							{
								goAhead = false;
								while (goAhead == false)
								{
									LoadInputBuffer loadInputBufferDialog = new LoadInputBuffer();
									loadInputBufferDialog.ShowDialog(); // show user what labware they need to load

									//scan input buffer DEMO FOR TESTING
									Random rnd = new Random();
									List<string> tempReagentBarcodeList = new List<string>();
									foreach(string reagent in Globals.reagents)
									{
										string tempReagentBarcode = Microsoft.VisualBasic.Interaction.InputBox("Enter a dummy barcode for the following reagent: " + reagent, "Input Buffer Scan", "PG" + rnd.Next(1, 999).ToString());
										tempReagentBarcodeList.Add(tempReagentBarcode);
									}
									
									reagentBarcodes = tempReagentBarcodeList.ToArray();

									goAhead = Functions.StartJob(i, reagentBarcodes);
									//move plates into system
									if (goAhead == true)
									{
										for (int j = 0; j < reagentBarcodes.Length; j++)
										{
											var getPlate = Functions.GetPlateOnSystem(Globals.currentJobID, reagentBarcodes[j]) as SystemAction;

											MessageBox.Show("Getting Plate " + getPlate.plate_barcode + " from " + getPlate.device_name + "(" + getPlate.lm_number + ")" + Environment.NewLine + 
												"Stack: " + getPlate.stack_num + Environment.NewLine + "Nest: " + getPlate.nest_num + Environment.NewLine + Environment.NewLine + "Plate Type: " + getPlate.plate_type);

											var putPlate = Functions.PutPlateOnSystem(Globals.currentJobID, reagentBarcodes[j]) as SystemAction;

											MessageBox.Show("Putting Plate " + putPlate.plate_barcode + " to " + putPlate.device_name + "(" + putPlate.lm_number + ")" + Environment.NewLine +
												"Stack: " + putPlate.stack_num + Environment.NewLine + "Nest: " + putPlate.nest_num + Environment.NewLine + Environment.NewLine + "Plate Type: " + putPlate.plate_type);
										}
									}
								}
							}
							else if(actionString != "") //actionString should be WFXXXX
							{
								reagentBarcodes = Functions.ReagentsNeededForTask(Globals.currentJobID);
								for (int j = 0;j < reagentBarcodes.Length; j++)
								{
									var getPlate = Functions.GetPlateOnSystem(Globals.currentJobID, reagentBarcodes[j]) as SystemAction;
									MessageBox.Show("Getting Plate " + getPlate.plate_barcode + " from " + getPlate.device_name + "(" + getPlate.lm_number + ")" + Environment.NewLine +
										"Stack: " + getPlate.stack_num + Environment.NewLine + "Nest: " + getPlate.nest_num + Environment.NewLine + Environment.NewLine + "Plate Type: " + getPlate.plate_type);
									////////

									var putPlate = Functions.PutPlateOnSystem(Globals.currentJobID, reagentBarcodes[j]) as SystemAction;
									MessageBox.Show("Putting Plate " + putPlate.plate_barcode + " to " + putPlate.device_name + "(" + putPlate.lm_number + ")" + Environment.NewLine +
										"Stack: " + putPlate.stack_num + Environment.NewLine + "Nest: " + putPlate.nest_num + Environment.NewLine + Environment.NewLine + "Plate Type: " + putPlate.plate_type);
									////////
								}

								labware = Functions.LabwareNeededForTask(Globals.currentJobID);
								for (int p = 0; p < labware.Length; p++)
								{
									goAhead = false;
									while(goAhead == false)	//set to true if the labware could be found in the microserve and it's count isn't 0
									{
										Console.WriteLine("checking for labware");
										var getLabware = Functions.GetLabwareFromMicroserve(Globals.currentJobID, labware[p]) as SystemAction;
										if(getLabware.isValid == true)
										{
											goAhead = true;		//set to True, labware was found
											MessageBox.Show("Getting Labware " + getLabware.plate_type + " from " + getLabware.device_name + "(" + getLabware.lm_number + ")" + Environment.NewLine +
												"Stack: " + getLabware.stack_num);
										}
										else	//the labware couldn't be found
										{
											string[] tempListJobLabware = Functions.LabwareNeededForJob(Globals.currentJobID);	//check what labware is still needed to finish this job
											var labwareTypeCount = from type in tempListJobLabware
												group type by type into g
												select new { g.Key, Count = g.Count() };
											string tempLabwareString = "";
											foreach (var type in labwareTypeCount) tempLabwareString = tempLabwareString + type.Key + " (" + type.Count + ")" + Environment.NewLine;	//get the type and number of each labware needed for the job

											MessageBox.Show("Could not locate the following Labware: " + getLabware.plate_type + Environment.NewLine + "Storage Device: " + getLabware.device_name + " (" + getLabware.lm_number + ")" + Environment.NewLine + Environment.NewLine +
												"The following labware are required to finish " + Globals.currentJobID + Environment.NewLine + tempLabwareString + Environment.NewLine + 
												"Click 'OK' to initiate the inventory.", "Error - Labware not available", MessageBoxButtons.OK, MessageBoxIcon.Error);

											InventoryMicroserve myMicroserveLabware = new InventoryMicroserve();    //instantiate the microserve labware type form and display
											myMicroserveLabware.ShowDialog();

											for (int j = 0; j < 14; j++)
											{
												microserveStackData = Functions.GetMicroserveStackToInventory(j);
												if (microserveStackData[0] != "skip")
												{
													//run microserve to count stack # microserveStackData[0] for labware type microserveStackData[1]
													string count = Microsoft.VisualBasic.Interaction.InputBox("Inventory Microserve:" + Environment.NewLine + "Rack: " + microserveStackData[0] + Environment.NewLine + "Labware: " + microserveStackData[1], "Stack Count", "0");
													microserveStackData[2] = count;
													Functions.InventoryMicroserveStack(microserveStackData); //add stack data to SystemStatus microserve inventory
												}
											}
										}
									}

									var putLabware = Functions.PutLabwareOnSystem(Globals.currentJobID, labware[p]) as SystemAction;
									MessageBox.Show("Putting Labware " + putLabware.plate_type + " to " + putLabware.device_name + "(" + putLabware.lm_number + ")" + Environment.NewLine +
										"Nest: " + putLabware.nest_num);
									////////
								}

								////////
								var setupTask = Functions.SetupTask(actionString) as SystemAction;
								if (setupTask.device_name != "Static Stacker") Functions.SetInstrumentStatus(setupTask.lm_number, "Loaded");  //set instrument status after load is complete (not if its a static stacker)
								///

								var runTask = Functions.RunTask(actionString) as SystemAction;
								MessageBox.Show("Running " + runTask.task + " on " + runTask.device_name + "(" + runTask.lm_number + ")");
								////////

								goAhead = true;
								while (goAhead == true)
								{
									var teardownTask = Functions.TeardownTask(actionString) as SystemAction;
									MessageBox.Show("Teardown Required: " + teardownTask.isValid + Environment.NewLine + "Target Instrument: " + teardownTask.lm_number + Environment.NewLine +
										"Target Nest: " + teardownTask.nest_num + Environment.NewLine + "Plate Barcode: " + teardownTask.plate_barcode);
									goAhead = teardownTask.isValid;
								}
								////////
							}
						}
					}

					Functions.RemoveCompletedJobsFromQueue();
					if (DynamicConfirmation.Functions.CountJobsInJobQueue() == 0) Globals.systemStatus = "Idle";
				}
							   				 			  
			}
		}
	}
}
