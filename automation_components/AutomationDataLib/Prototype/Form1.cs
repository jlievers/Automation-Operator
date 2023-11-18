using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Prototype
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //AutomationDataLib.AutoDataTableAdapters.DeviceTableAdapter myAda = new AutomationDataLib.AutoDataTableAdapters.DeviceTableAdapter();
            //AutomationDataLib.AutoData.DeviceDataTable myTable = new AutomationDataLib.AutoData.DeviceDataTable();
            //myAda.FillBy_AvailableDeviceType(myTable, 1,3);
            //dataGridView1.DataSource = myTable;

            //AutomationDataLib.AutoDataTableAdapters.SystemTableAdapter myAda2 = new AutomationDataLib.AutoDataTableAdapters.SystemTableAdapter();
            //myAda2.Insert("MSG", "LM12345");
            //AutomationDataLib.AutoData.SystemDataTable myTable2 = new AutomationDataLib.AutoData.SystemDataTable();
            //myAda2.Fill(myTable2);
            //dataGridView1.DataSource = myTable2;


            //AutomationDataLib.AutoDataTableAdapters.Device_TypeTableAdapter myAda3 = new AutomationDataLib.AutoDataTableAdapters.Device_TypeTableAdapter();
            //int jeff = (int)myAda3.GetNumNests(3);
            //MessageBox.Show(jeff.ToString());


            //AutomationDataLib.AutoDataTableAdapters.Device_TypeTableAdapter myAda = new AutomationDataLib.AutoDataTableAdapters.Device_TypeTableAdapter();
            //AutomationDataLib.AutoData.Device_TypeDataTable myTable = new AutomationDataLib.AutoData.Device_TypeDataTable();
            //myAda.(myTable, 1);
            //dataGridView1.DataSource = myTable;

            AutomationDataLib.AutoDataTableAdapters.DeviceTableAdapter myAda = new AutomationDataLib.AutoDataTableAdapters.DeviceTableAdapter();

            int jeff = (int)myAda.InsertQuery("Lynx6", 3, true, "LM55535", true, 1);
            MessageBox.Show(jeff.ToString());
        }

    }
}
