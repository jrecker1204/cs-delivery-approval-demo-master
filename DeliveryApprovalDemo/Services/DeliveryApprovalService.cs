using DeliveryApprovalDemo.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DeliveryApprovalDemo.Services
{
    public class DeliveryApprovalService
    {
        private readonly string _defaultStatus;
        private readonly SeismicDbService _dbService;
        private readonly TenantConfigurationService _configurationService;
        private readonly ILogger<DeliveryApprovalService> _logger;

        public DeliveryApprovalService(IConfiguration configuration, SeismicDbService dbService, TenantConfigurationService configurationService, ILogger<DeliveryApprovalService> logger)
        {
            _defaultStatus = configuration["DefaultStatus"];
            _dbService = dbService;
            _configurationService = configurationService;
            _logger = logger;
       }

        public async Task<ApprovalResponse> ProcessRequest(ApprovalRequest approvalRequest)
        {
            // Step: Build approval response
            var approval = new DeliveryApproval { Recipients = new List<ApprovalRecipientResponse>() };
            var global = new ApprovalStatus { Status = "approve" };
            try
            {
                // Try to get config for tenant
                var tenantConfig = await _configurationService.GetConfig(approvalRequest.TenantName);

                // If no config, throw error
                if (tenantConfig == null)
                {
                    throw new ArgumentNullException($"No config found for Tenant: '{approvalRequest.Tenant}'");
                }

                var deliveryType = approvalRequest.Data.Delivery.Type.ToLower();
                var recipients = approvalRequest.Data.Delivery.Recipients;
                var contentItems = approvalRequest.Data.Delivery.Content;


                // Step: Get OptStatus for recipient emails
                var optStatus = await _dbService.GetEmailOptOuts(tenantConfig.DbConnStr, recipients.Select(x => x.EmailAddress).ToList());

                // Step: Get Forbidden Content. Create a list of forbidden and a list of approved content.
                var forbidList = await _dbService.GetForbidContent(tenantConfig.DbConnStr);
                var forbiddenContent = contentItems.Where(x => forbidList.Contains(x.Name)).ToList();
                var approvedContent = contentItems.Where(x => !forbidList.Contains(x.Name)).ToList();

                // Loop through each recipient and create approval statuses.
                foreach (var recipient in recipients)
                {
                    var recipientStatus = new ApprovalStatus();

                    // If the email is opted out, approval response = forbid. Otherwise, response = approve
                    if (optStatus.ContainsKey(recipient.EmailAddress))
                    {
                        var status = optStatus[recipient.EmailAddress].Split("|")[0];
                        var region = optStatus[recipient.EmailAddress].Split("|")[1];
                        _logger.LogInformation($"Recipient email: '{recipient.EmailAddress}' is in opt-out list. Setting status to '{status}'.");
                        recipientStatus.Status = status;

                        if (recipientStatus.Status == "warn")
                        {
                            global.Status = "warn";
                            recipientStatus.Message = "Please review this material before sending to this user.";
                        }
                        else if (recipientStatus.Status == "forbid")
                        {
                            global.Status = "warn";
                            recipientStatus.Message = $"This user is in a restricted region, '{region}'.";
                        }
                    }
                    else
                    {
                        _logger.LogWarning($"Recipient email: '{recipient.EmailAddress}' is not in opt-out list. Setting status to warn");
                        recipientStatus.Status = _defaultStatus;
                        recipientStatus.Message = $"Recipient Email not found in opt-out list. Setting status to '{_defaultStatus}'";
                    }

                    var recipientApproval = new ApprovalRecipientResponse
                    {
                        Recipient = recipient,
                        RecipientStatus = recipientStatus,
                        ContentStatus = new List<ApprovalContentStatus>()
                    };

                    // If the recipient status is forbid, then all content is forbidden. Otherwise, approvedContent gets the recipient status. Forbidden content is always forbidden.
                    //if (recipientStatus.Status == "forbid")
                    //{
                    //    if (contentItems.Count > 0)
                    //    {
                    //        var contentStatus = new ApprovalContentStatus
                    //        {
                    //            Status = recipientStatus.Status,
                    //            Message = recipientStatus.Message,
                    //            Content = contentItems
                    //        };

                    //        recipientApproval.ContentStatus.Add(contentStatus);
                    //    }
                    //}
                    //else
                    //{
                    //    if (approvedContent.Count > 0)
                    //    {
                    //        var contentStatus = new ApprovalContentStatus
                    //        {
                    //            Status = recipientStatus.Status,
                    //            Message = recipientStatus.Message,
                    //            Content = approvedContent
                    //        };

                    //        recipientApproval.ContentStatus.Add(contentStatus);
                    //    }

                    //    if (forbiddenContent.Count > 0)
                    //    {
                    //        global.Status = "warn";
                    //        var forbiddenStatus = new ApprovalContentStatus
                    //        {
                    //            Status = "forbid",
                    //            Message = "This content is forbidden from being shared.",
                    //            Content = forbiddenContent
                    //        };

                    //        recipientApproval.ContentStatus.Add(forbiddenStatus);
                    //    }
                    //}

                    approval.Recipients.Add(recipientApproval);
                    approval.Global = global;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"There was an error. Message: {ex.Message}");
                approval.Global = new ApprovalStatus { Status = "forbid", Message = ex.Message };
            }

            var toReturn = new ApprovalResponse { DeliveryApproval = approval };

            return toReturn;
        }
    }
}
