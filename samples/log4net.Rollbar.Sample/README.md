# log4net.Rollbar Sample Application

This sample console application demonstrates how to use the log4net.Rollbar appender in different ways.

## Prerequisites

- .NET 8.0 SDK or later
- A Rollbar account and access token (sign up at https://rollbar.com)

## Configuration

Before running the sample, you need to configure your Rollbar access token.

### Option 1: Using app.config (XML Configuration)

Edit `app.config` and replace `your-rollbar-access-token-here` with your actual Rollbar access token:

```xml
<appSettings>
  <add key="Rollbar.AccessToken" value="your-actual-token-here" />
  <add key="Rollbar.Environment" value="development" />
</appSettings>
```

### Option 2: Using Fluent API (Programmatic Configuration)

Edit `Program.cs` in the `ConfigureWithFluentAPI` method and replace the access token:

```csharp
var appender = new RollbarAppenderConfiguration()
    .WithAccessToken("your-actual-token-here")
    .WithEnvironment("development")
    .Build();
```

## Running the Sample

```bash
cd samples/log4net.Rollbar.Sample
dotnet run
```

## What the Sample Demonstrates

1. **XML Configuration**: Shows how to configure the appender using app.config
2. **Fluent API Configuration**: Shows how to configure the appender programmatically
3. **Different Log Levels**: Demonstrates Debug, Info, Warn, and Error logging
4. **Exception Logging**: Shows how exceptions are captured and sent to Rollbar
5. **Custom Properties**: Demonstrates attaching custom data to log events

## Expected Output

The application will log messages to both the console and Rollbar. Check your Rollbar dashboard at https://rollbar.com to see the logged events.

## Features Demonstrated

- Multiple configuration approaches
- Custom properties (UserId, RequestId)
- Exception tracking with stack traces
- Different severity levels (debug, info, warning, error)
- Asynchronous and synchronous logging modes
