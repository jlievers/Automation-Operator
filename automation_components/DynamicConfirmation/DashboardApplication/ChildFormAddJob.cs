using System;
using System.IO;
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
	public partial class ChildFormAddJob : Form
	{
		public ChildFormAddJob()
		{
			InitializeComponent();

			string protocolTaskListText = Functions.OpenJson(Globals.supportingFilesDirectory + "ProtocolTaskList.json");
			ProtocolTaskList myMasterProtocolTaskList = Globals.ser.Deserialize<ProtocolTaskList>(protocolTaskListText); //deserialize task manager into JobQueue object
			List<string> protocolList = new List<string>();
			foreach (var protocol in myMasterProtocolTaskList.protocols) protocolList.Add(protocol.protocol_name);

			protocolList.Sort();
			foreach (string protocol in protocolList) protocolComboBox.Items.Add(protocol);
			//open user_list.csv, add all of the users from the csv to the drop down list
			var data = Functions.CsvToArray(Globals.supportingFilesDirectory + "user_list.csv");
			var userlist = new List<string>();                                                      //create list of users
			for (int i = 0; i < data.Length; i++) userlist.Add(data[i][0]);
			userlist.Sort();                                                                        //sort the user list alphabetically
			foreach (string user in userlist) userComboBox.Items.Add(user);                         //add users to combo box

		}

		private void addJob_Click(object sender, EventArgs e)
		{
			if (protocolComboBox.Text == null || protocolComboBox.Text == "")
			{
				MessageBox.Show("You failed to select a Protocol Type for this job." + System.Environment.NewLine + System.Environment.NewLine + "Make sure to select a Protocol Type in the drop down menu before hitting Add Job", "Error");
			}
			else if (wfTextBox.Text == "WFXXXX")
			{
				MessageBox.Show("You failed to scan an assay barcode." + System.Environment.NewLine + System.Environment.NewLine + "Make sure to scan the Assay Barcode before hitting Add Job", "Error");
			}
			else if (userComboBox.Text == null || userComboBox.Text == "")
			{
				MessageBox.Show("You failed to select a User Name for this job." + System.Environment.NewLine + System.Environment.NewLine + "Make sure to select a User in the drop down menu before hitting Add Job", "Error");
			}
			else
			{
				string priority;
				if (radioButton1.Checked == true) priority = "priority";
				else priority = "standard";
				Functions.AddJobToTempFolder(protocolComboBox.Text, userComboBox.Text, wfTextBox.Text, priority);
				this.Close();
			}
		}

		private void cancelAddJob_Click(object sender, EventArgs e)
		{
			this.Close();
		}
	}
}