using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using System.Web.Script.Serialization;
using System.Windows.Forms;
using System.Data;


namespace LIMSObjectLibrary
{
    public static class Utilities
    {
        public static string SendJSONToAPI(string JSONString, string runID_In, string _runOutputFolderPath, string _apiAddress, string _apiUserName, string _apiPassword, int _apiTimeout, string reason)
        {
            JavaScriptSerializer ser = new JavaScriptSerializer();
            string responseFromServer = "";
            try
            {
                LoggerClass myLog = new LoggerClass();
                myLog.sender = "ROBOTS";
                myLog.reason = reason;
                DateTime dt = DateTime.Now;
                myLog.timestamp = String.Format("{0:G}", dt);
                myLog.message = ser.Deserialize<object>(JSONString);
                File.AppendAllText(_runOutputFolderPath + "\\LogFiles\\" + runID_In + ".json", ser.Serialize(myLog));
                WebRequest request;
                request = WebRequest.Create(_apiAddress);
                request.Credentials = new System.Net.NetworkCredential(_apiUserName, _apiPassword);
                request.Method = "POST";
                request.Timeout = _apiTimeout;
                string postData = JSONString;
                byte[] byteArray = Encoding.UTF8.GetBytes(postData);
                request.ContentType = "application/json";
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                request.ContentLength = byteArray.Length;
                Stream dataStream = request.GetRequestStream();
                dataStream.Write(byteArray, 0, byteArray.Length);
                dataStream.Close();
                WebResponse response = request.GetResponse();
                string status = (((HttpWebResponse)response).StatusDescription);
                dataStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(dataStream);
                responseFromServer = reader.ReadToEnd();
                reader.Close();
                dataStream.Close();
                response.Close();

                myLog.sender = "ENDPOINT";
                dt = DateTime.Now;
                myLog.timestamp = String .Format("{0:G}", dt);
                myLog.message = ser.Deserialize<object>(responseFromServer);
                File.AppendAllText(_runOutputFolderPath + "\\LogFiles\\" + runID_In + ".json", ser.Serialize(myLog));

                return responseFromServer;
            }
            catch (Exception ex)
            {
                throw new Exception("ERROR SEDNING TO ENDPOINT: " + ex.Message);
            }
        } //Http_Lib

        public static string SendJSONToAPI_V2(string JSONString, string runID_In, string _runOutputFolderPath, string _apiAddress, string _apiUserName, string _apiPassword, int _apiTimeout, string reason, string callType)
        {
            JavaScriptSerializer ser = new JavaScriptSerializer();
            string responseFromServer = "";
            try
            {
                LoggerClass myLog = new LoggerClass();
                myLog.sender = "ROBOTS";
                myLog.reason = reason;
                DateTime dt = DateTime.Now;
                myLog.timestamp = String.Format("{0:G}", dt);
                myLog.message = ser.Deserialize<object>(JSONString);
                File.AppendAllText(_runOutputFolderPath + "\\LogFiles\\" + runID_In + ".json", ser.Serialize(myLog));
                WebRequest request;
                request = WebRequest.Create(_apiAddress);
                request.Credentials = new System.Net.NetworkCredential(_apiUserName, _apiPassword);
                request.Method = callType;
                request.Timeout = _apiTimeout;
                string postData = JSONString;
                byte[] byteArray = Encoding.UTF8.GetBytes(postData);
                request.ContentType = "application/json";
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                request.ContentLength = byteArray.Length;
                Stream dataStream = request.GetRequestStream();
                dataStream.Write(byteArray, 0, byteArray.Length);
                dataStream.Close();
                WebResponse response = request.GetResponse();
                string status = (((HttpWebResponse)response).StatusDescription);
                dataStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(dataStream);
                responseFromServer = reader.ReadToEnd();
                reader.Close();
                dataStream.Close();
                response.Close();

                myLog.sender = "ENDPOINT";
                dt = DateTime.Now;
                myLog.timestamp = String.Format("{0:G}", dt);
                myLog.message = ser.Deserialize<object>(responseFromServer);
                File.AppendAllText(_runOutputFolderPath + "\\LogFiles\\" + runID_In + ".json", ser.Serialize(myLog));

                return responseFromServer;
            }
            catch (Exception ex)
            {
                throw new Exception("ERROR SEDNING TO ENDPOINT: " + ex.Message);
            }
        } //Http_Lib

        public static string HttpGet(string url, string runID_In, string _runOutputFolderPath, string _apiUserName, string _apiPassword, int _apiTimeout, string reason)
        {
            JavaScriptSerializer ser = new JavaScriptSerializer();
            try
            {
                LoggerClass myLog = new LoggerClass();
                myLog.sender = "ROBOTS";
                myLog.reason = reason;
                DateTime dt = DateTime.Now;
                myLog.timestamp = String.Format("{0:G}", dt);
                myLog.message = url;

                File.AppendAllText(_runOutputFolderPath + "\\LogFiles\\" + runID_In + ".json", ser.Serialize(myLog));
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "GET";
                request.KeepAlive = true;
                request.Timeout = _apiTimeout;
                request.Credentials = new System.Net.NetworkCredential(_apiUserName, _apiPassword);
                request.ContentType = "application/x-www-form-urlencoded";
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                string myResponse = "";
                using (System.IO.StreamReader sr = new System.IO.StreamReader(response.GetResponseStream()))
                {
                    myResponse = sr.ReadToEnd();
                }

                myLog.sender = "ENDPOINT";
                dt = DateTime.Now;
                myLog.timestamp = String.Format("{0:G}", dt);
                myLog.message = ser.Deserialize<object>(myResponse);
                File.AppendAllText(_runOutputFolderPath + "\\LogFiles\\" + runID_In + ".json", ser.Serialize(myLog));

                return myResponse;
            }
            catch (Exception ex)
            {
                throw new Exception("HTTP ERROR: " + ex.Message);
            }
        } //Http_Lib

        public static void ReadErrorsInResponse(List<LIMSResponse_Errors> myResponseErrors)
        {
            string errorsOut = "";
            if (myResponseErrors.Count > 0)
            {
                foreach (LIMSResponse_Errors myError in myResponseErrors)
                {
                    errorsOut = errorsOut + myError.field + ": " + myError.message + ": " + myError.error_class + "\r\n";
                }
            }
            if (errorsOut != "")
            {
                throw new Exception("ERRORS: " + errorsOut);
            }
        } //LIMS_Workflow 

        public static void getIncInvStringErrors(string[] myBarcodeInputArray)
        {
            try
            {
                string allErrors = "";
                int i = 0;
                string NoPlateErrors = "";
                string NoValueErrors = "";
                string MisreadErrors = "";
                foreach (string oneSlot in myBarcodeInputArray)
                {
                    if (oneSlot == "No Plate")
                    {
                        NoPlateErrors = NoPlateErrors + i.ToString() + ",";
                    }
                    if (oneSlot == "")
                    {
                        NoValueErrors = NoValueErrors + i.ToString() + ",";
                    }
                    if (oneSlot == "Loaded")
                    {
                        MisreadErrors = MisreadErrors + i.ToString() + ",";
                    }
                    i++;
                }
                if (NoPlateErrors != "") allErrors = allErrors + " Missing Plate In Incubator Position(s)" + ":" + NoPlateErrors;
                if (NoValueErrors != "") allErrors = allErrors + " Misread Barcode In Incubator Position(s)" + ":" + NoValueErrors;
                if (MisreadErrors != "") allErrors = allErrors + " Empty Entry For Incubator Position(s)" + ":" + MisreadErrors;
                if (allErrors != "")
                {
                    throw new Exception("INVENTORY ERRORS: " + allErrors);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("getIncInvStringErrors method: " + ex.Message);
            }
        } //Should Get moved to Library !!! DELETE FROM HERE

        //public static DataTable ReadCSV(string path)
        //{
        //    //            if (!File.Exists(path))
        //    //            {
        //    //                return null;
        //    //            }
        //    DataTable dt = new DataTable();
        //    dt.Columns.Add("Sample name", typeof(string));
        //    dt.Columns.Add("A260 Concentration (ng/ul)", typeof(string));
        //    dt.Columns.Add("A260/A230", typeof(string));
        //    dt.Columns.Add("A260/A280", typeof(string));
        //    dt.Columns.Add("DropPlate ID", typeof(string));
        //    dt.Columns.Add("Instrument ID", typeof(string));
        //    dt.Columns.Add("Date", typeof(string));
        //    dt.Columns.Add("Time", typeof(string));
        //    dt.Columns.Add("DropPlate Position", typeof(string));
        //    dt.Columns.Add("Median A260 Concentration (ng/ul)", typeof(string));
        //    string fullPath = Path.GetFullPath(path);
        //    string file = Path.GetFileName(fullPath);
        //    string dir = Path.GetDirectoryName(fullPath);
        //    string connString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=\"" + dir + "\\\";Extended Properties='text;HDR=Yes;IMEX=1;TypeGuessRows=0;ImportMixedTypes=Text;FMT=CSVDelimited(,)';";
        //    string query = "SELECT * FROM " + file;
        //    System.Data.OleDb.OleDbDataAdapter da = new System.Data.OleDb.OleDbDataAdapter(query, connString);
        //    da.Fill(dt);
        //    for (int i = 0; i < dt.Rows.Count; i++)
        //    {
        //        MessageBox.Show(dt.Rows[i]["A260 Concentration (ng/ul)"].ToString() + "::" + dt.Rows[i]["Date"].ToString());
        //        if (dt.Rows[i]["Date"] != System.DBNull.Value && dt.Rows[i]["Time"] != System.DBNull.Value)
        //        {
        //            DateTime date = Convert.ToDateTime(dt.Rows[i]["Date"]);
        //            DateTime time = Convert.ToDateTime(dt.Rows[i]["Time"]);
        //            string sDate = date.ToString("MM/dd/yyyy");
        //            string sTime = time.ToString("hh:mm:ss");
        //            dt.Rows[i]["Date"] = sDate;
        //            dt.Rows[i]["Time"] = sTime;
        //        }
        //        else
        //        {
        //            dt.Rows[i].Delete();
        //        }
        //    }
        //    dt.AcceptChanges();
        //    da.Dispose();
        //    return dt;
        //}//Redundant with Trinean//DLL_Instruments

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
        //            if(count > 15 )
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
        //}//Redundant with Trinean//DLL_Instruments//Legacy

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

                    MessageBox.Show(chip1_DT.Rows[i]["A260 Concentration (ng/ul)"].ToString() + "::" + conc_1.ToString() + "::" + conc1Valid + "::" + chip2_DT.Rows[i]["A260 Concentration (ng/ul)"].ToString() + "::" + conc_2.ToString() + "::" + conc2Valid );

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
                            outTable.Rows[i]["Median A260 Concentration (ng/ul)"] = (Convert.ToDecimal(chip1_DT.Rows[i]["Median A260 Concentration (ng/ul)"]) + Convert.ToDecimal(chip1_DT.Rows[i]["Median A260 Concentration (ng/ul)"])) / 2;
                            break;
                    }
                }
                return outTable;
            }
            catch (Exception ex)
            {
                throw new Exception("ERROR COMBINING DQ OUTPUT FILES" + ex.Message);
            }
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
        }//Redundant with Trinean//DLL_Instruments

        public static StorageDevice DistributePriorities(StorageDevice myDevice)
        {
            SortedDictionary<int, string> myDict = new SortedDictionary<int, string>();
            foreach (Nest _myNest in myDevice.nests)
            {
                if (_myNest.plate_barcode != null)
                {
                    myDict.Add(Convert.ToInt16(_myNest.priority), _myNest.plate_barcode);
                }
            }
            int i = 1;
            foreach (KeyValuePair<int, string> pair in myDict)
            {
                foreach (Nest _myNest in myDevice.nests)
                {
                    if (_myNest.plate_barcode == pair.Value)
                    {
                        _myNest.priority = i.ToString();
                    }
                }
                i++;
            }
            return myDevice;
        }//Inventory

        public static List<Nest> DistributePriorities2(List<Nest> myNests)
        {
            SortedDictionary<int, string> myDict = new SortedDictionary<int, string>();
            foreach (Nest _myNest in myNests)
            {
                if (_myNest.plate_barcode != null)
                {
                    myDict.Add(Convert.ToInt16(_myNest.priority), _myNest.plate_barcode);
                }
            }
            int i = 1;
            foreach (KeyValuePair<int, string> pair in myDict)
            {
                foreach (Nest _myNest in myNests)
                {
                    if (_myNest.plate_barcode == pair.Value)
                    {
                        _myNest.priority = i.ToString();
                    }
                }
                i++;
            }
            return myNests;
        } // Inventory
    }

    public class LoggerClass
    {
        public string sender { get; set; }
        public string reason { get; set; }
        public string timestamp { get; set; }
        public object message { get; set; }
    } //Http_Lib

  
}
