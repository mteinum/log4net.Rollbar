using System;
using Xunit;

namespace log4net.Rollbar.Tests
{
    public class RollbarAppenderConfiguratorTests
    {
        [Fact]
        public void Configure_WithAccessToken_ShouldNotThrow()
        {
            // Arrange
            var accessToken = "test-access-token-12345";

            // Act & Assert
            var exception = Record.Exception(() => 
                RollbarAppenderConfigurator.Configure(accessToken));
            
            Assert.Null(exception);
        }

        [Fact]
        public void Configure_WithConfigureAction_ShouldInvokeAction()
        {
            // Arrange
            var accessToken = "test-access-token-12345";
            var actionInvoked = false;
            RollbarAppender capturedAppender = null;

            // Act
            RollbarAppenderConfigurator.Configure(accessToken, appender =>
            {
                actionInvoked = true;
                capturedAppender = appender;
                appender.Environment = "test-environment";
                appender.Platform = "test-platform";
            });

            // Assert
            Assert.True(actionInvoked);
            Assert.NotNull(capturedAppender);
            Assert.Equal("test-environment", capturedAppender.Environment);
            Assert.Equal("test-platform", capturedAppender.Platform);
        }

        [Fact]
        public void Configure_ShouldSetAccessToken()
        {
            // Arrange
            var accessToken = "test-access-token-12345";
            RollbarAppender capturedAppender = null;

            // Act
            RollbarAppenderConfigurator.Configure(accessToken, appender =>
            {
                capturedAppender = appender;
            });

            // Assert
            Assert.NotNull(capturedAppender);
            Assert.Equal(accessToken, capturedAppender.AccessToken);
        }

        [Fact]
        public void Configure_ShouldActivateOptions()
        {
            // Arrange
            var accessToken = "test-access-token-12345";
            var configureActionCalled = false;

            // Act
            RollbarAppenderConfigurator.Configure(accessToken, appender =>
            {
                configureActionCalled = true;
            });

            // Assert
            Assert.True(configureActionCalled);
        }

        [Fact]
        public void Configure_WithMultipleProperties_ShouldSetAllProperties()
        {
            // Arrange
            var accessToken = "test-access-token-12345";
            RollbarAppender capturedAppender = null;

            // Act
            RollbarAppenderConfigurator.Configure(accessToken, appender =>
            {
                appender.Environment = "production";
                appender.Framework = ".NET Framework 4.8";
                appender.Language = "C#";
                appender.Platform = "Windows";
                appender.GitSha = "abc123def456";
                appender.ScrubParams = "password,apiKey,secret";
                appender.Asynchronous = false;
                capturedAppender = appender;
            });

            // Assert
            Assert.NotNull(capturedAppender);
            Assert.Equal("production", capturedAppender.Environment);
            Assert.Equal(".NET Framework 4.8", capturedAppender.Framework);
            Assert.Equal("C#", capturedAppender.Language);
            Assert.Equal("Windows", capturedAppender.Platform);
            Assert.Equal("abc123def456", capturedAppender.GitSha);
            Assert.Equal("password,apiKey,secret", capturedAppender.ScrubParams);
            Assert.False(capturedAppender.Asynchronous);
        }
    }
}
