using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Security.Permissions;

namespace DynamicConfirmation
{
	public partial class MainForm : Form
	{
		public MainForm()
		{
			InitializeComponent();

			systemStatusLabel.Text = "Running";
			systemStatusTextColor("Running");

			jobSelectedTextBox.Text = "";
			gridViewData();
		}

		private void systemStatusTextColor(string status)
		{
			if (status == "Running") systemStatusLabel.BackColor = Color.LightGreen;
			else if (status == "Paused") systemStatusLabel.BackColor = Color.LightGoldenrodYellow;
			else if (status == "Idle") systemStatusLabel.BackColor = Color.LightBlue;
		}

		public void gridViewData()
		{
			using (var fileStream = new FileStream(Globals.directory + "TaskManager.json", FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
			using (var textReader = new StreamReader(fileStream))
			{
				string taskManagerText = textReader.ReadToEnd();
				JobQueue myTaskManager = Globals.ser.Deserialize<JobQueue>(taskManagerText); //deserialize task manager into JobQueue object

				List<TaskList> activeTaskList = new List<TaskList>();       //create task list with the task from each job that is currently in progress
				foreach (var job in myTaskManager.jobs)
				{
					TaskList activeTask = new TaskList();
					var currentTask = job.tasks.Find(p => p.status != "Complete");
					activeTask = currentTask;
					activeTaskList.Add(activeTask);
				}

				BindingSource bs = new BindingSource();                     //bind the jobQueue data grid to the JobQueue json object
				bs.DataSource = myTaskManager;
				jobQueueGrid.DataSource = bs;
				jobQueueGrid.AutoGenerateColumns = true;

				BindingSource bs2 = new BindingSource();                    //bind the taskQueue data grid to the activeTaskList json object created above
				bs2.DataSource = activeTaskList;
				taskQueueGrid.DataSource = bs2;
				taskQueueGrid.AutoGenerateColumns = true;

				bs.ResetBindings(true);
				bs2.ResetBindings(true);
			}
		}

		private int calcTaskComplete(string jobSelected)
		{
			int percentTasksCompleted = 0;

			using (var fileStream = new FileStream(Globals.directory + "TaskManager.json", FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
			using (var textReader = new StreamReader(fileStream))
			{
				string taskManagerText = textReader.ReadToEnd();
				JobQueue myTaskManager = Globals.ser.Deserialize<JobQueue>(taskManagerText); //deserialize task manager into JobQueue object				

				foreach (var job in myTaskManager.jobs)
				{
					if (job.jobID == jobSelected)
					{
						int tasksCompleted = job.tasks.Where(p => p.status == "Complete").Count();   //calculate the number of tasks completed for the currently selected job
						int totalTasks = job.tasks.Count();
						percentTasksCompleted = Convert.ToInt16(100 * (float)tasksCompleted / totalTasks);
					}
				}
				return percentTasksCompleted;
			}
		}

		private void jobQueueGrid_CellClick(object sender, DataGridViewCellEventArgs e)
		{
			string protocolSelected = jobQueueGrid.Rows[e.RowIndex].Cells["description"].FormattedValue.ToString();
			string jobID = jobQueueGrid.Rows[e.RowIndex].Cells["jobID"].FormattedValue.ToString();

			jobSelectedTextBox.Text = protocolSelected + "_" + jobID;
			progressBar1.Value = calcTaskComplete(jobID);
		}

		private void addJob_Click(object sender, EventArgs e)
		{
			ChildFormAddJob newJob = new ChildFormAddJob();
			newJob.ShowDialog();
		}

		private void pause_Click(object sender, EventArgs e)
		{
			if (pause.Text == "Pause")
			{
				pause.Text = "Continue";
				pause.BackColor = System.Drawing.Color.LightSeaGreen;
				Globals.systemStatus = "Paused";
			}
			else if (pause.Text == "Continue")
			{
				pause.Text = "Pause";
				pause.BackColor = System.Drawing.Color.Khaki;
				Globals.systemStatus = "Running";
			}
			systemStatusLabel.Text = Globals.systemStatus;
			systemStatusTextColor(Globals.systemStatus);
		}

		private void killJob_Click(object sender, EventArgs e)
		{
			ChildFormKillJob killJob = new ChildFormKillJob();
			killJob.ShowDialog();
		}

		private void instrumentStatusDashboard_Click(object sender, EventArgs e)
		{
			//
		}

		private void fileSystemWatcher1_Changed(object sender, FileSystemEventArgs e)
		{
			System.Threading.Thread.Sleep(200);
			gridViewData();
		}

		private void fileSystemWatcher1_Created(object sender, FileSystemEventArgs e)
		{
			System.Threading.Thread.Sleep(200);
			gridViewData();
		}

	}
}
