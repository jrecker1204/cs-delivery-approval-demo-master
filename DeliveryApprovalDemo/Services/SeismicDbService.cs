using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Threading.Tasks;

namespace DeliveryApprovalDemo.Services
{
    public class SeismicDbService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<SeismicDbService> _logger;

        public SeismicDbService(IConfiguration config, ILogger<SeismicDbService> logger)
        {
            _configuration = config;
            _logger = logger;
        }

        public async Task<Dictionary<string, string>> GetEmailOptOuts(string connStr, List<string> emails)
        {
            var toReturn = new Dictionary<string, string>();

            try
            {
                var sw = new Stopwatch();
                sw.Start();

                using (var sqlConn = new SqlConnection(connStr))
                {
                    var query = $"SELECT Email, OptStatus, Region FROM [S_{GetUserId(connStr)}].[EmailOptions] where Email IN ('{string.Join("','", emails)}')";
                    using (var cmd = new SqlCommand(query, sqlConn))
                    {
                        sqlConn.Open();

                        var reader = await cmd.ExecuteReaderAsync();

                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                toReturn.Add(reader.SafeGetString(0), $"{reader.SafeGetString(1).ToLower()}|{reader.SafeGetString(2)}");
                            }
                        }
                    }

                }

                sw.Stop();

                _logger.LogInformation($"Database calls took {sw.Elapsed}s");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"There was a problem querying the DB for EmailOptOutList.");

                throw new Exception("There was a problem communicating to the DB");
            }

            return toReturn;
        }

        public async Task<List<string>> GetForbidContent(string connStr)
        {
            var toReturn = new List<string>();

            try
            {
                var sw = new Stopwatch();
                sw.Start();

                using (var sqlConn = new SqlConnection(connStr))
                {
                    var query = $"SELECT ContentName FROM [S_{GetUserId(connStr)}].[ForbidContent]";
                    using (var cmd = new SqlCommand(query, sqlConn))
                    {
                        sqlConn.Open();

                        var reader = await cmd.ExecuteReaderAsync();

                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                toReturn.Add(reader.SafeGetString(0).ToLower());
                            }
                        }
                    }

                }

                sw.Stop();

                _logger.LogInformation($"Database calls took {sw.Elapsed}s");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"There was a problem querying the DB for EmailOptOutList.");

                throw new Exception("There was a problem communicating to the DB");
            }

            return toReturn;
        }

        private string GetUserId(string connStr)
        {
            var parts = connStr.Split(';');
            foreach (var part in parts)
            {
                if (part.StartsWith("User"))
                {
                    var userId = part.Split('=')[1];
                    return userId;
                }
            }

            return null;
        }
    }

    public static class SqlDataReaderExtension
    {
        public static string SafeGetString(this SqlDataReader reader, int colIndex)
        {
            if (!reader.IsDBNull(colIndex))
            {
                return reader.GetString(colIndex);
            }
            return string.Empty;
        }
    }
}
