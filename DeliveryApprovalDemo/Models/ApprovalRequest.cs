using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DeliveryApprovalDemo.Models
{
    public class ApprovalRequest
    {
        [Required]
        public string ExtensionPoint { get; set; }
        [Required]
        public string TenantName { get; set; }
        [Required]
        public string Tenant { get; set; }
        [Required]
        public DateTime OccurredAt { get; set; }
        [Required]
        public ApprovalData Data { get; set; }
    }

    public class ApprovalResponse
    {
        public DeliveryApproval DeliveryApproval { get; set; }
    }

    public class DeliveryApproval
    {
        public ApprovalStatus Global { get; set; }
        public List<ApprovalRecipientResponse> Recipients { get; set; }
    }

    public class ApprovalRecipientResponse
    {
        public ApprovalUser Recipient { get; set; }
        public ApprovalStatus RecipientStatus { get; set; }
        public List<ApprovalContentStatus> ContentStatus { get; set; }
    }

    public class ApprovalData
    {
        public ApprovalUser User { get; set; }
        [Required]
        public ApprovalDelivery Delivery { get; set; }
    }

    public class ApprovalUser
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailAddress { get; set; }
        public List<ApprovalContext> Context { get; set; }
    }

    public class ApprovalContext
    {
        public string SystemType { get; set; }
        public string SystemId { get; set; }
        public string ContextType { get; set; }
        public string ContextName { get; set; }
        public string ContextId { get; set; }
        public Dictionary<string, string> Fields { get; set; }
    }

    public class ApprovalEngagementInteraction
    {
        public ApprovalDelivery Delivery { get; set; }
        public ApprovalClientDetails ClientDetails { get; set; }
    }

    public class ApprovalDelivery
    {
        public string Type { get; set; }
        public ApprovalDeliveryDetails Details { get; set; }
        public MicroFormInformation FormInformation { get; set; }
        public List<ApprovalUser> Recipients { get; set; }
        public List<ApprovalContent> Content { get; set; }
        public List<ApprovalContext> Context { get; set; }
    }

    public class ApprovalDeliveryDetails
    {
        public bool ContainsAttachments { get; set; }
    }

    public class ApprovalContent
    {
        public ApprovalOriginContent OriginContent { get; set; }
        public string Id { get; set; }
        public string Repository { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Format { get; set; }
        public string ParentFolderId { get; set; }
        public string ContentProfileId { get; set; }
        public List<string> ContentProfilePath { get; set; }
        public List<ApprovalApplicationUrl> ApplicationUrls { get; set; }
        public ApprovalLibraryContent LibraryContent { get; set; }
        public string Version { get; set; }
        public string IconUrl { get; set; }
        public List<ApprovalDeliveryOption> DeliveryOptions { get; set; }
        public string TeamsiteId { get; set; }
        public string KitId { get; set; }
    }

    public class ApprovalOriginContent
    {
        public string OriginContentProfileId { get; set; }
        public string OriginLibraryContentId { get; set; }
        public string OriginLibraryContentVersionId { get; set; }
    }

    public class ApprovalApplicationUrl
    {
        public string Name { get; set; }
        public string Url { get; set; }
    }

    public class ApprovalDeliveryOption
    {
        public string Id { get; set; }
    }

    public class ApprovalLibraryContent
    {
        public string Id { get; set; }
        public string VersionId { get; set; }
        public string TeamSiteId { get; set; }
    }

    public class ApprovalClientDetails
    {
        public string SessionId { get; set; }
        public string IpAddress { get; set; }
        public string ProductArea { get; set; }
        public string DeviceType { get; set; }
        public string Browser { get; set; }
        public string BrowserVersion { get; set; }
        public string OperatingSystem { get; set; }
        public string OperatingSystemVersion { get; set; }
        public string Locale { get; set; }
    }

    public class ApprovalStatus
    {
        public string Status { get; set; }
        public string Message { get; set; }
    }

    public class ApprovalContentStatus
    {
        public string Status { get; set; }
        public string Message { get; set; }
        public List<ApprovalContent> Content { get; set; }
    }

    public class MicroFormInformation
    {
        public List<ApprovalFormInput> FormInputs { get; set; }
    }

    public class ApprovalFormInput
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public bool IsTable { get; set; }
        public List<List<ApprovalFormInput>> ChildVariableData { get; set; }
        public List<ApprovalExtensionInformation> ExtensionInformation { get; set; }
    }

    public class ApprovalExtensionInformation
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }
}
