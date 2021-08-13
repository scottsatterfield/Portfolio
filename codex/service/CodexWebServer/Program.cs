using System;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using NLog;
using NLog.Extensions.Logging;
using NLog.Web;

namespace CodexWebServer
{
    /// <summary>
    /// Program initializes and launches the ASP.NET web service.
    /// </summary>
    public class Program
    {
        private const string AspNetCoreEnvironmentKey = "ASPNETCORE_ENVIRONMENT";
        private const string DevelopmentEnvironmentKey = "Development";
        private const string StagingEnvironmentKey = "Staging";
        private const string DefaultSettingsFilename = "appsettings.json";
        private const string SettingsFilenameStringTemplate = "appsettings.{0}.json";

        public static void Main(string[] args)
        {
            var settingsFilename = DefaultSettingsFilename;
            var environment = Environment.GetEnvironmentVariable(AspNetCoreEnvironmentKey);

            // use appsettings.Development.json when ASPNETCORE_ENVIRONMENT environment variable is set to Development 
            if (string.Equals(environment, DevelopmentEnvironmentKey, StringComparison.CurrentCultureIgnoreCase))
                settingsFilename = string.Format(SettingsFilenameStringTemplate, DevelopmentEnvironmentKey);

            // use appsettings.Staging.json when ASPNETCORE_ENVIRONMENT environment variable is set to Staging
            if (string.Equals(environment, StagingEnvironmentKey, StringComparison.CurrentCultureIgnoreCase))
                settingsFilename = string.Format(SettingsFilenameStringTemplate, StagingEnvironmentKey);

            // configure NLog
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(settingsFilename, optional: false, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();
            
            // Setup NLog
            var logger = LogManager.Setup()
                .LoadConfigurationFromSection(config)
                .GetCurrentClassLogger();

            logger.Debug($"using {settingsFilename} configuration");
            
            try
            {
                logger.Debug("initalize main");
                
                // run ASP.NET web service
                CreateHostBuilder(args)
                    .Build()
                    .Run();
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Stopped program because of exception");
            }
            finally
            {
                LogManager.Shutdown();
            }
        }

        /// <summary>
        /// Create the ASP.NET web service host.
        /// </summary>
        /// <param name="args">Program Command Line Parameters</param>
        /// <returns>Host Builder Object</returns>
        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); })
                .UseNLog();
    }
}