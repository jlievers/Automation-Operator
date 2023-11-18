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
	public partial class InventoryMicroserve : Form
	{
		public InventoryMicroserve()
		{
			InitializeComponent();
			AddLabwareToComboBox();
		}

		public void AddLabwareToComboBox()
		{
			foreach (var comboBox in this.Controls.OfType<ComboBox>())
			{
				comboBox.Text = "None";
				comboBox.Items.AddRange(Globals.microserveLabwareTypes);
			}
		}

		private void inventory_Click(object sender, EventArgs e)
		{
			List<string> templist = new List<string>();
			foreach (var comboBox in this.Controls.OfType<ComboBox>())
			{
				templist.Add(comboBox.Text);
			}
			Globals.microserveLabware = templist.ToArray();
			this.Close();
		}
	}
}
