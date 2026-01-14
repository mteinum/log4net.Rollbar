using System;

namespace log4net.Rollbar
{
    /// <summary>
    /// Fluent configuration builder for <see cref="RollbarAppender"/>.
    /// Provides a programmatic way to configure the appender without relying on app.config.
    /// </summary>
    public class RollbarAppenderConfiguration
    {
        private readonly RollbarAppender _appender;

        /// <summary>
        /// Initializes a new instance of the <see cref="RollbarAppenderConfiguration"/> class.
        /// </summary>
        public RollbarAppenderConfiguration()
        {
            _appender = new RollbarAppender();
        }

        /// <summary>
        /// Sets the Rollbar access token for API authentication.
        /// </summary>
        /// <param name="token">The access token.</param>
        /// <returns>The configuration builder for method chaining.</returns>
        public RollbarAppenderConfiguration WithAccessToken(string token)
        {
            _appender.AccessToken = token;
            return this;
        }

        /// <summary>
        /// Sets the environment name (e.g., "production", "development").
        /// </summary>
        /// <param name="environment">The environment name.</param>
        /// <returns>The configuration builder for method chaining.</returns>
        public RollbarAppenderConfiguration WithEnvironment(string environment)
        {
            _appender.Environment = environment;
            return this;
        }

        /// <summary>
        /// Sets the Rollbar API endpoint URL.
        /// </summary>
        /// <param name="endpoint">The endpoint URL.</param>
        /// <returns>The configuration builder for method chaining.</returns>
        public RollbarAppenderConfiguration WithEndpoint(string endpoint)
        {
            _appender.Endpoint = endpoint;
            return this;
        }

        /// <summary>
        /// Sets the framework name.
        /// </summary>
        /// <param name="framework">The framework name.</param>
        /// <returns>The configuration builder for method chaining.</returns>
        public RollbarAppenderConfiguration WithFramework(string framework)
        {
            _appender.Framework = framework;
            return this;
        }

        /// <summary>
        /// Sets the Git SHA of the deployed code.
        /// </summary>
        /// <param name="gitSha">The Git SHA.</param>
        /// <returns>The configuration builder for method chaining.</returns>
        public RollbarAppenderConfiguration WithGitSha(string gitSha)
        {
            _appender.GitSha = gitSha;
            return this;
        }

        /// <summary>
        /// Sets the programming language.
        /// </summary>
        /// <param name="language">The programming language.</param>
        /// <returns>The configuration builder for method chaining.</returns>
        public RollbarAppenderConfiguration WithLanguage(string language)
        {
            _appender.Language = language;
            return this;
        }

        /// <summary>
        /// Sets the platform name.
        /// </summary>
        /// <param name="platform">The platform name.</param>
        /// <returns>The configuration builder for method chaining.</returns>
        public RollbarAppenderConfiguration WithPlatform(string platform)
        {
            _appender.Platform = platform;
            return this;
        }

        /// <summary>
        /// Sets comma-separated list of parameter names to scrub from payloads.
        /// </summary>
        /// <param name="scrubParams">Comma-separated parameter names.</param>
        /// <returns>The configuration builder for method chaining.</returns>
        public RollbarAppenderConfiguration WithScrubParams(string scrubParams)
        {
            _appender.ScrubParams = scrubParams;
            return this;
        }

        /// <summary>
        /// Sets whether to send log events to Rollbar asynchronously.
        /// </summary>
        /// <param name="async">True for asynchronous mode (default), false for synchronous.</param>
        /// <returns>The configuration builder for method chaining.</returns>
        public RollbarAppenderConfiguration AsynchronousMode(bool async = true)
        {
            _appender.Asynchronous = async;
            return this;
        }

        /// <summary>
        /// Builds and activates the configured <see cref="RollbarAppender"/>.
        /// </summary>
        /// <returns>The configured and activated RollbarAppender instance.</returns>
        /// <exception cref="InvalidOperationException">Thrown when configuration fails.</exception>
        public RollbarAppender Build()
        {
            _appender.ActivateOptions();
            return _appender;
        }
    }
}
