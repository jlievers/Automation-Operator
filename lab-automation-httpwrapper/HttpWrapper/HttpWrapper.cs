using System;
using System.Text;
using System.IO;
using System.Net;
using System.Web.Script.Serialization;

namespace Com.Invitae.LabAutomation
{
    public static class HttpWrapper
    {
        public static string SendJSONToAPI(string JSONString, string runID_In, string _runOutputFolderPath, string _apiAddress, string _apiUserName, string _apiPassword, int _apiTimeout, string reason)
        {
            LoggerClass myLog = new LoggerClass();
            JavaScriptSerializer ser = new JavaScriptSerializer();
            
            string response = "";
            try
            {
                response = callOnly(JSONString, runID_In, _runOutputFolderPath, _apiAddress, _apiUserName, _apiPassword, _apiTimeout, reason);
              
            }
            catch (WebException ex)
            {
                string myError = "";
                myLog.sender = "HTTP ERROR";
                DateTime dt = DateTime.Now;
                myLog.timestamp = String.Format("{0:G}", dt);
                if (ex.Response != null)
                {
                    HttpStatusCode statusCode = ((HttpWebResponse)ex.Response).StatusCode;
                    myError = "CODE:" + (int)statusCode + " - " + statusCode.ToString() + " STATUS: " + ex.Status + " MESSAGE: " + ex.Message;
                }
                else
                {
                    myError = "STATUS: " + ex.Status + " MESSAGE: " + ex.Message;
                }
                myLog.message = myError;
                File.AppendAllText(_runOutputFolderPath + "\\LogFiles\\" + runID_In + ".json", ser.Serialize(myLog));
                try
                {
                    response = callOnly(JSONString, runID_In, _runOutputFolderPath, _apiAddress, _apiUserName, _apiPassword, _apiTimeout, reason);
                }
                catch (Exception ex1)
                {
                    throw new Exception("ERROR SEDNING TO ENDPOINT: " + ex1.Message);
                }
            }
            return response;
        }

        public static string SendJSONToAPI_origional(string JSONString, string runID_In, string _runOutputFolderPath, string _apiAddress, string _apiUserName, string _apiPassword, int _apiTimeout, string reason)
        {
            JavaScriptSerializer ser = new JavaScriptSerializer();
            ser.MaxJsonLength = Int32.MaxValue;
            LoggerClass myLog = new LoggerClass();
            string responseFromServer = "";
            try
            {

                myLog.sender = "ROBOTS";
                myLog.reason = reason;
                DateTime dt = DateTime.Now;
                myLog.timestamp = String.Format("{0:G}", dt);
                myLog.endpoint = _apiAddress;
                myLog.message = ser.Deserialize<object>(JSONString);
                File.AppendAllText(_runOutputFolderPath + "\\LogFiles\\" + runID_In + ".json", ser.Serialize(myLog));

                WebRequest request;
                request = WebRequest.Create(_apiAddress);
                request.Credentials = new NetworkCredential(_apiUserName, _apiPassword);
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

                //string status = (((HttpWebResponse)response).StatusDescription);

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
        }

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
                myLog.endpoint = _apiAddress;
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
                string status = (((HttpWebResponse)response).StatusDescription);//not used
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
        }

        public static string SendJSONToAPI_ToggleLog(string JSONString, string runID_In, string _runOutputFolderPath, string _apiAddress, string _apiUserName, string _apiPassword, int _apiTimeout, string reason, string callType, bool logOrNot)
        {
            JavaScriptSerializer ser = new JavaScriptSerializer();
            string responseFromServer = "";
            try
            {
                LoggerClass myLog = new LoggerClass();
                if (logOrNot)
                {

                    myLog.sender = "ROBOTS";
                    myLog.reason = reason;
                    DateTime dt = DateTime.Now;
                    myLog.timestamp = String.Format("{0:G}", dt);
                    myLog.endpoint = _apiAddress;
                    myLog.message = ser.Deserialize<object>(JSONString);
                    File.AppendAllText(_runOutputFolderPath + "\\LogFiles\\" + runID_In + ".json", ser.Serialize(myLog));
                }
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
                if (logOrNot)
                {
                    myLog.sender = "ENDPOINT";
                    DateTime dt = DateTime.Now;
                    dt = DateTime.Now;
                    myLog.timestamp = String.Format("{0:G}", dt);
                    myLog.message = ser.Deserialize<object>(responseFromServer);
                    File.AppendAllText(_runOutputFolderPath + "\\LogFiles\\" + runID_In + ".json", ser.Serialize(myLog));
                }

                return responseFromServer;
            }
            catch (Exception ex)
            {
                throw new Exception("ERROR SEDNING TO ENDPOINT: " + ex.Message);
            }
        }

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
                myLog.endpoint = url;
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
                using (System.IO.StreamReader sr = new StreamReader(response.GetResponseStream()))
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
        }

        public static string HttpGet_ToggleLog(string url, string runID_In, string _runOutputFolderPath, string _apiUserName, string _apiPassword, int _apiTimeout, string reason, bool logOrNot)
        {
            JavaScriptSerializer ser = new JavaScriptSerializer();
            try
            {
                LoggerClass myLog = new LoggerClass();
                if (logOrNot)
                {
                    myLog.sender = "ROBOTS";
                    myLog.reason = reason;
                    DateTime dt = DateTime.Now;
                    myLog.timestamp = String.Format("{0:G}", dt);
                    myLog.message = url;
                    myLog.endpoint = url;
                    File.AppendAllText(_runOutputFolderPath + "\\LogFiles\\" + runID_In + ".json", ser.Serialize(myLog));
                }
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

                if (logOrNot)
                {
                    myLog.sender = "ENDPOINT";
                    DateTime dt = DateTime.Now;
                    dt = DateTime.Now;
                    myLog.timestamp = String.Format("{0:G}", dt);
                    myLog.message = ser.Deserialize<object>(myResponse);
                    File.AppendAllText(_runOutputFolderPath + "\\LogFiles\\" + runID_In + ".json", ser.Serialize(myLog));
                }

                return myResponse;
            }
            catch (Exception ex)
            {
                throw new Exception("HTTP ERROR: " + ex.Message);
            }
        }

        private static string callOnly(string JSONString, string runID_In, string _runOutputFolderPath, string _apiAddress, string _apiUserName, string _apiPassword, int _apiTimeout, string reason)
        {

            JavaScriptSerializer ser = new JavaScriptSerializer();
            ser.MaxJsonLength = Int32.MaxValue;
            LoggerClass myLog = new LoggerClass();

            myLog.sender = "ROBOTS";
            myLog.reason = reason;
            DateTime dt = DateTime.Now;
            myLog.timestamp = String.Format("{0:G}", dt);
            myLog.endpoint = _apiAddress;
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

            //string status = (((HttpWebResponse)response).StatusDescription);

            dataStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(dataStream);
            string responseFromServer = reader.ReadToEnd();
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

        public static string SendJSONToAPI_AutoRetry(string JSONString, string runID_In, string _runOutputFolderPath, string _apiAddress, string _apiUserName, string _apiPassword, int _apiTimeout, string reason)
        {
            LoggerClass myLog = new LoggerClass();
            JavaScriptSerializer ser = new JavaScriptSerializer();
            string response = "";
            try
            {
                response = callOnly(JSONString, runID_In, _runOutputFolderPath, _apiAddress, _apiUserName, _apiPassword, _apiTimeout, reason);
            }
            catch (WebException ex)
            {
                string myError = "";
                myLog.sender = "HTTP ERROR";
                DateTime dt = DateTime.Now;
                myLog.timestamp = String.Format("{0:G}", dt);
                if (ex.Response != null)
                {
                    HttpStatusCode statusCode = ((HttpWebResponse)ex.Response).StatusCode;
                    myError = "CODE:" + (int)statusCode + " - " + statusCode.ToString() + " STATUS: " + ex.Status + " MESSAGE: " + ex.Message;
                }
                else
                {
                    myError = "STATUS: " + ex.Status + " MESSAGE: " + ex.Message;
                }
                myLog.message = myError;
                File.AppendAllText(_runOutputFolderPath + "\\LogFiles\\" + runID_In + ".json", ser.Serialize(myLog));
                try
                {
                    response = callOnly(JSONString, runID_In, _runOutputFolderPath, _apiAddress, _apiUserName, _apiPassword, _apiTimeout, reason);
                }
                catch (Exception ex1)
                {
                    throw new Exception("ERROR SEDNING TO ENDPOINT: " + ex1.Message);
                }
            }
            return response;
        }



        //Below here is my attempt at creating a grand unified API method. Above here is the flotsum of 5 years of dev that cannot be touched due to its presence everywhere in the lab

        public static string CallEndpoint(string JSONString, string runID_In, string _runOutputFolderPath, string _apiAddress, string _apiUserName, string _apiPassword, int _apiTimeout, string reason, string callType, bool logOrNot)
        {
            LoggerClass myLog = new LoggerClass();
            JavaScriptSerializer ser = new JavaScriptSerializer();
            string response = "";
            bool end = false;
            int i = 0;
            while(!end)
            {
                try
                {
                    response = callOnlyV2(JSONString, runID_In, _runOutputFolderPath, _apiAddress, _apiUserName, _apiPassword, _apiTimeout, reason, callType, logOrNot);
                    end = true;
                }
                catch (WebException ex)
                {
                    string myError = "";
                    myLog.sender = "HTTP ERROR";
                    DateTime dt = DateTime.Now;
                    myLog.timestamp = String.Format("{0:G}", dt);

                    if (ex.Response != null)
                    {
                        string resp = new StreamReader(ex.Response.GetResponseStream()).ReadToEnd().ToString();
                        HttpStatusCode statusCode = ((HttpWebResponse)ex.Response).StatusCode;
                        myError = "CODE:" + (int)statusCode + " - " + statusCode.ToString() + " STATUS: " + ex.Status + " MESSAGE: " + ex.Message
                            + ":" + resp;
                    }
                    else
                    {
                        myError = "STATUS: " + ex.Status + " MESSAGE: " + ex.Message;
                    }
                    myLog.message = myError;
                    File.AppendAllText(_runOutputFolderPath + "\\LogFiles\\" + runID_In + ".json", ser.Serialize(myLog));
                    i++;
                    if (i > 1)
                    {
                        throw new Exception(myError);
                    }
                }
            }
            return response;
        }

        private static string callOnlyV2(string JSONString, string runID_In, string _runOutputFolderPath, string _apiAddress, string _apiUserName, string _apiPassword, int _apiTimeout, string reason, string callType, bool logOrNot)
        {

            JavaScriptSerializer ser = new JavaScriptSerializer();
            ser.MaxJsonLength = Int32.MaxValue;
            LoggerClass myLog = new LoggerClass();

            if (logOrNot)
            {
                myLog.sender = "ROBOTS";
                myLog.reason = reason;
                DateTime dt = DateTime.Now;
                myLog.timestamp = String.Format("{0:G}", dt);
                myLog.endpoint = _apiAddress;
                myLog.message = ser.Deserialize<object>(JSONString);
                File.AppendAllText(_runOutputFolderPath + "\\LogFiles\\" + runID_In + ".json", ser.Serialize(myLog));
            }
            string responseFromServer = "";

            if (callType == "GET")
            {
                HttpWebRequest request1 = (HttpWebRequest)WebRequest.Create(_apiAddress);
                request1.Method = "GET";
                request1.KeepAlive = true;
                request1.Timeout = _apiTimeout;
                request1.Credentials = new System.Net.NetworkCredential(_apiUserName, _apiPassword);
                request1.ContentType = "application/x-www-form-urlencoded";
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                HttpWebResponse response1 = (HttpWebResponse)request1.GetResponse();
                using (System.IO.StreamReader sr = new System.IO.StreamReader(response1.GetResponseStream()))
                {
                    responseFromServer = sr.ReadToEnd();
                }
            }
            else
            {
                WebRequest request2;
                request2 = WebRequest.Create(_apiAddress);
                request2.Credentials = new System.Net.NetworkCredential(_apiUserName, _apiPassword);
                request2.Method = callType;
                request2.Timeout = _apiTimeout;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                string postData = JSONString;
                byte[] byteArray = Encoding.UTF8.GetBytes(postData);
                request2.ContentType = "application/json";
                request2.ContentLength = byteArray.Length;

                Stream dataStream = request2.GetRequestStream();
                dataStream.Write(byteArray, 0, byteArray.Length);
                dataStream.Close();
                WebResponse response = request2.GetResponse();
                dataStream = response.GetResponseStream();
               
                StreamReader reader = new StreamReader(dataStream);
                responseFromServer = reader.ReadToEnd();

                reader.Close();
                dataStream.Close();
                response.Close();
            }


            if (logOrNot)
            {
                myLog.sender = "ENDPOINT";
                DateTime dt = DateTime.Now;
                myLog.timestamp = String.Format("{0:G}", dt);
                myLog.message = ser.Deserialize<object>(responseFromServer);
                File.AppendAllText(_runOutputFolderPath + "\\LogFiles\\" + runID_In + ".json", ser.Serialize(myLog));
            }

            return responseFromServer;
        }
    }

    public class LoggerClass
    {
        public string sender { get; set; }
        public string reason { get; set; }
        public string endpoint { get; set; }
        public string timestamp { get; set; }
        public object message { get; set; }
    }
}
