﻿using System;
using System.IO;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace MicroService
{
  public class Program
  {
    public static readonly string AppName = typeof(Program).Namespace;

    public static int Main(string[] args)
    {
      var configuration = GetConfiguration();
      Log.Logger = CreateSerilogLogger(configuration);
      try
      {
        Log.Information("Configuring web host ({ApplicationContext})...", AppName);
        var host = BuildWebHost(configuration, args);

        Log.Information("Starting web host ({ApplicationContext})...", AppName);
        host.Run();

        return 0;
      }
      catch (Exception ex)
      {
        Log.Fatal(ex, "Program terminated unexpectedly ({ApplicationContext})!", AppName);
        return 1;
      }
      finally
      {
        Log.CloseAndFlush();
      }
    }
    private static IWebHost BuildWebHost(IConfiguration configuration, string[] args) =>
        WebHost.CreateDefaultBuilder(args)
            .CaptureStartupErrors(false)
            .UseStartup<Startup>()
            // .UseApplicationInsights()
            .UseContentRoot(Directory.GetCurrentDirectory())
            .UseConfiguration(configuration)
            .UseSerilog()
            .Build();

    private static Serilog.ILogger CreateSerilogLogger(IConfiguration configuration)
    {
      var seqServerUrl = configuration["Serilog:SeqServerUrl"];
      var logstashUrl = configuration["Serilog:LogstashUrl"];
      return new LoggerConfiguration()
          .MinimumLevel.Verbose()
          .Enrich.WithProperty("ApplicationContext", AppName)
          .Enrich.FromLogContext()
          .WriteTo.Console()
          .WriteTo.Seq(string.IsNullOrWhiteSpace(seqServerUrl) ? "http://seq" : seqServerUrl)
          .WriteTo.Http(string.IsNullOrWhiteSpace(logstashUrl) ? "http://logstash:8080" : logstashUrl)
          .ReadFrom.Configuration(configuration)
          .CreateLogger();
    }
    private static IConfiguration GetConfiguration()
    {
      var builder = new ConfigurationBuilder()
          .SetBasePath(Directory.GetCurrentDirectory())
          .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
          .AddEnvironmentVariables();
      var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

      if (environment == EnvironmentName.Development)
      {
        builder.AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true);
      }
      return builder.Build();
    }
  }
}
