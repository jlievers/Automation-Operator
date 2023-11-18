using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLL_Instruments
{
    public static class Rosalind
    {
        public static void ReadErrorsInResponse(List<RosalindResponse_Errors> myResponseErrors)
        {
            string errorsOut = "";
            if (myResponseErrors.Count > 0)
            {
                foreach (RosalindResponse_Errors myError in myResponseErrors)
                {
                    errorsOut = errorsOut + myError.field + ": " + myError.message + ": " + myError.error_class + "\r\n";
                }
            }
            if (errorsOut != "")
            {
                throw new Exception("ds ERRORS: " + errorsOut);
            }
        }
    }

    public class RosalindWorklistResponse
    {
        public List<RosalindResponse_Errors> errors { get; set; }
        public Device_XL20 data { get; set; }
    }

    public class RosalindResponse_Errors
    {
        public string field { get; set; }
        public object error_class { get; set; }
        public string message { get; set; }
    }
}
