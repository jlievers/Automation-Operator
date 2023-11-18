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
	public partial class ChildFormInstrumentStatus : Form
	{
		public ChildFormInstrumentStatus()
		{
			InitializeComponent();
			InstrumentStatusData();
		}

		public void InstrumentStatusData()
		{
			string systemStatusText = Functions.OpenJson(Globals.directory + "SystemStatus.json");
			SystemInventory mySystemStatus = Globals.ser.Deserialize<SystemInventory>(systemStatusText); //deserialize task manager into JobQueue object

			BindingSource bs = new BindingSource();                    //bind the systemStatus data grid to the activeSystemStatus json object created above
			bs.DataSource = mySystemStatus;
			instrumentStatusGrid.DataSource = bs;
			instrumentStatusGrid.AutoGenerateColumns = true;
		}

		private void fileSystemWatcher1_Changed(object sender, System.IO.FileSystemEventArgs e)
		{
			System.Threading.Thread.Sleep(500);
			InstrumentStatusData();
		}
	}
}