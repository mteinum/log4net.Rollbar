using System;
using System.Collections.Generic;
using System.Configuration;
using log4net.Appender;
using log4net.Core;
using RollbarSharp;
using RollbarSharp.Serialization;
using Configuration = RollbarSharp.Configuration;

namespace log4net.Rollbar
{
    public class RollbarAppender : AppenderSkeleton
    {
        private Configuration _configuration;

        public string AccessToken { get; set; }

        public override void ActivateOptions()
        {
            _configuration = new Configuration(AccessToken ?? GetConfigSetting("Rollbar.AccessToken"));
        }

        private static string GetConfigSetting(string name)
        {
            return ConfigurationManager.AppSettings[name];
        }

        /// <summary>
        /// Sends the given event to Rollbar
        /// </summary>
        /// <param name="loggingEvent">The event to report</param>
        protected override void Append(LoggingEvent loggingEvent)
        {
            var client = new RollbarClient(_configuration);

            if (loggingEvent.Level >= Level.Critical)
            {
                Send(loggingEvent, client.SendCriticalMessage, client.SendCriticalException);
            }
            else if (loggingEvent.Level >= Level.Error)
            {
                Send(loggingEvent, client.SendErrorMessage, client.SendErrorException);
            }
            else if (loggingEvent.Level >= Level.Warn)
            {
                Send(loggingEvent, client.SendWarningMessage, client.SendWarningException);
            }
            else if (loggingEvent.Level >= Level.Info)
            {
                client.SendInfoMessage(loggingEvent.RenderedMessage);
            }
            else if (loggingEvent.Level >= Level.Debug)
            {
                client.SendDebugMessage(loggingEvent.RenderedMessage);
            }
        }

        /// <summary>
        /// Helper method for reporting a given event. Keeps the code DRY
        /// </summary>
        /// <param name="loggingEvent"></param>
        /// <param name="sendMessage"></param>
        /// <param name="sendException"></param>
        private void Send(
            LoggingEvent loggingEvent,
            Action<string, IDictionary<string, object>, Action<DataModel>> sendMessage,
            Action<Exception, string, Action<DataModel>> sendException)
        {
            if (loggingEvent.ExceptionObject == null)
            {
                sendMessage(loggingEvent.RenderedMessage, null, null);
            }
            else
            {
                sendException(loggingEvent.ExceptionObject, null, null);
            }
        }
    }
}
