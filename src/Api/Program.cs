using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.StaticWebAssets;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;

namespace Api
{
    public static class Program
    {
        public static async Task<int> Main(string[] args)
        {
            Activity.DefaultIdFormat = ActivityIdFormat.W3C;

            try
            {
                var host = CreateHostBuilder(args).Build();

                await host.RunAsync()
                          .ContinueWith(_ => { })
                          .ConfigureAwait(false);
            }
            catch (Exception e)
            {
                Log.Logger.Error(e, e.Message);
                return -1;
            }

            return 0;
        }

        private static IHostBuilder CreateHostBuilder(string[] args)
        {
            return new HostBuilder()
                  .UseContentRoot(Directory.GetCurrentDirectory())
                  .ConfigureAppConfiguration((context, builder) =>
                   {
                       var env = context.HostingEnvironment;

                       builder.AddJsonFile("appsettings.json", false, false)
                              .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true, true)
                              .AddEnvironmentVariables()
                              .AddCommandLine(args);

                       context.Configuration = builder.Build();
                   })
                  .ConfigureLogging((context, builder) =>
                   {
                       builder.ClearProviders();

                       var loggerBuilder = new LoggerConfiguration()
                                          .Enrich.FromLogContext()
                                          .MinimumLevel.Information();

                       loggerBuilder.Enrich.WithMachineName()
                                    .Enrich.WithEnvironmentName()
                                    .MinimumLevel.Debug()
                                     //.MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                                     //.MinimumLevel.Override("System", LogEventLevel.Warning)
                                    .WriteTo.Console(outputTemplate:
                                                     "[{Timestamp:HH:mm:ss} {Level} {EnvironmentName}-{MachineName}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}{NewLine}",
                                                     theme: AnsiConsoleTheme.Literate);

                       builder.AddSerilog(loggerBuilder.CreateLogger());
                   })
                  .UseDefaultServiceProvider((context, options) =>
                   {
                       var isDevelopment = context.HostingEnvironment.IsDevelopment();
                       options.ValidateScopes = isDevelopment;
                       options.ValidateOnBuild = isDevelopment;
                   })
                  .ConfigureServices((hostContext, services) =>
                   {
                       services.AddOptions();
                       services.AddRouting();
                   })
                  .ConfigureWebHost(builder =>
                   {
                       builder.ConfigureAppConfiguration((ctx, cb) =>
                       {
                           if (ctx.HostingEnvironment.IsDevelopment())
                           {
                               StaticWebAssetsLoader.UseStaticWebAssets(ctx.HostingEnvironment, ctx.Configuration);
                           }
                       });
                       builder.UseContentRoot(Directory.GetCurrentDirectory());
                       builder.UseKestrel((context, options) => { options.Configure(context.Configuration.GetSection("Kestrel")); });
                       builder.UseStartup<Startup>();
                   });
        }
    }
}
