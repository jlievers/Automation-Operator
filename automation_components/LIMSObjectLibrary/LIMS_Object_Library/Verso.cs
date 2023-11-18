using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIMSObjectLibrary
{

    /// <summary>
    /// ////////////////////THIS IS ALL IN THE VERSO_API LIB!!!!!!!!!!!
    /// </summary>
    class Verso
    {
    }

    public class PicklistCreationData
    {
        public string PicklistName { get; set; }
        public string ParentLabwareDefinitionId { get; set; }
        public List<UserRoleAccessRights> UserRoleRights { get; set; }
    }

    public class UserRoleAccessRights
    {
        public string UserRoleId { get; set; }
        public string AccessRight { get; set; }
        public List<string> SpecificAccessRights { get; set; }
    }

    //response
    public class ItemCreatedResponse
    {
        public string ItemId { get; set; }
        public string ItemUri { get; set; }
        public string ItemType { get; set; }
    }

    ///// Add Containers to picklist
    //message
    public class PicklistContainerIdentifiers
    {
        public List<ContainerBarcodeIdentifier> Items { get; set; }
    }

    public class ContainerBarcodeIdentifier
    {
        public string BarcodeTyp { get; set; }
        public string BarcodeValue { get; set; }
        public string LabwareDefinitionId { get; set; }
    }

    public class ContainerCustomIdentifier
    {
        public string IdentifierName { get; set; }
        public string IdentifierValue { get; set; }
    }

    public class ContainerIdentifier
    {
        public string IdentifierValue { get; set; }
    }

    //response
    public class AddContainersResponse
    {
        public List<RejectedIdentifiers> RejectedIdentifiers { get; set; }
    }

    public class RejectedIdentifiers
    {
        public ContainerBarcodeIdentifier Identifier { get; set; }
        public string Reason { get; set; }
    }

    //Create and Run PickJob, ReturnJob, DefragJob
    //message 
    public class JobCreationData
    {
        public string JobName { get; set; }
        public int JobPriority { get; set; }
        public string JobType { get; set; }
        public List<PropertyItem> JobProperties { get; set; }
        //public List<JobUserAccessRights> UserRights { get; set; }
    }

    public class PropertyItem
    {
        public string PropertyItemKey { get; set; }
        public string PropertyItemValue { get; set; }
    }

    public class JobUserAccessRights
    {
        public string UserId { get; set; }
        public List<string> Rights { get; set; }
    }
    //response = ItemCreatedResponse

    //Query Jobs


    public class JobsResponse
    {
        public List<JobData> Items { get; set; }
        public int SkippedItems { get; set; }
        public int TakenItems { get; set; }
        public int CountedItems { get; set; }
    }


    public class JobData
    {
        public string JobId { get; set; }
        public string JobName { get; set; }
        public int JobPriority { get; set; }
        public string JobType { get; set; }
        public string JobState { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedAt { get; set; }
        public string StartDate { get; set; } //optional
        public string EndDate { get; set; }  //optional
    }

    public class JobDetailData
    {
        public string JobId { get; set; }
        public int JobPriority { get; set; }
        public string JobName { get; set; }
        public string JobType { get; set; }
        public string JobState { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedAt { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public List<PropertyItem> JobProperties { get; set; }
        public List<JobValidationResult> JobValidationResults { get; set; }
        public List<string> PredecessorJobIds { get; set; }
        public List<string> SuccessorJobIds { get; set; }
        public List<JobProcessLog> ProcessLogs { get; set; }
        public List<JobUserAccessRights> UserRights { get; set; }
    }

    public class JobValidationResult
    {
        public string Id { get; set; }
        public string RuleId { get; set; }
        public string ActionType { get; set; }
        public object Pass { get; set; }//bool optional!
        public string RuleDescription { get; set; }
        public string Description { get; set; }
    }

    public class JobProcessLog
    {
        public string End { get; set; }
        public string FinishReason { get; set; }
        public string ID { get; set; }
        public string Start { get; set; }
        public ErrorData ErrorItem { get; set; }
    }

    public class JobUpdateData
    {
        public int JobPriority { get; set; }//int optional
        public List<JobUserAccessRights> UserRights { get; set; }
        public bool ResumeJob { get; set; }
        public object RetryJobValidation { get; set; }//bool optional
    }

    public class JobResultsResponse
    {
        public List<JobResultData> Items { get; set; }
        public int SkippedItems { get; set; }
        public int TakenItems { get; set; }
        public int CountedItems { get; set; }
    }

    public class JobResultData
    {
        public string ResultId { get; set; }
        public string ParentResultId { get; set; }
        public string PredecessorJobId { get; set; }
        public string ResultClassifiers { get; set; }
        public bool IsAbnormal { get; set; }
        public string CreatedAt { get; set; }
        public string Message { get; set; }
        public string ContainerId { get; set; }
        public List<ContainerBarcodeIdentifier> Identifiers { get; set; }
        public object PositionIndex { get; set; }//int optional
        public string PositionLabel { get; set; }
        public string LocationDescription { get; set; }
        public object LocationSequence { get; set; }//int optional
        public object LocationId { get; set; }//int optional
        public object LabwareInstanceId { get; set; }//int optional
        public string LabwareDefinitionId { get; set; }
        public string LabwareType { get; set; }
        public List<JobResultData> Children { get; set; }

    }

    public class ErrorData
    {
        public string ErrorId { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public string State { get; set; }
        public string Severity { get; set; }
        public object ErrorCode { get; set; } //int optional!
        public string RecoveryTimestamp { get; set; }
        public string RecoveryTimeout { get; set; }
        public string EscalatingTimeout { get; set; }
        public string RecoveryUser { get; set; }
        public string PerformedRecovery { get; set; }
    }

    public class LibrariesResponse
    {
        public List<LibraryData> Items { get; set; }
        public int SkippedItems { get; set; }
        public int TakenItems { get; set; }
        public int CountedItems { get; set; }
    }

    public class LibraryData
    {
        public string LibraryId { get; set; }
        public string LibraryName { get; set; }
    }

    //Create Library
    //message
    public class LibraryCreationData
    {
        public string LibraryName { get; set; }
        public List<UserRoleAccessRights> UserRoleRights { get; set; }
    }
    //response = ItemCreatedResponse

    //Add Tubes To Library
    public class LibraryContainerIdentifiers
    {
        public List<ContainerBarcodeIdentifier> Items { get; set; }
    }
    //response = Add Containers Response

    //Get MaterialLocation IDs of Tubes in a Library
    //Containers query

    public class ContainerSearch // all inputs are optional but since this ia a post null values are not returned
    {
        public bool IsReserved { get; set; }
        public bool IsProblematic { get; set; }
        public bool IsLibraryAssigned { get; set; }
        public bool LoadParentTree { get; set; }
        public string PopulationStatus { get; set; }
        public List<string> LabwareTypes { get; set; }
        public List<string> LabwareDefinitions { get; set; }
        public List<string> LabwareStates { get; set; }
        public List<string> ContainerLocations { get; set; }
        public List<string> RegistrationStates { get; set; }
        public List<string> LibraryIDs { get; set; }
        public List<ContainerIdentifier> ContainerIdentifiers { get; set; }
    }

    //response
    public class ContainersResponse
    {
        public List<ContainerData> Items { get; set; }
        public int SkippedItems { get; set; }
        public int TakenItems { get; set; }
        public int CountedItems { get; set; }
    }

    //public class ContainersSearchResponse
    //{
    //    public List<ContainerData> Items { get; set; }
    //    public int SkippedItems { get; set; }
    //    public int TakenItems { get; set; }
    //    public int CountedItems { get; set; }
    //    public List<RejectedIdentifiers> RejectedIdentifiers { get; set; }
    //}

    public class ContainerData
    {
        public string ContainerId { get; set; }
        public string ParentId { get; set; }
        public ContainerData Parent { get; set; }
        public string LabwareDefinitionId { get; set; }
        public string LabwareType { get; set; }
        public string LabwareStatus { get; set; }
        public string LabwareId { get; set; }
        public bool Inactive { get; set; }
        public string RegistrationState { get; set; }
        public string ContainerLocation { get; set; }
        public List<ContainerBarcodeIdentifier> Identifiers { get; set; }
        public object PositionIndex { get; set; }//Int optional!
        public string PositionLabel { get; set; }
        public string PositionDescription { get; set; }
        public object IntegrityFault { get; set; }//Int optional!
        public string LocationDescription { get; set; }
        public object LocationSequence { get; set; }//int optional!
        public object LocationID { get; set; }//int optional!
        public List<PropertyItem> Attributes { get; set; }
    }

    //Containers From RackID response
    public class ContainerDetailData
    {
        public string ContainerId { get; set; }
        public string ParentId { get; set; }
        public ContainerDetailData Parent { get; set; }
        public List<ContainerDetailData> Children { get; set; }
        public string RegistrationState { get; set; }
        public string ContainerLocation { get; set; }
        public List<ContainerBarcodeIdentifier> Identifiers { get; set; }
        public List<PropertyItem> Attributes { get; set; }
        public object IntegrityFault { get; set; }//int optional
        public object LabwareId { get; set; }//int optional
        public string LabwareStatus { get; set; }
        public bool Inactive { get; set; }
        public string LabwareName { get; set; }
        public string LabwareType { get; set; }
        public string LabwareDefinitionId { get; set; }
        public string Orientation { get; set; }
        public object PositionIndex { get; set; }//int optional
        public string PositionLabel { get; set; }
        public string PositionDescription { get; set; }
        public string LocationDescription { get; set; }
        public object LocationSequence { get; set; }//int optional
        public object LocationTargetTemperature { get; set; } //int optional
        public object LocationID { get; set; } //int optional
    }


}