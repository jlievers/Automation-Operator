using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Runtime.InteropServices;
using System.IO;

namespace Com.Invitae.LabAutomation
{
    public static class Trinean
    {
        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);[DllImport("kernel32")]

        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);

        public static DataTable PopulateOutTable(DataTable chip1_DT, DataTable chip2_DT, out string error)
        {
            try
            {
                DataTable outTable = chip1_DT.Clone();
                chip1_DT.DefaultView.Sort = "Sample name ASC";
                chip2_DT.DefaultView.Sort = "Sample name ASC";
                error = "";
                int count = 0;
                for (int i = 0; i < chip1_DT.Rows.Count; i++)
                {
                    DataRow row = outTable.NewRow();
                    outTable.Rows.Add(row);
                    string name_1 = chip1_DT.Rows[i]["Sample name"].ToString();
                    string name_2 = chip2_DT.Rows[i]["Sample name"].ToString();
                    if (name_1 == name_2) outTable.Rows[i]["Sample name"] = name_1;
                    else
                    {
                        error = "Different Values in 'Sample name' column for the two DropSense files. Row:" + i.ToString();
                        return outTable;
                    }
                    string dropPlateID_1 = chip1_DT.Rows[i]["DropPlate ID"].ToString();
                    string dropPlateID_2 = chip2_DT.Rows[i]["DropPlate ID"].ToString();
                    if (dropPlateID_1 == dropPlateID_2) outTable.Rows[i]["DropPlate ID"] = dropPlateID_1;
                    else
                    {
                        error = "Different Values in 'DropPlate ID' column for the two DropSense files. Row:" + i.ToString();
                        return outTable;
                    }
                    string id_1 = chip1_DT.Rows[i]["Instrument ID"].ToString();
                    outTable.Rows[i]["Instrument ID"] = id_1;

                    string position_1 = chip1_DT.Rows[i]["DropPlate Position"].ToString();
                    string position_2 = chip2_DT.Rows[i]["DropPlate Position"].ToString();
                    if (position_1 == position_2) outTable.Rows[i]["DropPlate Position"] = position_1;
                    else
                    {
                        error = "Different Values in 'DropPlate Position' column for the two DropSense files. Row:" + i.ToString();
                        return outTable;
                    }
                    outTable.Rows[i]["Date"] = chip1_DT.Rows[i]["Date"].ToString();
                    outTable.Rows[i]["Time"] = chip1_DT.Rows[i]["Time"].ToString();

                    decimal conc_1; bool conc1Valid = Decimal.TryParse(chip1_DT.Rows[i]["A260 Concentration (ng/ul)"].ToString(), out conc_1);
                    decimal conc_2; bool conc2Valid = Decimal.TryParse(chip2_DT.Rows[i]["A260 Concentration (ng/ul)"].ToString(), out conc_2);

                    int caseSwitch;
                    if (conc1Valid && conc2Valid)
                    {
                        if (conc_1 > 23 && conc_2 > 23 || conc_1 < 23 && conc_2 < 23)
                        {
                            caseSwitch = 3;
                        }
                        else if (conc_1 > 23)
                        {
                            caseSwitch = 1;
                        }
                        else
                        {
                            caseSwitch = 2;
                        }
                    }
                    else if (conc1Valid)
                    {
                        caseSwitch = 1;
                    }
                    else if (conc2Valid)
                    {
                        caseSwitch = 2;
                    }
                    else
                    {
                        caseSwitch = 1;
                        count++;
                    }
                    if (count > 15)
                    {
                        error = "16 or more failed wells";
                    }
                    switch (caseSwitch)
                    {
                        case 1:
                            outTable.Rows[i]["A260 Concentration (ng/ul)"] = conc_1;
                            outTable.Rows[i]["A260/A230"] = chip1_DT.Rows[i]["A260/A230"];
                            outTable.Rows[i]["A260/A280"] = chip1_DT.Rows[i]["A260/A280"];
                            outTable.Rows[i]["Median A260 Concentration (ng/ul)"] = chip1_DT.Rows[i]["Median A260 Concentration (ng/ul)"];
                            break;
                        case 2:
                            outTable.Rows[i]["A260 Concentration (ng/ul)"] = conc_2;
                            outTable.Rows[i]["A260/A230"] = chip2_DT.Rows[i]["A260/A230"];
                            outTable.Rows[i]["A260/A280"] = chip2_DT.Rows[i]["A260/A280"];
                            outTable.Rows[i]["Median A260 Concentration (ng/ul)"] = chip2_DT.Rows[i]["Median A260 Concentration (ng/ul)"];
                            break;
                        case 3:
                            outTable.Rows[i]["A260 Concentration (ng/ul)"] = (conc_1 + conc_2) / 2;
                            outTable.Rows[i]["A260/A230"] = (Convert.ToDecimal(chip1_DT.Rows[i]["A260/A230"]) + Convert.ToDecimal(chip2_DT.Rows[i]["A260/A230"])) / 2;
                            outTable.Rows[i]["A260/A280"] = (Convert.ToDecimal(chip1_DT.Rows[i]["A260/A280"]) + Convert.ToDecimal(chip2_DT.Rows[i]["A260/A280"])) / 2;
                            outTable.Rows[i]["Median A260 Concentration (ng/ul)"] = (Convert.ToDecimal(chip1_DT.Rows[i]["Median A260 Concentration (ng/ul)"]) + Convert.ToDecimal(chip2_DT.Rows[i]["Median A260 Concentration (ng/ul)"])) / 2;
                            break;
                    }
                }
                return outTable;
            }
            catch (Exception ex)
            {
                throw new Exception("ERROR COMBINING DQ OUTPUT FILES" + ex.Message);
            }
        }//V2

        public static string readFile(string filePath)
        {
            try
            {
                string line = "";
                // Open the text file using a stream reader.
                using (StreamReader sr = new StreamReader(filePath))
                {
                    line = sr.ReadToEnd();
                    Console.WriteLine(line);
                }
                return line;
            }
            catch (Exception ex)
            {
                throw new Exception("ERROR READING .CSV: " + ex.Message);
            }
        }

        public static DataTable ReadCSV(string path)
        {

            DataTable dt = new DataTable();
            dt.Columns.Add("Sample name", typeof(string));
            dt.Columns.Add("A260 Concentration (ng/ul)", typeof(string));
            dt.Columns.Add("A260/A230", typeof(string));
            dt.Columns.Add("A260/A280", typeof(string));
            dt.Columns.Add("DropPlate ID", typeof(string));
            dt.Columns.Add("Instrument ID", typeof(string));
            dt.Columns.Add("Date", typeof(string));
            dt.Columns.Add("Time", typeof(string));
            dt.Columns.Add("DropPlate Position", typeof(string));
            dt.Columns.Add("Median A260 Concentration (ng/ul)", typeof(string));

            string[] myLines = File.ReadAllLines(path);

            for (int j = 0; j < myLines.Length; j++)
            {
                if (j == 0)
                {
                    //skip header
                }
                else
                {
                    string[] oneLine = myLines[j].Split(',');
                    if (oneLine[0] != "")
                    {
                        DataRow myRow = dt.NewRow();
                        myRow["Sample name"] = oneLine[0];
                        myRow["A260 Concentration (ng/ul)"] = oneLine[1];
                        myRow["A260/A230"] = oneLine[2];
                        myRow["A260/A280"] = oneLine[3];
                        myRow["DropPlate ID"] = oneLine[4];
                        myRow["Instrument ID"] = oneLine[5];
                        myRow["Date"] = oneLine[6];
                        myRow["Time"] = oneLine[7];
                        myRow["DropPlate Position"] = oneLine[8];
                        myRow["Median A260 Concentration (ng/ul)"] = oneLine[9];
                        dt.Rows.Add(myRow);
                    }
                }
            }

            dt.AcceptChanges();
            return dt;
        }//Redundant with Trinean//DLL_Instruments

        public static string DropquantFileOutString(DataTable myOutTable)
        {
            string strRowCommaSeparated = "";
            for (int i = 0; i < myOutTable.Columns.Count; i++)//Create Header Row
            {
                if (i != 0)
                {
                    strRowCommaSeparated = strRowCommaSeparated + ",";
                }
                strRowCommaSeparated = strRowCommaSeparated + myOutTable.Columns[i].ColumnName.ToString();
            }

            strRowCommaSeparated = strRowCommaSeparated + "\n";

            foreach (DataRow dr in myOutTable.Rows)
            {
                for (int i = 0; i < dr.ItemArray.Length; i++)
                {
                    if (i != 0)
                    {
                        strRowCommaSeparated = strRowCommaSeparated + ",";
                    }
                    strRowCommaSeparated = strRowCommaSeparated + dr.ItemArray[i].ToString();
                }
                strRowCommaSeparated = strRowCommaSeparated + "\n";
            }
            return strRowCommaSeparated;
        }

        #region Old Combined File Algorithms

        //public static DataTable PopulateOutTable(DataTable chip1_DT, DataTable chip2_DT)
        //{
        //    try
        //    {
        //        DataTable outTable = chip1_DT.Clone();
        //        chip1_DT.DefaultView.Sort = "Sample name ASC";
        //        chip2_DT.DefaultView.Sort = "Sample name ASC";
        //        string error = "";
        //        for (int i = 0; i < chip1_DT.Rows.Count; i++)
        //        {
        //            DataRow row = outTable.NewRow();
        //            outTable.Rows.Add(row);
        //            string name_1 = chip1_DT.Rows[i]["Sample name"].ToString();
        //            string name_2 = chip2_DT.Rows[i]["Sample name"].ToString();
        //            if (name_1 == name_2) outTable.Rows[i]["Sample name"] = name_1;
        //            else
        //            {
        //                error = "Different Values in 'Sample name' column for the two DropSense files. Row:" + i.ToString();
        //                return outTable;
        //            }
        //            string dropPlateID_1 = chip1_DT.Rows[i]["DropPlate ID"].ToString();
        //            string dropPlateID_2 = chip2_DT.Rows[i]["DropPlate ID"].ToString();
        //            if (dropPlateID_1 == dropPlateID_2) outTable.Rows[i]["DropPlate ID"] = dropPlateID_1;
        //            else
        //            {
        //                error = "Different Values in 'DropPlate ID' column for the two DropSense files. Row:" + i.ToString();
        //                return outTable;
        //            }
        //            string id_1 = chip1_DT.Rows[i]["Instrument ID"].ToString();
        //            string id_2 = chip2_DT.Rows[i]["Instrument ID"].ToString();
        //            if (id_1 == id_2) outTable.Rows[i]["Instrument ID"] = id_1;
        //            else
        //            {
        //                error = "Different Values in 'Instrument ID' column for the two DropSense files. Row:" + i.ToString();
        //                return outTable;
        //            }
        //            string position_1 = chip1_DT.Rows[i]["DropPlate Position"].ToString();
        //            string position_2 = chip2_DT.Rows[i]["DropPlate Position"].ToString();
        //            if (position_1 == position_2) outTable.Rows[i]["DropPlate Position"] = position_1;
        //            else
        //            {
        //                error = "Different Values in 'DropPlate Position' column for the two DropSense files. Row:" + i.ToString();
        //                return outTable;
        //            }
        //            outTable.Rows[i]["Date"] = chip1_DT.Rows[i]["Date"].ToString();
        //            outTable.Rows[i]["Time"] = chip1_DT.Rows[i]["Time"].ToString();

        //            decimal conc_1; bool conc1Valid = Decimal.TryParse(chip1_DT.Rows[i]["A260 Concentration (ng/ul)"].ToString(), out conc_1) && (conc_1 >= 0);
        //            decimal conc_2; bool conc2Valid = Decimal.TryParse(chip2_DT.Rows[i]["A260 Concentration (ng/ul)"].ToString(), out conc_2) && (conc_2 >= 0);

        //            int caseSwitch;
        //            if (conc1Valid && conc2Valid)
        //            {
        //                if (conc_1 > conc_2)
        //                {
        //                    caseSwitch = 1;
        //                }
        //                else
        //                {
        //                    caseSwitch = 2;
        //                }
        //            }
        //            else if (conc1Valid)
        //            {
        //                caseSwitch = 1;
        //            }
        //            else if (conc2Valid)
        //            {
        //                caseSwitch = 2;
        //            }
        //            else
        //            {
        //                caseSwitch = 3;
        //            }
        //            switch (caseSwitch)
        //            {
        //                case 1:
        //                    outTable.Rows[i]["A260 Concentration (ng/ul)"] = conc_1;
        //                    outTable.Rows[i]["A260/A230"] = Convert.ToDecimal(chip1_DT.Rows[i]["A260/A230"]);
        //                    outTable.Rows[i]["A260/A280"] = Convert.ToDecimal(chip1_DT.Rows[i]["A260/A280"]);
        //                    outTable.Rows[i]["Median A260 Concentration (ng/ul)"] = Convert.ToDecimal(chip1_DT.Rows[i]["Median A260 Concentration (ng/ul)"]);
        //                    break;
        //                case 2:
        //                    outTable.Rows[i]["A260 Concentration (ng/ul)"] = conc_2;
        //                    outTable.Rows[i]["A260/A230"] = Convert.ToDecimal(chip2_DT.Rows[i]["A260/A230"]);
        //                    outTable.Rows[i]["A260/A280"] = Convert.ToDecimal(chip2_DT.Rows[i]["A260/A280"]);
        //                    outTable.Rows[i]["Median A260 Concentration (ng/ul)"] = Convert.ToDecimal(chip2_DT.Rows[i]["Median A260 Concentration (ng/ul)"]);
        //                    break;
        //                case 3:
        //                    outTable.Rows[i]["A260 Concentration (ng/ul)"] = 0;
        //                    outTable.Rows[i]["A260/A230"] = 0;
        //                    outTable.Rows[i]["A260/A280"] = 0;
        //                    outTable.Rows[i]["Median A260 Concentration (ng/ul)"] = 0;
        //                    break;
        //            }
        //        }
        //        return outTable;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception("ERROR COMBINING DQ OUTPUT FILES" + ex.Message);
        //    }
        //}

        //public static DataTable PopulateOutTable(DataTable chip1_DT, DataTable chip2_DT, out string error)
        //{
        //    try
        //    {
        //        DataTable outTable = chip1_DT.Clone();
        //        chip1_DT.DefaultView.Sort = "Sample name ASC";
        //        chip2_DT.DefaultView.Sort = "Sample name ASC";
        //        error = "";
        //        int count = 0;
        //        for (int i = 0; i < chip1_DT.Rows.Count; i++)
        //        {
        //            DataRow row = outTable.NewRow();
        //            outTable.Rows.Add(row);
        //            string name_1 = chip1_DT.Rows[i]["Sample name"].ToString();
        //            string name_2 = chip2_DT.Rows[i]["Sample name"].ToString();
        //            if (name_1 == name_2) outTable.Rows[i]["Sample name"] = name_1;
        //            else
        //            {
        //                error = "Different Values in 'Sample name' column for the two DropSense files. Row:" + i.ToString();
        //                return outTable;
        //            }
        //            string dropPlateID_1 = chip1_DT.Rows[i]["DropPlate ID"].ToString();
        //            string dropPlateID_2 = chip2_DT.Rows[i]["DropPlate ID"].ToString();
        //            if (dropPlateID_1 == dropPlateID_2) outTable.Rows[i]["DropPlate ID"] = dropPlateID_1;
        //            else
        //            {
        //                error = "Different Values in 'DropPlate ID' column for the two DropSense files. Row:" + i.ToString();
        //                return outTable;
        //            }
        //            string id_1 = chip1_DT.Rows[i]["Instrument ID"].ToString();
        //            outTable.Rows[i]["Instrument ID"] = id_1;

        //            string position_1 = chip1_DT.Rows[i]["DropPlate Position"].ToString();
        //            string position_2 = chip2_DT.Rows[i]["DropPlate Position"].ToString();
        //            if (position_1 == position_2) outTable.Rows[i]["DropPlate Position"] = position_1;
        //            else
        //            {
        //                error = "Different Values in 'DropPlate Position' column for the two DropSense files. Row:" + i.ToString();
        //                return outTable;
        //            }
        //            outTable.Rows[i]["Date"] = chip1_DT.Rows[i]["Date"].ToString();
        //            outTable.Rows[i]["Time"] = chip1_DT.Rows[i]["Time"].ToString();

        //            decimal conc_1; bool conc1Valid = Decimal.TryParse(chip1_DT.Rows[i]["A260 Concentration (ng/ul)"].ToString(), out conc_1);
        //            decimal conc_2; bool conc2Valid = Decimal.TryParse(chip2_DT.Rows[i]["A260 Concentration (ng/ul)"].ToString(), out conc_2);

        //            int caseSwitch;
        //            if (conc1Valid && conc2Valid)
        //            {
        //                if (conc_1 > 23 && conc_2 > 23 || conc_1 < 23 && conc_2 < 23)
        //                {
        //                    caseSwitch = 4;
        //                }
        //                else if (conc_1 > 23)
        //                {
        //                    caseSwitch = 1;
        //                }
        //                else
        //                {
        //                    caseSwitch = 2;
        //                }
        //            }
        //            else if (conc1Valid)
        //            {
        //                caseSwitch = 1;
        //            }
        //            else if (conc2Valid)
        //            {
        //                caseSwitch = 2;
        //            }
        //            else
        //            {
        //                caseSwitch = 3;
        //                count++;
        //            }
        //            if (count > 15)
        //            {
        //                error = "16 or more failed wells";
        //            }
        //            switch (caseSwitch)
        //            {
        //                case 1:
        //                    outTable.Rows[i]["A260 Concentration (ng/ul)"] = conc_1;
        //                    outTable.Rows[i]["A260/A230"] = chip1_DT.Rows[i]["A260/A230"];
        //                    outTable.Rows[i]["A260/A280"] = chip1_DT.Rows[i]["A260/A280"];
        //                    outTable.Rows[i]["Median A260 Concentration (ng/ul)"] = chip1_DT.Rows[i]["Median A260 Concentration (ng/ul)"];
        //                    break;
        //                case 2:
        //                    outTable.Rows[i]["A260 Concentration (ng/ul)"] = conc_2;
        //                    outTable.Rows[i]["A260/A230"] = chip2_DT.Rows[i]["A260/A230"];
        //                    outTable.Rows[i]["A260/A280"] = chip2_DT.Rows[i]["A260/A280"];
        //                    outTable.Rows[i]["Median A260 Concentration (ng/ul)"] = chip2_DT.Rows[i]["Median A260 Concentration (ng/ul)"];
        //                    break;
        //                case 3:
        //                    decimal myDecimal;
        //                    if (Decimal.TryParse(chip1_DT.Compute("AVG([A260 Concentration (ng/ul)])", "[A260 Concentration (ng/ul)] > 0").ToString(), out myDecimal))
        //                    {
        //                        outTable.Rows[i]["A260 Concentration (ng/ul)"] = myDecimal;
        //                    }
        //                    else
        //                    {
        //                        throw new Exception("All Chip A260 Concentration (ng/ul) = EMPTY");
        //                    }
        //                    if (Decimal.TryParse(chip1_DT.Compute("AVG([A260/A230])", "[A260/A230] > 0").ToString(), out myDecimal))
        //                    {
        //                        outTable.Rows[i]["A260/A230"] = myDecimal;
        //                    }
        //                    else
        //                    {
        //                        throw new Exception("All Chip A260/A230  = EMPTY");
        //                    }
        //                    if (Decimal.TryParse(chip1_DT.Compute("AVG([A260/A280])", "[A260/A280] > 0").ToString(), out myDecimal))
        //                    {
        //                        outTable.Rows[i]["A260/A280"] = myDecimal;
        //                    }
        //                    else
        //                    {
        //                        throw new Exception("All Chip Median A260 Concentration (ng/ul) = EMPTY");
        //                    }
        //                    if (Decimal.TryParse(chip1_DT.Compute("AVG([Median A260 Concentration (ng/ul)])", "[Median A260 Concentration (ng/ul)] > 0").ToString(), out myDecimal))
        //                    {
        //                        outTable.Rows[i]["Median A260 Concentration (ng/ul)"] = myDecimal;
        //                    }
        //                    else
        //                    {
        //                        throw new Exception("All Chip Median A260 Concentration (ng/ul) values = EMPTY ");
        //                    }
        //                    break;
        //                case 4:
        //                    outTable.Rows[i]["A260 Concentration (ng/ul)"] = (conc_1 + conc_2) / 2;
        //                    outTable.Rows[i]["A260/A230"] = (Convert.ToDecimal(chip1_DT.Rows[i]["A260/A230"]) + Convert.ToDecimal(chip2_DT.Rows[i]["A260/A230"])) / 2;
        //                    outTable.Rows[i]["A260/A280"] = (Convert.ToDecimal(chip1_DT.Rows[i]["A260/A280"]) + Convert.ToDecimal(chip2_DT.Rows[i]["A260/A280"])) / 2;
        //                    outTable.Rows[i]["Median A260 Concentration (ng/ul)"] = (Convert.ToDecimal(chip1_DT.Rows[i]["Median A260 Concentration (ng/ul)"]) + Convert.ToDecimal(chip1_DT.Rows[i]["Median A260 Concentration (ng/ul)"])) / 2;
        //                    break;
        //            }
        //        }
        //        return outTable;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception("ERROR COMBINING DQ OUTPUT FILES" + ex.Message);
        //    }
        //}//Redundant with Trinean

        #endregion

        //    public static string CreateExtractionNormCombinedFile(string systemFolderPath, string runID, string currentPlateBarcode)//NOT USED. DEV. USES LIMS OBJ LIB
        //    {
        //        System.Web.Script.Serialization.JavaScriptSerializer ser = new System.Web.Script.Serialization.JavaScriptSerializer();
        //        //Read RunOutput
        //        string myPath = systemFolderPath + "\\RunOutputFiles\\NormShear\\" + runID + ".json";
        //        string JSONFileText = File.ReadAllText(myPath);
        //        DLL_RunOutput.NormShear_RunInfo myInfo = ser.Deserialize<DLL_RunOutput.NormShear_RunInfo>(JSONFileText);
        //        //Get DropQuant file names
        //        string dq1 = "";
        //        string dq2 = "";

        //        //bool error = false;
        //        foreach (DLL_RunOutput.NormShear_Plates myPlate in myInfo.normshear_plates)

        //        {
        //            if (myPlate.pq_id == currentPlateBarcode)
        //            {
        //                dq1 = myPlate.gdna_norm_dq1_plate_id;
        //                dq2 = myPlate.gdna_norm_dq2_plate_id;
        //            }
        //        }
        //        //Read DropQuant Files and Combine
        //        DataTable myChip1_DT = ReadCSV(systemFolderPath + "\\Dropsense_Output_Files\\" + dq1 + ".csv");
        //        DataTable myChip2_DT = ReadCSV(systemFolderPath + "\\Dropsense_Output_Files\\" + dq2 + ".csv");
        //        string error = "";
        //        DataTable myOutTable = PopulateOutTable(myChip1_DT, myChip2_DT, out error);
        //        if (error != "")
        //        {
        //            if (error == "16 or more failed wells")
        //            {
        //                throw new Exception("TOO MANY FAILED WELLS: " + error);
        //            }
        //            else
        //            {
        //                throw new Exception("ERROR COMBINING DQ OUTPUT FILES: " + error);
        //            }
        //        }
        //        //Recreate ".csv"
        //        string DQFileString = DropquantFileOutString(myOutTable);
        //        string combinedFile = "";
        //        //write combined file
        //        File.WriteAllText(systemFolderPath + "\\Dropsense_Output_Files\\" + dq1 + "+" + dq2 + ".csv", DQFileString);
        //        foreach (DLL_RunOutput.NormShear_Plates myPlate in myInfo.normshear_plates)
        //        {
        //            if (myPlate.pq_id == currentPlateBarcode)
        //            {
        //                combinedFile = dq1 + "+" + dq2 + ".csv";
        //                myPlate.combined_trinean_file_name = combinedFile;

        //            }
        //        }
        //        //Write RunFile
        //        File.WriteAllText(myPath, ser.Serialize(myInfo).ToString());
        //        return combinedFile;
        //    }


        //    ///FROM Trinean Driver Must Be reconciled at some point!

        //    private static void BuildWorklistData(ref DLL_RunOutput.Library_RunInfo myRunInfo_In, ref DLL_RunOutput.Hyb_RunInfo myRunInfo_In2, ref DLL_LIMS_Workflow.DQDataMessage myDQData_In, string runID, out bool error, int robotPositionalIndex, string worklistType_In, string numChipsIN, string runOutputFolderPath)
        //    {
        //        JavaScriptSerializer ser = new JavaScriptSerializer();
        //        string _DQResultsOutFolderPath = runOutputFolderPath; //THIS MUST BE FIXED!!!!!!!!!!!
        //        error = false;
        //        try
        //        {
        //            string myChip1BC = "";
        //            string myChip2BC = "";
        //            string myWFId = "";
        //            string mySourcePlate = "";
        //            string myDestination = "";
        //            int plateGroupIndex = robotPositionalIndex - 1;
        //            if (worklistType_In == "Pre Capture Normalization") //obsolete delete
        //            {
        //                string JSONFileText = File.ReadAllText(runOutputFolderPath + runID + ".json");
        //                myRunInfo_In = ser.Deserialize<DLL_RunOutput.Library_RunInfo>(JSONFileText);
        //                myChip1BC = myRunInfo_In.plate_data[plateGroupIndex].pre_cap_norm_dq1_plate_id;
        //                myChip2BC = myRunInfo_In.plate_data[plateGroupIndex].pre_cap_norm_dq2_plate_id;
        //                myWFId = myRunInfo_In.plate_data[plateGroupIndex].workflow;
        //                mySourcePlate = myRunInfo_In.plate_data[plateGroupIndex].ipcr_libs_plate;
        //                myDestination = myRunInfo_In.plate_data[plateGroupIndex].pre_cap_pool_plate;
        //            }
        //            else if (worklistType_In == "Post Capture Normalization")
        //            {
        //                string JSONFileText = File.ReadAllText(runOutputFolderPath + runID + ".json");
        //                myRunInfo_In2 = ser.Deserialize<DLL_RunOutput.Hyb_RunInfo>(JSONFileText);
        //                myChip1BC = myRunInfo_In2.hyb_plates[plateGroupIndex].post_cap_norm_dq1_plate_id;
        //                myChip2BC = myRunInfo_In2.hyb_plates[plateGroupIndex].post_cap_norm_dq2_plate_id;
        //                myWFId = myRunInfo_In2.hyb_plates[plateGroupIndex].workflow;
        //                mySourcePlate = myRunInfo_In2.hyb_plates[plateGroupIndex].post_cap_lib_plate;
        //                myDestination = myRunInfo_In2.hyb_plates[plateGroupIndex].post_cap_norm_plate;
        //            }
        //            myDQData_In.workflow = myWFId;
        //            myDQData_In.norm_source_plate = mySourcePlate;
        //            myDQData_In.norm_destination_plate = myDestination;
        //            myDQData_In.worklist_type = worklistType_In;

        //            DataTable myOutTable = null;
        //            if (Convert.ToInt32(numChipsIN) == 2)
        //            {
        //                DataTable myChip1_DT = ReadCSV(_DQResultsOutFolderPath + myChip1BC + ".csv");
        //                DataTable myChip2_DT = Trinean.ReadCSV(_DQResultsOutFolderPath + myChip2BC + ".csv");
        //                myOutTable = PopulateOutTable(myChip1_DT, myChip2_DT);
        //            }
        //            else
        //            {
        //                myOutTable = ReadCSV(_DQResultsOutFolderPath + myChip1BC + ".csv");
        //            }
        //            string DQFileString = DropquantFileOutString(myOutTable);
        //            myDQData_In.dropquant_data = DQFileString;
        //            if (Convert.ToInt32(numChipsIN) == 2)
        //            {
        //                File.WriteAllText(_DQResultsOutFolderPath + myChip1BC + "+" + myChip2BC + ".csv", DQFileString);
        //            }
        //            else
        //            {
        //                File.WriteAllText(_DQResultsOutFolderPath + myChip1BC + ".csv", DQFileString);
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            throw new Exception("BUILD WORKLIST MESSAGE: " + ex.Message);
        //        }
        //    }

        //    private static void GetWorklist(DLL_RunOutput.Library_RunInfo myRunInfo_In, DLL_RunOutput.Hyb_RunInfo myRunInfo_In2, DLL_LIMS_Workflow.DQDataMessage myDQData_In, string runID, out bool error, int robotPositionalIndex, string worklistType_In, string runOutputFolderPath)
        //    {
        //        JavaScriptSerializer ser = new JavaScriptSerializer();
        //        string _worklistFolderPath = runOutputFolderPath; //THIS MUST BE FIXED!!!!!!!!!!!
        //        error = false;
        //        try
        //        {
        //            string JSONOutput = ser.Serialize(myDQData_In);
        //            //LIMSObjectLibrary.DQDataResponse worklistResponseObj = ser.Deserialize<LIMSObjectLibrary.DQDataResponse>(LIMSObjectLibrary.Utilities.SendJSONToAPI(JSONOutput, runID, runOutputFolderPath, _apiAddress, _apiUserName, _apiPassword, _apiTimeout, "Get PreCap Worklist"));
        //            DLL_LIMS_Workflow.DQDataResponse worklistResponseObj = new DLL_LIMS_Workflow.DQDataResponse();
        //            DLL_LIMS_Workflow.LIMS_Workflow.ReadErrorsInResponse(worklistResponseObj.errors);
        //            string worklistFilename = runID + "_" + worklistResponseObj.data.qm_id;

        //            //add worklist name to output file
        //            int plateGroupIndex = robotPositionalIndex - 1;
        //            if (worklistType_In == "Pre Capture Normalization")
        //            {
        //                myRunInfo_In.plate_data[plateGroupIndex].pre_cap_normpool_worklist_filename = worklistFilename;
        //                File.WriteAllText(runOutputFolderPath + runID + ".json", ser.Serialize(myRunInfo_In));
        //            }
        //            else if (worklistType_In == "Post Capture Normalization")
        //            {
        //                myRunInfo_In2.hyb_plates[plateGroupIndex].post_cap_norm_worklist_filename = worklistFilename;
        //                File.WriteAllText(runOutputFolderPath + runID + ".json", ser.Serialize(myRunInfo_In2));
        //                WritePrivateProfileString("MAIN", "Plate" + robotPositionalIndex + "_WorklistName", worklistFilename, runOutputFolderPath + runID + ".ini");
        //            }
        //            //write worklist
        //            File.WriteAllText(_worklistFolderPath + worklistFilename + ".csv", worklistResponseObj.data.worklist.Trim('"'));
        //        }
        //        catch (Exception ex)
        //        {
        //            throw new Exception("GET WORKLIST: " + ex.Message);
        //        }
        //    }

        //    private static void GetPreCapWorklist(string runID, int robotPositionalIndex, string runOutputFolderPath)
        //    {
        //        JavaScriptSerializer ser = new JavaScriptSerializer();
        //        string _DQResultsOutFolderPath = runOutputFolderPath; //THIS MUST BE FIXED!!!!!!!!!!!
        //        string _worklistFolderPath = runOutputFolderPath; //THIS MUST BE FIXED!!!!!!!!!!!
        //        try
        //        {
        //            //Get Run Info
        //            string JSONFileText = File.ReadAllText(runOutputFolderPath + runID + ".json");
        //            DLL_RunOutput.Library_RunInfo myRunInfo = ser.Deserialize<DLL_RunOutput.Library_RunInfo>(JSONFileText);
        //            //Get plate being normalized 
        //            DLL_RunOutput.Library_WorkFlowPlateGroup myPlateGroup = new DLL_RunOutput.Library_WorkFlowPlateGroup();
        //            int i = 0;
        //            int myPlateIndex = -1;
        //            foreach (DLL_RunOutput.Library_WorkFlowPlateGroup onePlate in myRunInfo.plate_data)
        //            {
        //                if (onePlate.plate_transition_group == robotPositionalIndex && onePlate.status == "active")
        //                {
        //                    myPlateGroup = onePlate;
        //                    myPlateIndex = i;
        //                }
        //                i++;
        //            }
        //            //Build worklist request payload
        //            DLL_LIMS_Workflow.GePreCaptWorklist_Payload myPayload = new DLL_LIMS_Workflow.GePreCaptWorklist_Payload();
        //            DLL_LIMS_Workflow.GetPreCapWorklist_Payload_Inputs myInputs = new DLL_LIMS_Workflow.GetPreCapWorklist_Payload_Inputs();
        //            myPayload.user = "normshearbot@invitae.com";
        //            myPayload.workflow = myPlateGroup.workflow;
        //            myPayload.step_name = "Pre-Cap Quantification";
        //            myPayload.report_name = "pre_cap_quant";
        //            myPayload.plate_id = myPlateGroup.ipcr_libs_plate;
        //            myPayload.assay_id = myPlateGroup.assay;
        //            myPayload.drop_quant_data = File.ReadAllText(_DQResultsOutFolderPath + myPlateGroup.pre_cap_norm_dq1_plate_id + ".csv");
        //            myPayload.material_name = "pre_cap_worklist";
        //            myInputs.ipcr_libs_plate = myPlateGroup.ipcr_libs_plate;
        //            myInputs.pre_cap_pool_plate = myPlateGroup.pre_cap_pool_plate;
        //            myPayload.step_inputs = myInputs;
        //            //Get worklist
        //            string JSONOutput = ser.Serialize(myPayload);
        //            //LIMSObjectLibrary.GetPreCapWorklist_Response responseObj = ser.Deserialize<LIMSObjectLibrary.GetPreCapWorklist_Response>(LIMSObjectLibrary.Utilities.SendJSONToAPI(JSONOutput, runID, _runOutputFolderPath, _apiAddress, _apiUserName, _apiPassword, _apiTimeout, "Get PreCap Worklist"));
        //            DLL_LIMS_Workflow.GetPreCapWorklist_Response responseObj = new DLL_LIMS_Workflow.GetPreCapWorklist_Response();
        //            DLL_LIMS_Workflow.LIMS_Workflow.ReadErrorsInResponse(responseObj.errors);
        //            //Update RunInfo with response data
        //            myPlateGroup.pre_cap_normpool_worklist_filename = responseObj.data.file_name;
        //            myRunInfo.plate_data[myPlateIndex] = myPlateGroup;
        //            //Write files
        //            File.WriteAllText(runOutputFolderPath + runID + ".json", ser.Serialize(myRunInfo));
        //            File.WriteAllText(_worklistFolderPath + responseObj.data.file_name, responseObj.data.file_contents.Trim('"'));
        //        }
        //        catch (Exception ex)
        //        {
        //            throw new Exception("GET PRECAP WORKLIST: " + ex.Message);
        //        }

        //    }
    }
}
