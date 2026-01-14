using System;
using log4net.Config;

namespace log4net.Rollbar
{
    /// <summary>
    /// Provides static methods to configure log4net with the <see cref="RollbarAppender\"/>.
    /// </summary>
    public class RollbarAppenderConfigurator
    {
        /// <summary>
        /// Initializes the log4net system using the <see cref="RollbarAppender\"/>
        /// </summary>
        /// <param name="accessToken">the post_server_item key</param>
        /// <param name="configureAppender">Optional action to configure additional appender properties</param>
        public static void Configure(string accessToken = null, Action<RollbarAppender> configureAppender = null)
        {
            var appender = new RollbarAppender { AccessToken = accessToken };

            configureAppender?.Invoke(appender);

            appender.ActivateOptions();

            BasicConfigurator.Configure(appender);
        }
    }
}