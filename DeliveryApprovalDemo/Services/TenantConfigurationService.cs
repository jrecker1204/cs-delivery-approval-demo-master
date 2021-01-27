using DeliveryApprovalDemo.Models;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace DeliveryApprovalDemo.Services
{
    public class TenantConfigurationService
    {
        private readonly CloudTable _table;
        private readonly ILogger<TenantConfigurationService> _logger;
        private readonly string _environment;

        public TenantConfigurationService(IConfiguration config, ILogger<TenantConfigurationService> logger)
        {
            _logger = logger;
            _environment = config["Environment"];

            var account = CloudStorageAccount.Parse(config["AzureStorageConnStr"]);
            var tableClient = account.CreateCloudTableClient();
            _table = tableClient.GetTableReference(config["AzureStorageTableName"]);
        }

        public async Task<TenantConfiguration> GetConfig(string tenant)
        {
            TenantConfiguration tenantConfig = null;

            var to = TableOperation.Retrieve<ServiceConfig>(_environment, tenant);
            var response = await _table.ExecuteAsync(to);

            if (response.Result != null)
            {
                var entity = response.Result as ServiceConfig;
                tenantConfig = JsonConvert.DeserializeObject<TenantConfiguration>(entity.Value);
            }

            return tenantConfig;
        }
    }

    public class ServiceConfig : TableEntity
    {
        public ServiceConfig() { }

        public ServiceConfig(string env, string tenant)
        {
            PartitionKey = env;
            RowKey = tenant;
        }

        public string Value { get; set; }
    }
}
