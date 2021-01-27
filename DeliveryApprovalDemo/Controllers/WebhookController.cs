using System;
using System.Threading.Tasks;
using DeliveryApprovalDemo.Models;
using DeliveryApprovalDemo.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace DeliveryApprovalDemo.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class WebhookController : ControllerBase
    {
        private readonly ILogger<WebhookController> _logger;
        private readonly DeliveryApprovalService _approvalService;

        public WebhookController(ILogger<WebhookController> logger, DeliveryApprovalService approvalService)
        {
            _logger = logger;
            _approvalService = approvalService;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody]dynamic approvalRequest)
        {
            try
            {
                _logger.LogInformation($"Request JSON received: {approvalRequest}");

                var approval = JsonConvert.DeserializeObject<ApprovalRequest>(approvalRequest.ToString());
                var response = await _approvalService.ProcessRequest(approval);

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"There was an error processing request. Message: {ex.Message}");

                var errorResponse = new ApprovalResponse { DeliveryApproval = new DeliveryApproval { Global = new ApprovalStatus { Status = "forbid", Message = ex.Message } } };

                // For some reason, Seismic DeliveryApproval app needs the error to be sent as a HTTP 200 OK response.
                // This service requires a 200 or 202 response. If 200, conforms to approved response payload, 202 means the provider will get back the answer within 3 seconds with proper 200 code
                return Ok(errorResponse);
            }
        }
    }
}
