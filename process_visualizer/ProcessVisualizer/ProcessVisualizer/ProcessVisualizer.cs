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
using System.Windows.Forms.DataVisualization.Charting;
using System.Timers;
using System.Xml;

namespace ProcessVisualizer
{
    public partial class ProcessVisualizer : Form
    {
        public ProcessVisualizer()
        {
            InitializeComponent();
        }

        Dictionary<string, string> myTranslator = new Dictionary<string, string>()
        {
            {"ampure_er_setup", "01_RobotSetup_PostShrAmpAndER"},
            {"ampure_er", "02_BravoMethod_PostShrAmpureAndER"},
            {"ampure_er_teardown", "03_RobotTeardown_PostShrAmpureAndER"},
            {"er_incubation", "04_Incubation_ER"},
            {"er_to_at", "05_RobotSetup_AT"},
            {"at_incubation", "06_Incubation_AT"},
            {"ligation_setup", "07_RobotSetup_Ligation"},
            {"ligation", "08_BravoMethod_Ligation"},
            {"ligation_teardown", "09_RobotTeardown_Ligation"},
            {"ligation_incubation", "10_Incubation_Ligation"},
            {"post_ligation_ampure_pcr_setup", "11_RobotSetup_PostLigAmpAndiPCR"},
            {"post_ligation_ampure_pcr", "12_BravoMethod_PostLigAmpAndiPCR"},
            {"post_ligation_ampure_pcr_teardown", "13_RobotTeardown_PostLigAmpAndiPCR"},
            {"pcr", "14_Incubation_iPCR"},
            {"post_pcr_ampure_setup", "15_RobotSetup_iPCRAmp"},
            {"post_pcr_ampure", "16_BravoMethod_iPCRAmp"},
            {"post_pcr_ampure_teardown", "17_RobotTeardown_iPCRAmp_TO_RobotSetup_PreCapQuantNorm"},
            {"pre_cap_quant_norm", "18_Incubation_PreCapQuantNorm"},
            {"pre_cap_quant_norm_teardown", "19_RobotTeardown_PreCapQuanNorm"},
        };

        private static System.Timers.Timer myTimer = new System.Timers.Timer(1000);
        System.Web.Script.Serialization.JavaScriptSerializer ser = new System.Web.Script.Serialization.JavaScriptSerializer();

        private void button1_Click(object sender, EventArgs e)
        {
            Run myRun = ser.Deserialize<Run>(File.ReadAllText("C:\\Users\\jlievers\\Desktop\\DevelopmentTopics\\labScheduler\\Schedules\\labSchedule.json"));

            int myCurrentStepIndex = myRun.current_step;
            Step currentStepObj = myRun.schedule[myCurrentStepIndex];
            string currentPlateGroup = currentStepObj.plate_name;
            string currentStepName = currentStepObj.step;
            int currentTime = currentStepObj.start_time;
            int totalNumSteps = myRun.schedule.Count();

            Dictionary<string, string[]> plateGroupCurrentStep = new Dictionary<string, string[]>();
            foreach (Step oneStep in myRun.schedule)
            {
                if (!plateGroupCurrentStep.Keys.Contains(oneStep.plate_name))
                {
                    //index 0=step or 'step_name', index 1 = 'end_time'
                    string[] myValues = new string[] { "", "" };
                    plateGroupCurrentStep.Add(oneStep.plate_name, myValues);

                }
            }
            string[] previousValues = new string[2];
            for (int i = 0; i <= myCurrentStepIndex; i++)
            {
                Step oneStep = myRun.schedule[i];
                string endTime = Math.Max(0, (oneStep.end_time - currentTime)).ToString();
                string[] myValues = new string[] { oneStep.step, endTime };

                if (currentPlateGroup == oneStep.plate_name)
                {
                    previousValues[1] = (Convert.ToInt32(previousValues[1]) + 1).ToString();
                    plateGroupCurrentStep[oneStep.plate_name] = previousValues;
                    previousValues = myValues;
                }
                else
                {
                    plateGroupCurrentStep[oneStep.plate_name] = myValues;
                }
            }

            ProcessModel myModel = ser.Deserialize<ProcessModel>(File.ReadAllText("C:\\Users\\jlievers\\Desktop\\DevelopmentTopics\\labScheduler\\ProcessModels\\ProcessModel.json"));

        }

        private void Form3_Load(object sender, EventArgs e)
        {

            chart2.ChartAreas["ChartArea1"].AxisX.IsReversed = true;
            chart2.ChartAreas["ChartArea1"].AxisX.ScaleView.Zoomable = true;
            chart2.ChartAreas["ChartArea1"].AxisY.ScaleView.Zoomable = true;
            chart2.MouseWheel += chart1_MouseWheel;
            
            myTimer.Elapsed += OnTimedEvent;
            myTimer.SynchronizingObject = this;
            myTimer.Enabled = true;

            InitChart();
        }

        private void chart1_MouseWheel(object sender, MouseEventArgs e)
        {
            var chart = (Chart)sender;
            var xAxis = chart.ChartAreas[0].AxisX;
            var yAxis = chart.ChartAreas[0].AxisY;

            try
            {
                if (e.Delta < 0) // Scrolled down.
                {
                    xAxis.ScaleView.ZoomReset();
                    yAxis.ScaleView.ZoomReset();
                }
                else if (e.Delta > 0) // Scrolled up.
                {
                    var xMin = xAxis.ScaleView.ViewMinimum;
                    var xMax = xAxis.ScaleView.ViewMaximum;
                    var yMin = yAxis.ScaleView.ViewMinimum;
                    var yMax = yAxis.ScaleView.ViewMaximum;

                    var posXStart = xAxis.PixelPositionToValue(e.Location.X) - (xMax - xMin) / 1.5;
                    var posXFinish = xAxis.PixelPositionToValue(e.Location.X) + (xMax - xMin) / 1.5;
                    var posYStart = yAxis.PixelPositionToValue(e.Location.Y) - (yMax - yMin) / 1.5;
                    var posYFinish = yAxis.PixelPositionToValue(e.Location.Y) + (yMax - yMin) / 1.5;

                    xAxis.ScaleView.Zoom(posXStart, posXFinish);
                    yAxis.ScaleView.Zoom(posYStart, posYFinish);
                }
            }
            catch { }
        }

        private void InitChart()
        {
            Run myRun = ser.Deserialize<Run>(File.ReadAllText("C:\\Users\\jlievers\\Desktop\\EVERYTHING\\labScheduler\\Schedules\\Thee_labSchedule.json"));
            int myCurrentStepIndex = myRun.current_step;
            Step currentStepObj = myRun.schedule[myCurrentStepIndex];
            int currentStepStartTime = currentStepObj.start_time;
            int totalNumSteps = myRun.schedule.Count();
            Step firstStep = myRun.schedule[0];
            string firstStepName = firstStep.step;

            chart2.Series.Clear();
            chart2.Series.Add("wait");
            chart2.Series["wait"].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.StackedBar;
            chart2.Series["wait"].Color = Color.Transparent;
            List<string> allSteps = new List<string>();
            foreach (Step oneStep in myRun.schedule)
            {
                if (!allSteps.Contains(oneStep.step))
                {
                    allSteps.Add(oneStep.step);
                    chart2.Series.Add(oneStep.step);
                    chart2.Series[oneStep.step].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.StackedBar;
                    string toolTipIn = myTranslator[oneStep.step];

                    chart2.Series[oneStep.step].ToolTip = oneStep.step.ToUpper() + ": \n" + getToolTip(toolTipIn);
                }
            }
            foreach (Step oneStep in myRun.schedule)
            {
                string currentPlateGroup = oneStep.plate_name;
                string currentStepName = oneStep.step;//could be used in diaplay
                string plateIndex = oneStep.plate_index.ToString();
                int startTime = oneStep.start_time;
                int endTime = oneStep.end_time;
                int totalTime = endTime - startTime;
                if (oneStep.step == firstStepName)
                {
                    chart2.Series["wait"].Points.AddXY(plateIndex, endTime);
                }
               
                chart2.Series[currentStepName].Points.AddXY(plateIndex, totalTime);
            }
            chart2.Annotations["VerticalLineAnnotation1"].X = currentStepStartTime;
        }

        private void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            InitChart();
        }

        private void chart2_MouseMove(object sender, MouseEventArgs e)
        {
            Point mousePoint = new Point(e.X, e.Y);
            chart2.ChartAreas[0].CursorX.Interval = 0;
            chart2.ChartAreas[0].CursorY.Interval = 0;
            chart2.ChartAreas[0].CursorX.SetCursorPixelPosition(mousePoint, true);
            chart2.ChartAreas[0].CursorY.SetCursorPixelPosition(mousePoint, true);
            HitTestResult result = chart2.HitTest(e.X, e.Y);
            if (result.PointIndex > -1 && result.ChartArea != null)
            {
                //result.Series.Points[result.PointIndex].ToolTip = result.Series.Name;
            }
        }

        private string getToolTip(string ovpName)
        {
            //Read Doc
            XmlDocument document = new XmlDocument();
            document.LoadXml(File.ReadAllText("P:\\384_library\\Overlord_Procedures\\Transitions\\" + ovpName + ".ovp"));

            //The final string for thre tooltip
            string myActions = "";

            //Get param list
            XmlNodeList xnList1 = document.SelectNodes("/Procedure/Actions/Action/Parameters");
            for (int i = 1; i < xnList1.Count-1; i++)
            {
                //inner text of parameter node
                string parametersText = xnList1[i].InnerText;
                if (parametersText != "")
                {
                    //turn inner text into xml, cuz Paa xml format is whack
                    XmlDocument paramNode = new XmlDocument();
                    paramNode.LoadXml(parametersText);

                    //the First child is the name of the Name of the 'Action'
                    string aString = paramNode.FirstChild.Name;
                    if (paramNode.FirstChild.Name == "callProcedure")
                    {
                        aString = aString + " : " + paramNode.GetElementsByTagName("procedure")[0].InnerText;
                    }
                    if (paramNode.FirstChild.Name == "timerCommand")
                    {
                        aString = aString + " : " + paramNode.GetElementsByTagName("timer")[0].InnerText;
                    }
                    if (paramNode.FirstChild.Name == "VBScript")
                    {
                        aString = aString + " : " + paramNode.GetElementsByTagName("scriptFile")[0].InnerText;
                    }
                    if (paramNode.FirstChild.Name == "setVarCommand")
                    {
                        aString = aString + " : " + paramNode.GetElementsByTagName("leftSide")[0].InnerText + " = " + paramNode.GetElementsByTagName("rightSide")[0].InnerText;
                    }
                    if (paramNode.FirstChild.Name == "ProNEDx")
                    {
                        aString = aString + " : " + paramNode.GetElementsByTagName("sequenceName")[0].InnerText;
                    }
                    if (paramNode.FirstChild.Name == "decision")
                    {
                        XmlNodeList xnList2 = xnList1[i].SelectNodes("Group");
                        for (int j = 0; j < xnList2.Count; j++)
                        {
                            XmlNodeList xnList3 = xnList2[i].SelectNodes("/Action/Parameters");
                            for (int x = 0; j < xnList3.Count; x++)
                            {
                                string paramText2 = xnList1[x].InnerText;
                                aString = aString + " : \n\t" + paramText2;
                            }
                        }
                    }
                    myActions = myActions + aString + "\n";
                }
            }
            return myActions;
        }

        private void chart2_Click(object sender, EventArgs e)
        {

        }
    }
}
