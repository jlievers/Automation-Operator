using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;

namespace Com.Invitae.LabAutomation
{
    [TestClass]
    public class InventoryTests
    {
        readonly string testFilePath = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName + "\\InventoryUnitTestProject\\SystemInventoryforTesting.json";
        readonly string tempTestFilePath = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName + "\\InventoryUnitTestProject\\tempSystemInventoryforTesting.json";
        readonly System.Web.Script.Serialization.JavaScriptSerializer ser = new System.Web.Script.Serialization.JavaScriptSerializer();

        [TestMethod]
        public void TestReadAllText()
        {
            //Arrange
            string expectedRawTextString = File.ReadAllText(testFilePath);

            //Act
            string actualRawTextString = Inventory.ReadAllText(testFilePath);

            //Assert
            Assert.AreEqual(expectedRawTextString, actualRawTextString);
        }

        [TestMethod]
        public void TestGetAvailableNest()
        {
            //Arrange
            string deviceName = "stack1";
            int expectedNestNumber = 1;

            //Act        
            int actualNestNumber = Inventory.GetAvailableNest(testFilePath, deviceName);

            //Assert
            Assert.AreEqual(expectedNestNumber, actualNestNumber);
        }

        [TestMethod]
        public void TestAddLabwareToNest()
        {
            //Arrange
            string deviceName = "centrifuge1";
            int nest_num = 1;
            string expectedBarcode = "FEXXXXXXXX";
            string expectedWorkflow_ID = "WFXXXXXX";
            string expectedPriority = "1";
            string actualBarcode = "";
            string actualWorkflow_ID = "";
            string actualPriority = "";

            //Act
            File.Copy(testFilePath, tempTestFilePath, true);
            Inventory.AddLabwareToNest(tempTestFilePath, deviceName, nest_num, expectedBarcode, expectedWorkflow_ID);
            SystemInventory modifiedInventory = ser.Deserialize<SystemInventory>(File.ReadAllText(tempTestFilePath));
            foreach (Device myDevice in modifiedInventory.devices)
            {
                if (myDevice.device_name == deviceName)
                {
                    foreach (Nest myNest in myDevice.nests)
                    {
                        if (myNest.nest_number == nest_num)
                        {
                            actualBarcode = myNest.plate_barcode;
                            actualWorkflow_ID = myNest.workflow_id;
                            actualPriority = myNest.priority;
                        }
                    }
                }
            }
            File.Delete(tempTestFilePath);

            //Assert
            Assert.AreEqual(expectedBarcode, actualBarcode);
            Assert.AreEqual(expectedWorkflow_ID, actualWorkflow_ID);
            Assert.AreEqual(expectedPriority, actualPriority);
        }

        [TestMethod]
        public void TestGetNextNestPosition()
        {
            //Arrange
            string deviceName = "stack1";
            string expectedBarcode = "PQXXXXX";
            int expectedNestNumber = 5;

            //Act
            int actualNestNumber = Inventory.GetNextNestPosition(testFilePath, deviceName, out string actualBarcode);

            //Assert
            Assert.AreEqual(expectedBarcode, actualBarcode);
            Assert.AreEqual(expectedNestNumber, actualNestNumber);
        }

        [TestMethod]
        public void TestRemoveLabwareFromNest()
        {
            //Arrange
            string deviceName = "stack1";
            int nest_num = 5;
            string expectedBarcode = "PQXXXXX";
            List<string> actualNestProperties = new List<string>(5);
            int expectedLengthofList = 5;

            //Act
            File.Copy(testFilePath, tempTestFilePath, true);
            string actualReturnedBarcode = Inventory.RemoveLabwareFromNest(tempTestFilePath, deviceName, nest_num);
            SystemInventory modifiedInventory = ser.Deserialize<SystemInventory>(File.ReadAllText(tempTestFilePath));
            foreach (Device myDevice in modifiedInventory.devices)
            {
                if (myDevice.device_name == deviceName)
                {
                    foreach (Nest myNest in myDevice.nests)
                    {
                        if (myNest.nest_number == nest_num)
                        {
                            actualNestProperties.Add(myNest.workflow_id);
                            actualNestProperties.Add(myNest.plate_barcode);
                            actualNestProperties.Add(myNest.priority);
                            actualNestProperties.Add(myNest.assay_id);
                            actualNestProperties.Add(myNest.status);
                        }
                    }
                }
            }
            File.Delete(tempTestFilePath);

            //Assert
            Assert.AreEqual(expectedBarcode, actualReturnedBarcode);
            Assert.AreEqual(actualNestProperties.Count, expectedLengthofList);
            foreach (string nestProperty in actualNestProperties)
            {
                Assert.IsNull(nestProperty);
            }
        }

        [TestMethod]
        public void TestGetNestAttribute()
        {
            //Arrange
            string attributeName = "plate_barcode";
            string deviceName = "stack1";
            int nest_num = 5;
            string expectedAttributeValue = "PQXXXXX";

            //Act
            string actualAttributeValue = Inventory.GetNestAttribute(attributeName, testFilePath, deviceName, nest_num);

            //Assert
            Assert.AreEqual(expectedAttributeValue, actualAttributeValue);
        }

        [TestMethod]
        public void TestSetNestAttribute()
        {
            //Arrange
            string attributeName = "plate_barcode";
            string expectedAttributeValue = "PQYYYYY";
            string deviceName = "stack1";
            int nest_num = 5;
            string actualAttributeValue = "";

            //Act
            File.Copy(testFilePath, tempTestFilePath, true);
            Inventory.SetNestAttribute(attributeName, expectedAttributeValue, tempTestFilePath, deviceName, nest_num);
            SystemInventory modifiedInventory = ser.Deserialize<SystemInventory>(File.ReadAllText(tempTestFilePath));
            foreach (Device myDevice in modifiedInventory.devices)
            {
                if (myDevice.device_name == deviceName)
                {
                    foreach (Nest myNest in myDevice.nests)
                    {
                        if (myNest.nest_number == nest_num)
                        {
                            actualAttributeValue = myNest.plate_barcode;                            
                        }
                    }
                }
            }
            File.Delete(tempTestFilePath);

            //Assert
            Assert.AreEqual(expectedAttributeValue, actualAttributeValue);
        }

        [TestMethod]
        public void TestGetNestNumAndDeviceFromBarcode()
        {
            //Arrange
            string barcode = "PQXXXXX";
            string expectedNestNumber = "5";
            string expectedDeviceName = "stack1";

            //Act
            string[] actualNestNumberandDevice = Inventory.GetNestNumAndDeviceFromBarcode(barcode, testFilePath);

            //Assert
            Assert.AreEqual(expectedNestNumber, actualNestNumberandDevice[0]);
            Assert.AreEqual(expectedDeviceName, actualNestNumberandDevice[1]);
        }

        [TestMethod]
        public void TestSetDeviceObject()
        {
            //Arrange
            Device deviceObject = new Device();
            deviceObject.device_name = "centrifuge1";
            deviceObject.device_type = "centrifuge";
            deviceObject.lm_number = "LMXXXXX";
            deviceObject.enabled = true;
            deviceObject.num_nests = 1;
            Nest nestObject = new Nest();
            nestObject.nest_number = 1;
            nestObject.workflow_id = "WFXXXXXX";
            nestObject.plate_barcode = "FEXXXXXXXX";
            nestObject.priority = "1";
            nestObject.assay_id = "AYXX";
            nestObject.status = "active";
            deviceObject.nests = new List<Nest>(1)
            {
                nestObject
            };
            string deviceName = "centrifuge1";
            Device devicetoReference = new Device();
            SystemInventory inventory = ser.Deserialize<SystemInventory>(File.ReadAllText(testFilePath));
            foreach (Device device in inventory.devices)
            {
                if (device.device_name == deviceName)
                {
                    devicetoReference = device;
                }
            }
            string expectedDeviceObjectString = ser.Serialize(deviceObject);
            string actualDeviceObjectString = "";

            //Act
            File.Copy(testFilePath, tempTestFilePath, true);
            Inventory.SetDeviceObject(tempTestFilePath, deviceObject, deviceName, devicetoReference, out bool error, out Device outDevice);
            SystemInventory modifiedInventory = ser.Deserialize<SystemInventory>(File.ReadAllText(tempTestFilePath));
            foreach (Device device in modifiedInventory.devices)
            {
                if (device.device_name == deviceName)
                {
                    actualDeviceObjectString = ser.Serialize(device);
                }
            }
            File.Delete(tempTestFilePath);

            //Assert
            Assert.IsFalse(error);
            Assert.IsNull(outDevice);
            Assert.AreEqual(expectedDeviceObjectString, actualDeviceObjectString);
        }

        [TestMethod]
        public void TestDistributePriorities()
        {
            //Arrange
            List<Nest> nestsBefore = new List<Nest>(5)
            {
                new Nest() { plate_barcode = "PQ1", priority = "1" },
                new Nest() { plate_barcode = "PQ2", priority = "11" },
                new Nest() { plate_barcode = "PQ3", priority = "111" },
                new Nest() { plate_barcode = "PQ4", priority = "1111" },
                new Nest() { plate_barcode = "PQ5", priority = "11111" }
            };
            List<string> expectedPriorities = new List<string>(5) { "1", "2", "3", "4", "5" };
            int i = 0;

            //Act
            List<Nest> nestsAfter = Inventory.DistributePriorities(nestsBefore);

            //Assert
            foreach (Nest actualNest in nestsAfter)
            {
                Assert.AreEqual(actualNest.priority, expectedPriorities[i]);
                i++;
            }
        }

        [TestMethod]
        public void TestInvalidFilePathProvided()
        {
            //Arrange
            string invFilePath = string.Empty;
            string deviceName = "stack1";
            int nest_num = 1;
            string barcode = "FEXXXXXXXX";
            string workflow_ID = "WFXXXXXX";
            string attributeName = "plate_barcode";
            string attributeValue = "PQYYYYY";
            Device devicetoSet = new Device()
            {
                device_name = "centrifuge1",
                device_type = "centrifuge",
                lm_number = "LMXXXXX",
                enabled = true,
                num_nests = 1
            };
            Device devicetoReference = new Device();

            //Act + Assert
            Assert.ThrowsException<ArgumentException>(() => Inventory.ReadAllText(invFilePath));
            Assert.ThrowsException<ArgumentException>(() => Inventory.GetAvailableNest(invFilePath, deviceName));
            Assert.ThrowsException<ArgumentException>(() => Inventory.AddLabwareToNest(invFilePath, deviceName, nest_num, barcode, workflow_ID));
            Assert.ThrowsException<ArgumentException>(() => Inventory.GetNextNestPosition(invFilePath, deviceName, out _));
            Assert.ThrowsException<ArgumentException>(() => Inventory.RemoveLabwareFromNest(invFilePath, deviceName, nest_num));
            Assert.ThrowsException<ArgumentException>(() => Inventory.GetNestAttribute(attributeName, invFilePath, deviceName, nest_num));
            Assert.ThrowsException<ArgumentException>(() => Inventory.SetNestAttribute(attributeName, attributeValue, invFilePath, deviceName, nest_num));
            Assert.ThrowsException<ArgumentException>(() => Inventory.GetNestNumAndDeviceFromBarcode(barcode, invFilePath));
            Assert.ThrowsException<ArgumentException>(() => Inventory.SetDeviceObject(invFilePath, devicetoSet, deviceName, devicetoReference, out _, out _));
        }

        [TestMethod]
        public void TestInvalidDeviceProvided()
        {
            //Arrange
            string invFilePath = tempTestFilePath;
            string deviceName = string.Empty;
            int nest_num = 1;
            string barcode = "FEXXXXXXXX";
            string workflow_ID = "WFXXXXXX";
            string attributeName = "plate_barcode";
            string attributeValue = "PQYYYYY";
            Device devicetoSet = new Device()
            {
                device_name = "centrifuge1",
                device_type = "centrifuge",
                lm_number = "LMXXXXX",
                enabled = true,
                num_nests = 1
            };
            Device devicetoReference = new Device();
            int expectedNestNumber = -1;

            //Act
            File.Copy(testFilePath, tempTestFilePath, true);
            int actualNestNumber = Inventory.GetAvailableNest(invFilePath, deviceName);

            //Assert
            Assert.AreEqual(expectedNestNumber, actualNestNumber);
            Assert.ThrowsException<ArgumentException>(() => Inventory.AddLabwareToNest(invFilePath, deviceName, nest_num, barcode, workflow_ID));
            Assert.ThrowsException<Exception>(() => Inventory.GetNextNestPosition(invFilePath, deviceName, out _));
            Assert.ThrowsException<ArgumentException>(() => Inventory.RemoveLabwareFromNest(invFilePath, deviceName, nest_num));
            Assert.ThrowsException<ArgumentException>(() => Inventory.GetNestAttribute(attributeName, invFilePath, deviceName, nest_num));
            Assert.ThrowsException<ArgumentException>(() => Inventory.SetNestAttribute(attributeName, attributeValue, invFilePath, deviceName, nest_num));
            Assert.ThrowsException<ArgumentException>(() => Inventory.SetDeviceObject(invFilePath, devicetoSet, deviceName, devicetoReference, out _, out _));

            //Cleanup
            File.Delete(tempTestFilePath);
        }

        [TestMethod]
        public void TestInvalidNestProvided()
        {
            //Arrange
            string invFilePath = tempTestFilePath;
            string deviceName = "stack1";
            int nest_num = 9999;
            string barcode = "FEXXXXXXXX";
            string workflow_ID = "WFXXXXXX";
            string attributeName = "plate_barcode";
            string attributeValue = "PQYYYYY";
            Device devicetoSet = new Device()
            {
                device_name = "centrifuge1",
                device_type = "centrifuge",
                lm_number = "LMXXXXX",
                enabled = true,
                num_nests = 1
            };
            Device devicetoReference = new Device();
            File.Copy(testFilePath, tempTestFilePath, true);

            //Act + Assert
            Assert.ThrowsException<ArgumentException>(() => Inventory.AddLabwareToNest(invFilePath, deviceName, nest_num, barcode, workflow_ID));
            Assert.ThrowsException<ArgumentException>(() => Inventory.RemoveLabwareFromNest(invFilePath, deviceName, nest_num));
            Assert.ThrowsException<ArgumentException>(() => Inventory.GetNestAttribute(attributeName, invFilePath, deviceName, nest_num));
            Assert.ThrowsException<ArgumentException>(() => Inventory.SetNestAttribute(attributeName, attributeValue, invFilePath, deviceName, nest_num));

            //Cleanup
            File.Delete(tempTestFilePath);
        }

        [TestMethod]
        public void TestInvalideAttributeProvided()
        {
            //Arrange
            string invFilePath = tempTestFilePath;
            string deviceName = "stack1";
            int nest_num = 1;
            string attributeName = "dummy_attribute";
            string attributeValue = "PQYYYYY";
            File.Copy(testFilePath, tempTestFilePath, true);

            //Act + Assert
            Assert.ThrowsException<NullReferenceException>(() => Inventory.GetNestAttribute(attributeName, invFilePath, deviceName, nest_num));
            Assert.ThrowsException<NullReferenceException>(() => Inventory.SetNestAttribute(attributeName, attributeValue, invFilePath, deviceName, nest_num));

            //Cleanup
            File.Delete(tempTestFilePath);
        }

        [TestMethod]
        public void TestInvalidBarcodeProvided()
        {
            //Arrange
            string invFilePath = testFilePath;
            string barcode = "Dummy_Barcode";

            //Act + Assert
            Assert.ThrowsException<Exception>(() => Inventory.GetNestNumAndDeviceFromBarcode(barcode, invFilePath));
        }

        [TestMethod]
        public void TestInvalidPrioritiesProvided()
        {
            //Arrange
            List<Nest> myNests = new List<Nest>(5)
            {
                new Nest() { plate_barcode = "PQ1" },
                new Nest() { plate_barcode = "PQ2" },
                new Nest() { plate_barcode = "PQ3" },
                new Nest() { plate_barcode = "PQ4" },
                new Nest() { plate_barcode = "PQ5" }
            };

            //Act + Assert
            Assert.ThrowsException<ArgumentException>(() => Inventory.DistributePriorities(myNests));
        }
    }
}
