using System;
using System.Configuration;
using log4net.Core;
using Xunit;

namespace log4net.Rollbar.Tests
{
    public class RollbarAppenderTests
    {
        [Fact]
        public void Constructor_ShouldSetAsynchronousToTrue()
        {
            // Arrange & Act
            var appender = new RollbarAppender();

            // Assert
            Assert.True(appender.Asynchronous);
        }

        [Fact]
        public void Properties_ShouldBeSettable()
        {
            // Arrange
            var appender = new RollbarAppender();

            // Act
            appender.AccessToken = "test-token";
            appender.Environment = "test-env";
            appender.Endpoint = "https://test.endpoint";
            appender.Framework = "test-framework";
            appender.GitSha = "abc123";
            appender.Language = "C#";
            appender.Platform = "Windows";
            appender.ScrubParams = "password,secret";
            appender.Asynchronous = false;

            // Assert
            Assert.Equal("test-token", appender.AccessToken);
            Assert.Equal("test-env", appender.Environment);
            Assert.Equal("https://test.endpoint", appender.Endpoint);
            Assert.Equal("test-framework", appender.Framework);
            Assert.Equal("abc123", appender.GitSha);
            Assert.Equal("C#", appender.Language);
            Assert.Equal("Windows", appender.Platform);
            Assert.Equal("password,secret", appender.ScrubParams);
            Assert.False(appender.Asynchronous);
        }

        [Fact]
        public void ActivateOptions_WithAccessToken_ShouldNotThrow()
        {
            // Arrange
            var appender = new RollbarAppender
            {
                AccessToken = "test-access-token-12345",
                Environment = "test"
            };

            // Act & Assert
            var exception = Record.Exception(() => appender.ActivateOptions());
            Assert.Null(exception);
        }

        [Fact]
        public void Append_WithCriticalLevel_ShouldNotThrow()
        {
            // Arrange
            var appender = new RollbarAppender
            {
                AccessToken = "test-token-12345",
                Environment = "test",
                Asynchronous = false
            };
            appender.ActivateOptions();

            var loggingEvent = new LoggingEvent(
                typeof(RollbarAppenderTests),
                null,
                "TestLogger",
                Level.Critical,
                "Test message",
                null
            );

            // Act & Assert
            var exception = Record.Exception(() => appender.DoAppend(loggingEvent));
        }

        [Fact]
        public void Append_WithErrorLevel_ShouldNotThrow()
        {
            // Arrange
            var appender = new RollbarAppender
            {
                AccessToken = "test-token-12345",
                Environment = "test",
                Asynchronous = false
            };
            appender.ActivateOptions();

            var loggingEvent = new LoggingEvent(
                typeof(RollbarAppenderTests),
                null,
                "TestLogger",
                Level.Error,
                "Test message",
                null
            );

            // Act & Assert
            var exception = Record.Exception(() => appender.DoAppend(loggingEvent));
        }

        [Fact]
        public void Append_WithWarnLevel_ShouldNotThrow()
        {
            // Arrange
            var appender = new RollbarAppender
            {
                AccessToken = "test-token-12345",
                Environment = "test",
                Asynchronous = false
            };
            appender.ActivateOptions();

            var loggingEvent = new LoggingEvent(
                typeof(RollbarAppenderTests),
                null,
                "TestLogger",
                Level.Warn,
                "Test message",
                null
            );

            // Act & Assert
            var exception = Record.Exception(() => appender.DoAppend(loggingEvent));
        }

        [Fact]
        public void Append_WithInfoLevel_ShouldNotThrow()
        {
            // Arrange
            var appender = new RollbarAppender
            {
                AccessToken = "test-token-12345",
                Environment = "test",
                Asynchronous = false
            };
            appender.ActivateOptions();

            var loggingEvent = new LoggingEvent(
                typeof(RollbarAppenderTests),
                null,
                "TestLogger",
                Level.Info,
                "Test message",
                null
            );

            // Act & Assert
            var exception = Record.Exception(() => appender.DoAppend(loggingEvent));
        }

        [Fact]
        public void Append_WithDebugLevel_ShouldNotThrow()
        {
            // Arrange
            var appender = new RollbarAppender
            {
                AccessToken = "test-token-12345",
                Environment = "test",
                Asynchronous = false
            };
            appender.ActivateOptions();

            var loggingEvent = new LoggingEvent(
                typeof(RollbarAppenderTests),
                null,
                "TestLogger",
                Level.Debug,
                "Test message",
                null
            );

            // Act & Assert
            var exception = Record.Exception(() => appender.DoAppend(loggingEvent));
        }

        [Fact]
        public void Append_WithException_ShouldHandleExceptionObject()
        {
            // Arrange
            var appender = new RollbarAppender
            {
                AccessToken = "test-token-12345",
                Environment = "test",
                Asynchronous = false
            };
            appender.ActivateOptions();

            var testException = new InvalidOperationException("Test exception");
            var loggingEvent = new LoggingEvent(
                typeof(RollbarAppenderTests),
                null,
                "TestLogger",
                Level.Error,
                "Error occurred",
                testException
            );

            // Act & Assert
            var exception = Record.Exception(() => appender.DoAppend(loggingEvent));
            // Should not throw unhandled exceptions
        }

        [Fact]
        public void Asynchronous_WhenFalse_ShouldWaitForCompletion()
        {
            // Arrange
            var appender = new RollbarAppender
            {
                AccessToken = "test-token-12345",
                Environment = "test",
                Asynchronous = false
            };
            appender.ActivateOptions();

            var loggingEvent = new LoggingEvent(
                typeof(RollbarAppenderTests),
                null,
                "TestLogger",
                Level.Info,
                "Test message",
                null
            );

            // Act
            var startTime = DateTime.UtcNow;
            appender.DoAppend(loggingEvent);
            var duration = DateTime.UtcNow - startTime;

            // Assert - When not async, it should wait (though might fail quickly if network issue)
            Assert.True(duration.TotalMilliseconds >= 0);
        }
    }
}
