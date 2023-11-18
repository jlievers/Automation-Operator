using System;
using System.Data;
using System.IO;

namespace Com.Invitae.LabAutomation.Model
{
    public static class Trinean
    {
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
        }//DLL_Instruments

        public static DataTable ReadCSV(string path)
        {
            if (!File.Exists(path))
            {
                return null;
            }
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
            string fullPath = Path.GetFullPath(path);
            string file = Path.GetFileName(fullPath);
            string dir = Path.GetDirectoryName(fullPath);
            string connString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=\"" + dir + "\\\";Extended Properties='text;HDR=Yes;IMEX=1;TypeGuessRows=0;ImportMixedTypes=Text;FMT=CSVDelimited(,)';";
            string query = "SELECT * FROM " + file;
            System.Data.OleDb.OleDbDataAdapter da = new System.Data.OleDb.OleDbDataAdapter(query, connString);
            da.Fill(dt);
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (dt.Rows[i]["Date"] != System.DBNull.Value && dt.Rows[i]["Time"] != System.DBNull.Value)
                {
                    DateTime date = Convert.ToDateTime(dt.Rows[i]["Date"]);
                    DateTime time = Convert.ToDateTime(dt.Rows[i]["Time"]);
                    string sDate = date.ToString("MM/dd/yyyy");
                    string sTime = time.ToString("hh:mm:ss");
                    dt.Rows[i]["Date"] = sDate;
                    dt.Rows[i]["Time"] = sTime;
                }
                else
                {
                    dt.Rows[i].Delete();
                }
            }
            dt.AcceptChanges();
            da.Dispose();
            return dt;
        }//DLL_Instruments

        public static DataTable PopulateOutTable(DataTable chip1_DT, DataTable chip2_DT)
        {
            try
            {
                DataTable outTable = chip1_DT.Clone();
                chip1_DT.DefaultView.Sort = "Sample name ASC";
                chip2_DT.DefaultView.Sort = "Sample name ASC";
                string error = "";
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
                    string id_2 = chip2_DT.Rows[i]["Instrument ID"].ToString();
                    if (id_1 == id_2) outTable.Rows[i]["Instrument ID"] = id_1;
                    else
                    {
                        error = "Different Values in 'Instrument ID' column for the two DropSense files. Row:" + i.ToString();
                        return outTable;
                    }
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

                    decimal conc_1; bool conc1Valid = Decimal.TryParse(chip1_DT.Rows[i]["A260 Concentration (ng/ul)"].ToString(), out conc_1) && (conc_1 >= 0);
                    decimal conc_2; bool conc2Valid = Decimal.TryParse(chip2_DT.Rows[i]["A260 Concentration (ng/ul)"].ToString(), out conc_2) && (conc_2 >= 0);

                    int caseSwitch;
                    if (conc1Valid && conc2Valid)
                    {
                        if (conc_1 > conc_2)
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
                        caseSwitch = 3;
                    }
                    switch (caseSwitch)
                    {
                        case 1:
                            outTable.Rows[i]["A260 Concentration (ng/ul)"] = conc_1;
                            outTable.Rows[i]["A260/A230"] = Convert.ToDecimal(chip1_DT.Rows[i]["A260/A230"]);
                            outTable.Rows[i]["A260/A280"] = Convert.ToDecimal(chip1_DT.Rows[i]["A260/A280"]);
                            outTable.Rows[i]["Median A260 Concentration (ng/ul)"] = Convert.ToDecimal(chip1_DT.Rows[i]["Median A260 Concentration (ng/ul)"]);
                            break;
                        case 2:
                            outTable.Rows[i]["A260 Concentration (ng/ul)"] = conc_2;
                            outTable.Rows[i]["A260/A230"] = Convert.ToDecimal(chip2_DT.Rows[i]["A260/A230"]);
                            outTable.Rows[i]["A260/A280"] = Convert.ToDecimal(chip2_DT.Rows[i]["A260/A280"]);
                            outTable.Rows[i]["Median A260 Concentration (ng/ul)"] = Convert.ToDecimal(chip2_DT.Rows[i]["Median A260 Concentration (ng/ul)"]);
                            break;
                        case 3:
                            outTable.Rows[i]["A260 Concentration (ng/ul)"] = 0;
                            outTable.Rows[i]["A260/A230"] = 0;
                            outTable.Rows[i]["A260/A280"] = 0;
                            outTable.Rows[i]["Median A260 Concentration (ng/ul)"] = 0;
                            break;
                    }
                }
                return outTable;
            }
            catch (Exception ex)
            {
                throw new Exception("ERROR COMBINING DQ OUTPUT FILES" + ex.Message);
            }
        }//DLL_Instruments //Origional Algorithm

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
        }//DLL_Instruments
    }
}
