# log4net.Rollbar

[![Build](https://github.com/mteinum/log4net.Rollbar/actions/workflows/build.yml/badge.svg)](https://github.com/mteinum/log4net.Rollbar/actions/workflows/build.yml)

RollbarAppender is a custom log4net appender for reporting events to Rollbar. This package depends on the RollbarSharp package.

https://rollbar.com

## Install

```
Install-Package log4net.Rollbar
```

## Configuration

### config file

You must add the appender to your log4net section in app or web.config

#### Example

```xml
<log4net>
  <appender name="RollbarAppender" type="log4net.Rollbar.RollbarAppender, log4net.Rollbar">
    <param name="AccessToken" value="..." />
  </appender>
  <root>
      <level value="ERROR" />
      <appender-ref ref="RollbarAppender" />
    </root>
</log4net>
```
The AccessToken is the post_server_item key. If not specified as a parameter you must add it to the &lt;appSettings&gt; with &lt;add key="Rollbar.AccessToken" value="..."/&gt;

### Code

Or you can use the RollbarAppenderConfigurator when your application initializes.

#### Example

```cs
class Program
{
  static void Main(string[] args)
  {
    RollbarAppenderConfigurator.Configure("<accessToken>");
  }
}
```
