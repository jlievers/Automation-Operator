using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Net;
using System.Web.Script.Serialization;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        string LIMSEndpoint = "weblims-balancer-lims-dev-joy-foster-lims-qa.sqa.locusdev.net";
        string  SystemFolderPath = "C:\\Test";
        string RunID = "test";

        //private void button1_Click(object sender, EventArgs e)
        //{
        //    string[] myBarcodes = new string[] { "IB4838765", "XXX", "CCC" };
        //    List<string> nonValid = RNA_LIMS_COM.LIMSCalls.validateIBs(myBarcodes);
        //    MessageBox.Show(String.Join(",", nonValid.ToArray()));
        //}

        //LIMS provides these
        private string extractionBatchingWorkflow = "";
        private string extractionWorkflow = "";
        private string quantWorkflow = "";
        private string normWorkflow = "";
        private string retrieveNStampWorkflow = "";
        private string normWorklistName = "";
        //Venus provides these
        private string tuberackID = "RCK123";
        private string LMNum = "LM8820";
        private string picoDyePlate = "PG511642";
        private string picoStandards = "PG511648";
        private string dataTableName = "datatable";
        private string flourostarFileName = "readerfile";
        private string assayPlateId = "PG511664";


        private void button9_Click(object sender, EventArgs e)
        {
            extractionBatchingWorkflow = RNA_LIMS_COM.LIMSCalls.InitExtractionBatching(LIMSEndpoint, SystemFolderPath, RunID, tuberackID, LMNum, dataTableName);
            MessageBox.Show("done");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            extractionWorkflow = RNA_LIMS_COM.LIMSCalls.InitExtraction(LIMSEndpoint, SystemFolderPath, RunID, extractionBatchingWorkflow, tuberackID, LMNum, dataTableName, extractionWorkflow);
            MessageBox.Show("done");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            RNA_LIMS_COM.LIMSCalls.CompleteExtraction(LIMSEndpoint, SystemFolderPath, RunID, extractionWorkflow);
            MessageBox.Show("done");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            quantWorkflow = RNA_LIMS_COM.LIMSCalls.InitQuantification(LIMSEndpoint, SystemFolderPath, RunID, extractionWorkflow, tuberackID, LMNum, picoDyePlate,picoStandards, quantWorkflow);
            MessageBox.Show("done");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            RNA_LIMS_COM.LIMSCalls.CompleteQuantification(LIMSEndpoint, SystemFolderPath, RunID, quantWorkflow,  flourostarFileName);
            MessageBox.Show("done");
        }

        private void button5_Click(object sender, EventArgs e)
        {
            normWorkflow = RNA_LIMS_COM.LIMSCalls.InitNormalization(LIMSEndpoint, SystemFolderPath, RunID, quantWorkflow, tuberackID, LMNum, normWorkflow);
            MessageBox.Show("done");
        }

        private void button10_Click(object sender, EventArgs e)
        {
            normWorklistName = RNA_LIMS_COM.LIMSCalls.GetNormalizationWorklist(LIMSEndpoint, SystemFolderPath, RunID, normWorkflow);
            MessageBox.Show("done");
        }

        private void button6_Click(object sender, EventArgs e)
        {
            RNA_LIMS_COM.LIMSCalls.CompleteNormalization(LIMSEndpoint, SystemFolderPath, RunID, normWorkflow);
            MessageBox.Show("done");
        }

        private void button7_Click(object sender, EventArgs e)
        {
            retrieveNStampWorkflow = RNA_LIMS_COM.LIMSCalls.InitRetrieveAndStamp(LIMSEndpoint, SystemFolderPath, RunID, normWorkflow, tuberackID, LMNum, assayPlateId, retrieveNStampWorkflow);
            MessageBox.Show("done");
        }

        private void button8_Click(object sender, EventArgs e)
        {
            RNA_LIMS_COM.LIMSCalls.CompleteRetrieveAndStamp(LIMSEndpoint, SystemFolderPath, RunID, retrieveNStampWorkflow);
            MessageBox.Show("done");
        }
    }
}

