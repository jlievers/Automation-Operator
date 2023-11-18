using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Windows.Forms;
using System.Threading;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using System.Security.Permissions;

namespace TestConsole
{
    class TestConsole
    {
        static void Main(string[] args)
        {
            Console.WriteLine("start test");
            string jsonFileText = DLL_RunOutput.Utilities.OpenRunOutputFile("C:\\Users\\labuser.LOCUSDEV\\Desktop\\RunOutput", "", "9_17_2018");
            Console.WriteLine("jsonFileText: " + jsonFileText);

            string attribute = DLL_RunOutput.Utilities.GetAttributeVal(jsonFileText, "operation_data_norm", "pq_id", "PG209768", "scanfile_name");
            MessageBox.Show("Attribute: " + attribute);

            //string userInput1 = Microsoft.VisualBasic.Interaction.InputBox("Enter an atribute identifier", "Attribute ID", "attribute", -1, -1);
            //string userInput2 = Microsoft.VisualBasic.Interaction.InputBox("Enter an atribute value", "Attribute Value", "value", -1, -1);

            string JSONOutputString = DLL_RunOutput.Utilities.SetAttributeVal(jsonFileText, "plate_data", "workflow", "WF57166", "er_at_plate", "eff");
            DLL_RunOutput.Utilities.WriteJson(JSONOutputString, "C:\\Users\\labuser.LOCUSDEV\\Desktop\\RunOutput\\","", "Library_Example");

            Console.WriteLine("----- click any key to exit -----");
            Console.ReadLine();
        }
    }
}
