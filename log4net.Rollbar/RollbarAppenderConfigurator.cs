using log4net.Config;

namespace log4net.Rollbar
{
    public class RollbarAppenderConfigurator
    {
        /// <summary>
        /// Initializes the log4net system using the <see cref="RollbarAppender"/>
        /// </summary>
        /// <param name="accessToken">the post_server_item key</param>
        public static void Configure(string accessToken = null)
        {
            var appender = new RollbarAppender { AccessToken = accessToken };

            appender.ActivateOptions();

            BasicConfigurator.Configure(appender);
        }
    }
}