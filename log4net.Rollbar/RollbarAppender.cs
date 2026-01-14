using System;
using System.Collections.Generic;
using System.Configuration;
using log4net.Appender;
using log4net.Core;
using RollbarSharp;
using RollbarSharp.Serialization;
using Configuration = RollbarSharp.Configuration;
using System.Threading.Tasks;

namespace log4net.Rollbar
{
    /// <summary>
    /// Appender that sends log4net events to Rollbar for error tracking and monitoring.
    /// </summary>
    public class RollbarAppender : AppenderSkeleton
    {
        private Configuration _configuration;

        /// <summary>
        /// Gets or sets the Rollbar access token for API authentication.
        /// Can also be configured via AppSettings key "Rollbar.AccessToken".
        /// </summary>
        public string AccessToken { get; set; }
        
        /// <summary>
        /// Gets or sets the environment name (e.g., "production", "development").
        /// Can also be configured via AppSettings key "Rollbar.Environment".
        /// </summary>
        public string Environment { get; set; }
        
        /// <summary>
        /// Gets or sets the Rollbar API endpoint URL.
        /// Can also be configured via AppSettings key "Rollbar.Endpoint".
        /// </summary>
        public string Endpoint { get; set; }
        
        /// <summary>
        /// Gets or sets the framework name.
        /// Can also be configured via AppSettings key "Rolllbar.Framework".
        /// </summary>
        public string Framework { get; set; }
        
        /// <summary>
        /// Gets or sets the Git SHA of the deployed code.
        /// Can also be configured via AppSettings key "Rollbar.GitSha".
        /// </summary>
        public string GitSha { get; set; }
        
        /// <summary>
        /// Gets or sets the programming language.
        /// Can also be configured via AppSettings key "Rollbar.CodeLanguage".
        /// </summary>
        public string Language { get; set; }
        
        /// <summary>
        /// Gets or sets the platform name.
        /// Can also be configured via AppSettings key "Rollbar.Platform".
        /// </summary>
        public string Platform { get; set; }
        
        /// <summary>
        /// Gets or sets comma-separated list of parameter names to scrub from payloads.
        /// Can also be configured via AppSettings key "Rollbar.ScrubParams".
        /// </summary>
        public string ScrubParams { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to send log events to Rollbar asynchronously.
        /// Default is true.
        /// </summary>
        public bool Asynchronous { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="RollbarAppender"/> class.
        /// </summary>
        public RollbarAppender()
        {
            Asynchronous = true;
        }

        /// <summary>
        /// Activates the appender options and initializes the Rollbar configuration.
        /// </summary>
        public override void ActivateOptions()
        {
            var accessToken = GetConfigSetting(AccessToken, "Rollbar.AccessToken");
            
            if (string.IsNullOrWhiteSpace(accessToken))
            {
                ErrorHandler.Error("Rollbar.AccessToken is required but not configured. Please set the AccessToken property or add Rollbar.AccessToken to AppSettings.");
                return;
            }
            
            try
            {
                _configuration = new Configuration(accessToken);

                _configuration.Endpoint = GetConfigSetting(Endpoint, "Rollbar.Endpoint", _configuration.Endpoint);
                _configuration.Environment = GetConfigSetting(Environment, "Rollbar.Environment", _configuration.Environment);
                _configuration.Framework = GetConfigSetting(Framework, "Rolllbar.Framework", _configuration.Framework);
                _configuration.GitSha = GetConfigSetting(GitSha, "Rollbar.GitSha");
                _configuration.Language = GetConfigSetting(Language, "Rollbar.CodeLanguage", _configuration.Language);
                _configuration.Platform = GetConfigSetting(Platform, "Rollbar.Platform", _configuration.Platform);

                var scrubParams = GetConfigSetting(ScrubParams, "Rollbar.ScrubParams");
                _configuration.ScrubParams = scrubParams == null ?
                    Configuration.DefaultScrubParams : scrubParams.Split(',');
            }
            catch (Exception ex)
            {
                ErrorHandler.Error("Failed to initialize Rollbar configuration", ex);
            }
        }

        /// <summary>
        /// Gets a configuration setting from the parameter, AppSettings, or fallback value.
        /// </summary>
        /// <param name="param">The parameter value.</param>
        /// <param name="name">The AppSettings key name.</param>
        /// <param name="fallback">The fallback value if neither parameter nor AppSettings contains a value.</param>
        /// <returns>The configuration setting value.</returns>
        private static string GetConfigSetting(string param, string name, string fallback = null)
        {
            return param ?? ConfigurationManager.AppSettings[name] ?? fallback;
        }

        /// <summary>
        /// Sends the given event to Rollbar with appropriate severity level.
        /// </summary>
        /// <param name="loggingEvent">The event to report.</param>
        protected override void Append(LoggingEvent loggingEvent)
        {
            if (_configuration == null)
            {
                ErrorHandler.Error("Rollbar configuration is not initialized. ActivateOptions may have failed.");
                return;
            }
            
            try
            {
                var client = new RollbarClient(_configuration);
                var customData = ExtractCustomData(loggingEvent);

                Task task = null;
                if (loggingEvent.Level >= Level.Critical)
                {
                    task = Send(loggingEvent, customData, client.SendCriticalMessage, client.SendCriticalException);
                }
                else if (loggingEvent.Level >= Level.Error)
                {
                    task = Send(loggingEvent, customData, client.SendErrorMessage, client.SendErrorException);
                }
                else if (loggingEvent.Level >= Level.Warn)
                {
                    task = Send(loggingEvent, customData, client.SendWarningMessage, client.SendWarningException);
                }
                else if (loggingEvent.Level >= Level.Info)
                {
                    task = client.SendInfoMessage(loggingEvent.RenderedMessage, customData);
                }
                else if (loggingEvent.Level >= Level.Debug)
                {
                    task = client.SendDebugMessage(loggingEvent.RenderedMessage, customData);
                }

                if (task != null && !Asynchronous)
                {
                    task.Wait(TimeSpan.FromSeconds(5));
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.Error($"Failed to send event to Rollbar: {loggingEvent.RenderedMessage}", ex);
            }
        }

        /// <summary>
        /// Extracts custom data from the logging event properties.
        /// </summary>
        /// <param name="loggingEvent">The logging event.</param>
        /// <returns>A dictionary of custom data, or null if no properties exist.</returns>
        private static IDictionary<string, object> ExtractCustomData(LoggingEvent loggingEvent)
        {
            if (loggingEvent.Properties == null || loggingEvent.Properties.Count == 0)
            {
                return null;
            }
            
            var data = new Dictionary<string, object>();
            
            foreach (var key in loggingEvent.Properties.GetKeys())
            {
                var value = loggingEvent.Properties[key];
                if (value != null)
                {
                    data[key] = value;
                }
            }
            
            return data.Count > 0 ? data : null;
        }

        /// <summary>
        /// Helper method for reporting a given event. Keeps the code DRY.
        /// </summary>
        /// <param name="loggingEvent">The logging event to send.</param>
        /// <param name="customData">Custom data extracted from the event properties.</param>
        /// <param name="sendMessage">Function to send a message.</param>
        /// <param name="sendException">Function to send an exception.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        private static Task Send(
            LoggingEvent loggingEvent,
            IDictionary<string, object> customData,
            Func<string, IDictionary<string, object>, Action<DataModel>, string, Task> sendMessage,
            Func<Exception, string, Action<DataModel>, string, Task> sendException)
        {
            if (loggingEvent.ExceptionObject == null)
            {
                return sendMessage(loggingEvent.RenderedMessage, customData, null, null);
            }
            else
            {
                return sendException(loggingEvent.ExceptionObject, null, null, null);
            }
        }
    }
}
