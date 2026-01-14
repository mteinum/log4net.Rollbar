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
    public class RollbarAppender : AppenderSkeleton
    {
        private Configuration _configuration;

        public string AccessToken { get; set; }
        public string Environment { get; set; }
        public string Endpoint { get; set; }
        public string Framework { get; set; }
        public string GitSha { get; set; }
        public string Language { get; set; }
        public string Platform { get; set; }
        public string ScrubParams { get; set; }

        /// <summary>
        /// Send log events to Rollbar asynchronously
        /// </summary>
        public bool Asynchronous { get; set; }

        public RollbarAppender()
        {
            Asynchronous = true;
        }

        public override void ActivateOptions()
        {
            _configuration = new Configuration(GetConfigSetting(AccessToken, "Rollbar.AccessToken"));

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

        private static string GetConfigSetting(string param, string name, string fallback = null)
        {
            return param ?? ConfigurationManager.AppSettings[name] ?? fallback;
        }

        /// <summary>
        /// Sends the given event to Rollbar
        /// </summary>
        /// <param name="loggingEvent">The event to report</param>
        protected override void Append(LoggingEvent loggingEvent)
        {
            var client = new RollbarClient(_configuration);

            Task task = null;
            if (loggingEvent.Level >= Level.Critical)
            {
                task = Send(loggingEvent, client.SendCriticalMessage, client.SendCriticalException);
            }
            else if (loggingEvent.Level >= Level.Error)
            {
                task = Send(loggingEvent, client.SendErrorMessage, client.SendErrorException);
            }
            else if (loggingEvent.Level >= Level.Warn)
            {
                task = Send(loggingEvent, client.SendWarningMessage, client.SendWarningException);
            }
            else if (loggingEvent.Level >= Level.Info)
            {
                task = client.SendInfoMessage(loggingEvent.RenderedMessage);
            }
            else if (loggingEvent.Level >= Level.Debug)
            {
                task = client.SendDebugMessage(loggingEvent.RenderedMessage);
            }

            if (task != null && !Asynchronous)
            {
                task.Wait(TimeSpan.FromSeconds(5));
            }
        }

        /// <summary>
        /// Helper method for reporting a given event. Keeps the code DRY
        /// </summary>
        /// <param name="loggingEvent"></param>
        /// <param name="sendMessage"></param>
        /// <param name="sendException"></param>
        private static Task Send(
            LoggingEvent loggingEvent,
            Func<string, IDictionary<string, object>, Action<DataModel>, string, Task> sendMessage,
            Func<Exception, string, Action<DataModel>, string, Task> sendException)
        {
            if (loggingEvent.ExceptionObject == null)
            {
                return sendMessage(loggingEvent.RenderedMessage, null, null, null);
            }
            else
            {
                return sendException(loggingEvent.ExceptionObject, null, null, null);
            }
        }
    }
}
