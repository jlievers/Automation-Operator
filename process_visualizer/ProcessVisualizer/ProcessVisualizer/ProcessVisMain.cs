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
using System.Web.Script.Serialization;
using System.Web;
using System.Net;
using Newtonsoft.Json.Linq;
//REF "C:\Program Files (x86)\PAA\Overlord3\DLL_RunOutput.dll"
//REF "C:\Program Files (x86)\PAA\Overlord3\DLL_LIMS_Workflow.dll"
//REF "C:\Program Files (x86)\PAA\Overlord3\DLL_Http.dll"

namespace ProcessVisualizer
{
    //Schedule Stuff
    public class Instrument
    {
        public int index { get; set; }
        public string name { get; set; }
    }

    public class Step
    {
        public int end_time { get; set; }
        public List<Instrument> instrument { get; set; }
        public int plate_index { get; set; }
        public string plate_name { get; set; }
        public string process { get; set; }
        public int start_time { get; set; }
        public string step { get; set; }
    }

    public class Run
    {
        public int current_step { get; set; }
        public List<Step> schedule { get; set; }
    }

    //Process Model Stuff
    ///////////////////////////////////////////////////////////////////////////
    
    public class ProcessModel
    {
        public List<PMStep> step { get; set; }
        public List<Process> process { get; set; }
        public List<PMInstrument> instrument { get; set; }
        public List<ProcessDemand> process_demand { get; set; }

    }

    public class PMStep
    {
        public string name { get; set; }
        public List<AssignmentOption> assignment_option { get; set; }
    }

    public class AssignmentOption
    {
        public List<string> instrument { get; set; }
        public int duration { get; set; }
    }

    public class Process
    {
        public string name { get; set; }
        public List<string> step { get; set; }
        public List<PrecedenceRelation> precedence_relation { get; set; }
    }

    public class PrecedenceRelation
    {
        public string step { get; set; }
        public string next_step { get; set; }
        public bool colocated { get; set; }
        public int max_gap { get; set; }
    }

    public class PMInstrument
    {
        public string name { get; set; }
        public int count { get; set; }
    }

    public class ProcessDemand
    {
        public string process { get; set; }
        public int num_plates { get; set; }
        public bool enforce_uniform_pacing { get; set; }
        public List<PartialPlate> partial_plate { get; set; }
    }

    public class PartialPlate
    {
        public string plate_name { get; set; }
        public List<Assignment> assignment { get; set; }
    }

    public class Assignment
    {
        public string step { get; set; }
        public int end_time { get; set; }
    }

    public partial class Form1 : Form
    {
        System.Web.Script.Serialization.JavaScriptSerializer ser = new System.Web.Script.Serialization.JavaScriptSerializer();
        public Form1()
        {
            InitializeComponent();
        }
        
        private void button1_Click(object sender, EventArgs e)
        {
            //Run myRun = ser.Deserialize<Run>(File.ReadAllText("C:\\Development\\labScheduler\\labSchedule.json"));
            //PMStep currentStep = myRun.schedule[myRun.current_step];
            //foreach (PMInstrument myInst in currentStep.instrument)
            //{
            //    if (myInst.name.Contains("bravo"))
            //    {
            //        int bravoIndex = myInst.index;
            //    }
            //}

            //string process = currentStep.step;
            //int plate_index = currentStep.plate_index;

        }

       
    }
}
