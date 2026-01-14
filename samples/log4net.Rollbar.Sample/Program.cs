using System;
using log4net;
using log4net.Config;

namespace log4net.Rollbar.Sample
{
    /// <summary>
    /// Sample console application demonstrating RollbarAppender usage.
    /// </summary>
    class Program
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Program));

        static void Main(string[] args)
        {
            Console.WriteLine("log4net.Rollbar Sample Application");
            Console.WriteLine("===================================\n");

            // Example 1: Configure from app.config
            Console.WriteLine("Example 1: Configuration from app.config");
            ConfigureFromAppConfig();

            // Example 2: Fluent configuration
            Console.WriteLine("\nExample 2: Fluent configuration");
            ConfigureWithFluentAPI();

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }

        static void ConfigureFromAppConfig()
        {
            try
            {
                // Configure log4net from app.config
                XmlConfigurator.Configure();

                log.Info("Application started - configured from app.config");

                // Log different severity levels
                log.Debug("This is a debug message");
                log.Info("This is an informational message");
                log.Warn("This is a warning message");

                // Log with custom properties
                log4net.ThreadContext.Properties["UserId"] = "12345";
                log4net.ThreadContext.Properties["RequestId"] = Guid.NewGuid().ToString();
                log.Info("Message with custom properties");

                // Log an exception
                try
                {
                    throw new InvalidOperationException("Sample exception for Rollbar");
                }
                catch (Exception ex)
                {
                    log.Error("An error occurred during processing", ex);
                }

                Console.WriteLine("✓ Logs sent to Rollbar via app.config configuration");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ Error: {ex.Message}");
            }
        }

        static void ConfigureWithFluentAPI()
        {
            try
            {
                // Example using fluent configuration API
                // Note: Replace with your actual Rollbar access token
                var appender = new RollbarAppenderConfiguration()
                    .WithAccessToken("your-rollbar-access-token-here")
                    .WithEnvironment("development")
                    .WithPlatform("Console Application")
                    .WithLanguage("C#")
                    .AsynchronousMode(true)
                    .Build();

                // Configure log4net with the fluent-configured appender
                BasicConfigurator.Configure(appender);

                var fluentLog = LogManager.GetLogger("FluentConfiguredLogger");
                
                fluentLog.Info("Message sent using fluent configuration");
                fluentLog.Warn("Warning from fluent-configured logger");

                // Log with exception
                try
                {
                    var result = 10 / int.Parse("0");
                }
                catch (Exception ex)
                {
                    fluentLog.Error("Mathematical operation failed", ex);
                }

                Console.WriteLine("✓ Logs sent to Rollbar via fluent API configuration");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ Error: {ex.Message}");
            }
        }
    }
}
