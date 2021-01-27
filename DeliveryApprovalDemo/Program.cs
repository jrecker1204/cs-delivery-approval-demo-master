using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;

namespace DeliveryApprovalDemo
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Local Development logs to Console. Otherwise log to LogEntries
            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
            {
                Log.Logger = new LoggerConfiguration()
                    .Enrich.FromLogContext()
                    .MinimumLevel.Information()
                    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                    .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
                    .MinimumLevel.Override("System.Net.Http.HttpClient", LogEventLevel.Warning)
                    .WriteTo.Console() //new RenderedCompactJsonFormatter())
                    .CreateLogger();
            }
            else
            {
                Log.Logger = new LoggerConfiguration()
                    .Enrich.FromLogContext()
                    .MinimumLevel.Information()
                    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                    .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
                    .MinimumLevel.Override("System.Net.Http.HttpClient", LogEventLevel.Warning)
                    .WriteTo.Logentries(Environment.GetEnvironmentVariable("LogEntriesToken"))
                    .CreateLogger();
            }

            try
            {
                Log.Information("Starting up");
                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Application start-up failed");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
