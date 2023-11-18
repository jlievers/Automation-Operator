using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutomationDataLib.AutoDataTableAdapters;

namespace AutomationDataLib
{
    public class Functions
    {
        WorkflowTableAdapter Workflow = new WorkflowTableAdapter();
        Workflow_TypeTableAdapter Workflow_Type = new Workflow_TypeTableAdapter();
        TransitionTableAdapter Transition = new TransitionTableAdapter();
        DeviceTableAdapter Device = new DeviceTableAdapter();
        SystemTableAdapter IntSystem = new SystemTableAdapter();
        Device_TypeTableAdapter deviceType = new Device_TypeTableAdapter();
        Transition_TypeTableAdapter Transition_Type = new Transition_TypeTableAdapter();
        PlateTableAdapter Plate = new PlateTableAdapter();
        Plate_TypeTableAdapter Plate_Type = new Plate_TypeTableAdapter();
        Workflow_AttributeTableAdapter Workflow_Attribute = new Workflow_AttributeTableAdapter();
        Workflow_Attribute_TypeTableAdapter Workflow_Attribute_Type = new Workflow_Attribute_TypeTableAdapter();
        NestTableAdapter Nest = new NestTableAdapter();

        public int GetAvailableDeviceForSystem(string deviceTypeModel, string systemName)
        {
            int deviceTypeId = (int)deviceType.GetIdFromModel(deviceTypeModel);
            int systemId = (int)IntSystem.GetIdFromName(systemName);
            AutoData.DeviceDataTable myTable = new AutoData.DeviceDataTable();
            Device.FillBy_AvailableDeviceType(myTable, systemId, deviceTypeId);
            return 1; /////NOT DONE
        }

        public void AddNewWorkflowToSystem(string systemName, string workflowName, string lIMSWorkflowId)
        {
            int systemId = (int)IntSystem.GetIdFromName(systemName);
            int WFTypeId = (int)Workflow_Type.GetIdFromName(workflowName);
            int WFId = (int)Workflow.InsertQuery(lIMSWorkflowId, DateTime.Now, null, true, WFTypeId, systemId);

            AutoData.Transition_TypeDataTable myTable = new AutoData.Transition_TypeDataTable();
            Transition_Type.FillBy_WorkflowTypeId(myTable, WFTypeId);

            foreach (AutoData.Transition_TypeRow oneRow in myTable)
            {
                Transition.Insert(true, null, null, oneRow.Id, WFId);
            }
        }

        public void AddWorkflowType(string newWorkflowName, string newWorkflowDescription, string sauronWorkorderName)
        {
            Workflow_Type.Insert(newWorkflowName, newWorkflowDescription, sauronWorkorderName);
            ///Not Done Add Transition Types!!
        }

        public void AddNewDevice(string newDeviceModel, string systemName, string newDeviceName, string newDeviceLM)
        {
            int DeviceTypeId = (int)deviceType.GetIdFromModel(newDeviceModel);
            int systemId = (int)IntSystem.GetIdFromName(systemName);
            int deviceId = (int)Device.InsertQuery(newDeviceName, DeviceTypeId, true, newDeviceLM, true, systemId);
            int numNests = (int)deviceType.GetNumNests(DeviceTypeId);
            for (int i = 0; i < numNests; i++)
            {
                Nest.Insert(i, null, true, deviceId, null);
            }
        }

        private AutoData.WorkflowDataTable GetWorkflowsForSystem(int systemId)
        {
            AutoData.WorkflowDataTable myTable = new AutoData.WorkflowDataTable();
            Workflow.GetActiveWorkflowsForSystem(myTable, systemId);
            return myTable;
        }

        private void GetTransitionsForWorkflow(int workflowId)
        {
            AutoData.WorkflowDataTable myTable = new AutoData.WorkflowDataTable();
            // Transition.(myTable, workflowId);
        }

        private void AddPlateToWorkflow(string invitaePlateType, string plateBarcode, int workflowId)
        {
            int plate_TypeId = (int)Plate_Type.GetIdFromInvitaeType(invitaePlateType);
            Plate.Insert(plateBarcode, workflowId, plate_TypeId);
        }

        private void AddAttributeToWorkflow(string AttributeName, string attributeValue, int workflowId)
        {
            int AttributeTypeId = (int)Workflow_Attribute_Type.GetIdFromName(AttributeName);
            Workflow_Attribute.Insert(attributeValue, AttributeTypeId, workflowId);
        }
    }

}
