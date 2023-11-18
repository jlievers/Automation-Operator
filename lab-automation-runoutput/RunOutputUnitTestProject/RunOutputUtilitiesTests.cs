using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Linq;

namespace Com.Invitae.LabAutomation
{
    [TestClass]
    public class RunOutputUtilitiesTests
    {
        readonly string testFilePath = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName + "\\RunOutputUnitTestProject\\RunOutputforTesting.json";
        readonly string testFileAfterPath = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName + "\\RunOutputUnitTestProject\\RunOutputforTestingAfter.json";
        readonly string tempTestFilePath = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName + "\\RunOutputUnitTestProject\\tempRunOutputforTesting.json";
        readonly string tempRootTestDirectory = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName + "\\RunOutputUnitTestProject\\test";
        readonly string tempTestDirectory = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName + "\\RunOutputUnitTestProject\\test\\RunOutputFiles\\test";

        [TestMethod]
        public void TestReadAllText()
        {
            //Arrange
            string expectedRawTextString = File.ReadAllText(testFilePath);

            //Act
            string actualRawTextString = Utilities.ReadAllText(testFilePath);

            //Assert
            Assert.AreEqual(expectedRawTextString, actualRawTextString);
        }

        [TestMethod]
        public void TestOpenRunOutputFile()
        {
            //Arrange
            string systemFolderPathExtension = "test";
            string runID = "RunOutputforTesting";
            string expectedRawTextString = File.ReadAllText(testFilePath);

            //Act        
            if (!Directory.Exists(tempTestDirectory))
            {
                Directory.CreateDirectory(tempTestDirectory);
            }          
            File.Copy(testFilePath, tempTestDirectory + "\\RunOutputforTesting.json");
            string actualRawTextString = Utilities.OpenRunOutputFile(tempRootTestDirectory, systemFolderPathExtension, runID);
            Directory.Delete(tempRootTestDirectory, true);

            //Assert
            Assert.AreEqual(expectedRawTextString, actualRawTextString);
        }

        [TestMethod]
        public void TestWriteJson()
        {
            //Arrange
            string expectedJsonString = File.ReadAllText(testFilePath);
            string systemFolderPathExtension = "test";
            string runID = "RunOutputforTesting";

            //Act        
            if (!Directory.Exists(tempTestDirectory))
            {
                Directory.CreateDirectory(tempTestDirectory);
            }
            Utilities.WriteJson(expectedJsonString, tempRootTestDirectory, systemFolderPathExtension, runID);
            string actualJsonString = File.ReadAllText(tempTestDirectory + "\\" + runID + ".json");
            Directory.Delete(tempRootTestDirectory, true);

            //Assert
            Assert.AreEqual(expectedJsonString, actualJsonString);
        }

        [TestMethod]
        public void TestGetAttributeVal1()
        {
            //Arrange
            string runFileText = File.ReadAllText(testFilePath);
            string workflowGroupName = "operation_data_norm";
            string primaryKeyName = "pq_id";
            string primaryKeyValue = "PQ40063";
            string returnValueKey = "extraction_workflow";
            string expectedAttributeReturn = "WF284983";

            //Act
            string actualAttributeReturn = Utilities.GetAttributeVal(runFileText, workflowGroupName, primaryKeyName, primaryKeyValue, returnValueKey);

            //Assert
            Assert.AreEqual(expectedAttributeReturn, actualAttributeReturn);
        }

        [TestMethod]
        public void TestGetAttributeVal2()
        {
            //Arrange
            string workflowGroupName = "operation_data_norm";
            string primaryKeyName = "pq_id";
            string primaryKeyValue = "PQ40063";
            string returnValueKey = "extraction_workflow";
            string expectedAttributeReturn = "WF284983";

            //Act
            string actualAttributeReturn = Utilities.GetAttributeVal(true, testFilePath, workflowGroupName, primaryKeyName, primaryKeyValue, returnValueKey);

            //Assert
            Assert.AreEqual(expectedAttributeReturn, actualAttributeReturn);
        }

        //[TestMethod]
        //public void TestSetAttributeVal1()
        //{
        //    //Arrange
        //    string runFileText = File.ReadAllText(testFilePath);
        //    string workflowGroupName = "operation_data_norm";
        //    string primaryKeyName = "pq_id";
        //    string primaryKeyValue = "PQ40063";
        //    string setValueKey = "extraction_workflow";
        //    string setValueValue = "WFXXXXXX";
        //    string expectedJSONOutput = File.ReadAllText(testFileAfterPath);

        //    //Act
        //    string actualJSONOutput = Utilities.SetAttributeVal(runFileText, workflowGroupName, primaryKeyName, primaryKeyValue, setValueKey, setValueValue);

        //    //Assert
        //    Assert.AreEqual(expectedJSONOutput, actualJSONOutput);
        //}

        [TestMethod]
        public void TestSetAttributeVal2()
        {
            //Arrange
            string workflowGroupName = "operation_data_norm";
            string primaryKeyName = "pq_id";
            string primaryKeyValue = "PQ40063";
            string setValueKey = "extraction_workflow";
            string expectedAttributeValue = "WFXXXXXX";

            //Act
            File.Copy(testFilePath, tempTestFilePath, true);
            Utilities.SetAttributeVal(true, tempTestFilePath, workflowGroupName, primaryKeyName, primaryKeyValue, setValueKey, expectedAttributeValue);
            JObject runOutputFileObject = JObject.Parse(File.ReadAllText(tempTestFilePath));
            JObject match = runOutputFileObject[workflowGroupName].Values<JObject>()
                .Where(p => p[primaryKeyName].Value<string>() == primaryKeyValue)
                .FirstOrDefault();
            string actualAttributeValue = match[setValueKey].ToString();           
            File.Delete(tempTestFilePath);

            //Assert
            Assert.AreEqual(expectedAttributeValue, actualAttributeValue);
        }

        [TestMethod]
        public void TestSetWorkfloworPlateObject()
        {
            //Arrange                      
            string workflowGroupName = "operation_data_norm";
            string primaryKeyName = "pq_id";
            string primaryKeyValue = "PQ40063";
            OperationData_Norm normObject = new OperationData_Norm();
            normObject.pq_id = "PQXXXXX";
            normObject.tube_rack_id = "FEXXXXXXXX";
            normObject.scanfile_name = "FEXXXXXXXX_YYYYMmmDd_H-mm-ss";
            normObject.extraction_workflow = "WFXXXXXX";
            normObject.process_type = "Shearing";
            normObject.timestamp_enter = "MM/DD/YYYY H:mm AM/PM";
            normObject.timestamp_exit = "MM/DD/YYYY H:mm AM/PM";
            normObject.quant_metric = "QDXXXXX";
            normObject.gdna_norm_dq1_plate_id = "XXXXXXXXXXXXXX";
            normObject.gdna_norm_dq2_plate_id = "XXXXXXXXXXXXXX";
            normObject.combined_trinean_file_name = "XXXXXXXXXXXXXX+XXXXXXXXXXXXXX.csv";
            normObject.post_extraction_norm_worklist_filename = "ExtractonNorm_PQXXXXX.csv";
            normObject.status = "active";
            string expectedNormObjectString = JsonConvert.SerializeObject(normObject);
            string expectedKeyValue = "PQXXXXX";

            //Act
            File.Copy(testFilePath, tempTestFilePath, true);
            Utilities.SetWorkfloworPlateObject(tempTestFilePath, workflowGroupName, primaryKeyName, primaryKeyValue, normObject);          
            JObject runOutputFileObject = JObject.Parse(File.ReadAllText(tempTestFilePath));
            JObject match = runOutputFileObject[workflowGroupName].Values<JObject>()
                .Where(p => p[primaryKeyName].Value<string>() == expectedKeyValue)
                .FirstOrDefault();
            string actualNormObjectString = JsonConvert.SerializeObject(match);
            File.Delete(tempTestFilePath);

            //Assert
            Assert.AreEqual(expectedNormObjectString, actualNormObjectString);
        }

        [TestMethod]
        public void TestInvalidFilePathProvided()
        {
            //Arrange
            string runFilePath = string.Empty;
            string systemFolderPath = string.Empty;
            string systemFolderPathExtension = string.Empty;
            string runID = string.Empty;
            string jsonString = File.ReadAllText(testFilePath);
            string workflowGroupName = "operation_data_norm";
            string primaryKeyName = "pq_id";
            string primaryKeyValue = "PQ40063";
            string returnValueKey = "extraction_workflow";
            string setValueKey = "extraction_workflow";
            string setValueValue = "WFXXXXXX";
            object workfloworPlateObject = new OperationData_Norm();

            //Act + Assert
            Assert.ThrowsException<ArgumentException>(() => Utilities.ReadAllText(runFilePath));
            Assert.ThrowsException<FileNotFoundException>(() => Utilities.OpenRunOutputFile(systemFolderPath, systemFolderPathExtension, runID));
            Assert.ThrowsException<DirectoryNotFoundException>(() => Utilities.WriteJson(jsonString, systemFolderPath, systemFolderPathExtension, runID));
            Assert.ThrowsException<ArgumentException>(() => Utilities.GetAttributeVal(true, runFilePath, workflowGroupName, primaryKeyName, primaryKeyValue, returnValueKey));
            Assert.ThrowsException<ArgumentException>(() => Utilities.SetAttributeVal(true, runFilePath, workflowGroupName, primaryKeyName, primaryKeyValue, setValueKey, setValueValue));
            Assert.ThrowsException<ArgumentException>(() => Utilities.SetWorkfloworPlateObject(runFilePath, workflowGroupName, primaryKeyName, primaryKeyValue, workfloworPlateObject));
        }

        [TestMethod]
        public void TestInvalidFileTextProvided()
        {
            //Arrange
            string runFileText = string.Empty;
            string workflowGroupName = "operation_data_norm";
            string primaryKeyName = "pq_id";
            string primaryKeyValue = "PQ40063";
            string returnValueKey = "extraction_workflow";
            string setValueKey = "extraction_workflow";
            string setValueValue = "WFXXXXXX";

            //Act + Assert
            Assert.ThrowsException<JsonReaderException>(() => Utilities.GetAttributeVal(runFileText, workflowGroupName, primaryKeyName, primaryKeyValue, returnValueKey));
            Assert.ThrowsException<JsonReaderException>(() => Utilities.SetAttributeVal(runFileText, workflowGroupName, primaryKeyName, primaryKeyValue, setValueKey, setValueValue));
        }

        [TestMethod]
        public void TestInvalidWorkflowGroupProvided()
        {
            //Arrange
            string runFilePath = tempTestFilePath;
            string runFileText = File.ReadAllText(testFilePath);
            string workflowGroupName = "dummy_workflow";
            string primaryKeyName = "pq_id";
            string primaryKeyValue = "PQ40063";
            string returnValueKey = "extraction_workflow";
            string setValueKey = "extraction_workflow";
            string setValueValue = "WFXXXXXX";
            object workfloworPlateObject = new OperationData_Norm();
            File.Copy(testFilePath, tempTestFilePath, true);

            //Act + Assert
            Assert.ThrowsException<NullReferenceException>(() => Utilities.GetAttributeVal(runFileText, workflowGroupName, primaryKeyName, primaryKeyValue, returnValueKey));
            Assert.ThrowsException<NullReferenceException>(() => Utilities.GetAttributeVal(true, runFilePath, workflowGroupName, primaryKeyName, primaryKeyValue, returnValueKey));
            Assert.ThrowsException<NullReferenceException>(() => Utilities.SetAttributeVal(runFileText, workflowGroupName, primaryKeyName, primaryKeyValue, setValueKey, setValueValue));
            Assert.ThrowsException<NullReferenceException>(() => Utilities.SetAttributeVal(true, runFilePath, workflowGroupName, primaryKeyName, primaryKeyValue, setValueKey, setValueValue));
            Assert.ThrowsException<NullReferenceException>(() => Utilities.SetWorkfloworPlateObject(runFilePath, workflowGroupName, primaryKeyName, primaryKeyValue, workfloworPlateObject));

            //Cleanup
            File.Delete(tempTestFilePath);
        }

        [TestMethod]
        public void TestInvalidPrimaryKeyProvided()
        {
            //Arrange
            string runFilePath = tempTestFilePath;
            string runFileText = File.ReadAllText(testFilePath);
            string workflowGroupName = "operation_data_norm";
            string primaryKeyName = "dummy_key";
            string primaryKeyValue = "PQ40063";
            string returnValueKey = "extraction_workflow";
            string setValueKey = "extraction_workflow";
            string setValueValue = "WFXXXXXX";
            object workfloworPlateObject = new OperationData_Norm();
            File.Copy(testFilePath, tempTestFilePath, true);

            //Act + Assert
            Assert.ThrowsException<ArgumentNullException>(() => Utilities.GetAttributeVal(runFileText, workflowGroupName, primaryKeyName, primaryKeyValue, returnValueKey));
            Assert.ThrowsException<ArgumentNullException>(() => Utilities.GetAttributeVal(true, runFilePath, workflowGroupName, primaryKeyName, primaryKeyValue, returnValueKey));
            Assert.ThrowsException<ArgumentNullException>(() => Utilities.SetAttributeVal(runFileText, workflowGroupName, primaryKeyName, primaryKeyValue, setValueKey, setValueValue));
            Assert.ThrowsException<ArgumentNullException>(() => Utilities.SetAttributeVal(true, runFilePath, workflowGroupName, primaryKeyName, primaryKeyValue, setValueKey, setValueValue));
            Assert.ThrowsException<ArgumentNullException>(() => Utilities.SetWorkfloworPlateObject(runFilePath, workflowGroupName, primaryKeyName, primaryKeyValue, workfloworPlateObject));

            //Cleanup
            File.Delete(tempTestFilePath);
        }

        [TestMethod]
        public void TestInvalidPrimaryValueProvided()
        {
            //Arrange
            string runFilePath = tempTestFilePath;
            string runFileText = File.ReadAllText(testFilePath);
            string workflowGroupName = "operation_data_norm";
            string primaryKeyName = "pq_id";
            string primaryKeyValue = "dummy_value";
            string returnValueKey = "extraction_workflow";
            string setValueKey = "extraction_workflow";
            string setValueValue = "WFXXXXXX";
            object workfloworPlateObject = new OperationData_Norm();
            File.Copy(testFilePath, tempTestFilePath, true);

            //Act + Assert
            Assert.ThrowsException<Exception>(() => Utilities.GetAttributeVal(runFileText, workflowGroupName, primaryKeyName, primaryKeyValue, returnValueKey));
            Assert.ThrowsException<Exception>(() => Utilities.GetAttributeVal(true, runFilePath, workflowGroupName, primaryKeyName, primaryKeyValue, returnValueKey));
            Assert.ThrowsException<Exception>(() => Utilities.SetAttributeVal(runFileText, workflowGroupName, primaryKeyName, primaryKeyValue, setValueKey, setValueValue));
            Assert.ThrowsException<Exception>(() => Utilities.SetAttributeVal(true, runFilePath, workflowGroupName, primaryKeyName, primaryKeyValue, setValueKey, setValueValue));
            Assert.ThrowsException<Exception>(() => Utilities.SetWorkfloworPlateObject(runFilePath, workflowGroupName, primaryKeyName, primaryKeyValue, workfloworPlateObject));

            //Cleanup
            File.Delete(tempTestFilePath);
        }

        [TestMethod]
        public void TestInvalidReturnKeyProvided()
        {
            //Arrange
            string runFilePath = testFilePath;
            string runFileText = File.ReadAllText(testFilePath);
            string workflowGroupName = "operation_data_norm";
            string primaryKeyName = "pq_id";
            string primaryKeyValue = "PQ40063";
            string returnValueKey = "dummy_key";

            //Act + Assert
            Assert.ThrowsException<NullReferenceException>(() => Utilities.GetAttributeVal(runFileText, workflowGroupName, primaryKeyName, primaryKeyValue, returnValueKey));
            Assert.ThrowsException<NullReferenceException>(() => Utilities.GetAttributeVal(true, runFilePath, workflowGroupName, primaryKeyName, primaryKeyValue, returnValueKey));
        }

        [TestMethod]
        public void TestInvalidSetKeyProvided()
        {
            //Arrange
            string runFilePath = tempTestFilePath;
            string runFileText = File.ReadAllText(testFilePath);
            string workflowGroupName = "operation_data_norm";
            string primaryKeyName = "pq_id";
            string primaryKeyValue = "PQ40063";
            string setValueKey = "dummy_key";
            string setValueValue = "WFXXXXXX";
            File.Copy(testFilePath, tempTestFilePath, true);

            //Act + Assert
            Assert.ThrowsException<Exception>(() => Utilities.SetAttributeVal(runFileText, workflowGroupName, primaryKeyName, primaryKeyValue, setValueKey, setValueValue));
            Assert.ThrowsException<Exception>(() => Utilities.SetAttributeVal(true, runFilePath, workflowGroupName, primaryKeyName, primaryKeyValue, setValueKey, setValueValue));

            //Cleanup
            File.Delete(tempTestFilePath);
        }
    }
}
