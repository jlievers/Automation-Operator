using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DynamicConfirmation
{
	public partial class LoadInputBuffer : Form
	{
		public LoadInputBuffer()
		{
			InitializeComponent();

			string taskManagerText = Functions.OpenJson(Globals.directory + "TaskManager.json");
			JobQueue myTaskManager = Globals.ser.Deserialize<JobQueue>(taskManagerText); //deserialize task manager into JobQueue object

			List<string> reagentList = new List<string>();
			List<string> reagentsTemp = new List<string>(); //populate to set as global reagent array - dialogs will appear in test console to create dummy barcodes for appropriate plates

			foreach(var task in myTaskManager.jobs.Find(p => p.jobID == Globals.currentJobID).tasks)
			{
				foreach(var reagent in task.reagents) reagentList.Add(reagent.type);
			}

			string reagentListString = "";
			foreach (var labwareType in Globals.reagentTypes)
			{
				int labwareCount = 0;
				foreach (string reagent in reagentList)
				{
					if (reagent == labwareType)
					{
						labwareCount++;
					}
				}
				if (labwareCount != 0)
				{
					if (labwareType == "Assay Plate") labwareCount = 1;
					reagentListString = reagentListString + "(" + labwareCount + ") " + labwareType + Environment.NewLine;
					for(int i = 0; i < labwareCount; i++)
					{
						reagentsTemp.Add(labwareType);
					}
				}
			}
			labwareListLabel.Text = reagentListString;
			Globals.reagents = reagentsTemp.ToArray();
		}

		private void inventory_Click(object sender, EventArgs e)
		{
			this.Close();
		}
	}
}
