using System.Collections.Generic;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Steeltoe.Extensions.Configuration.CloudFoundry;
using Steeltoe.Extensions.Logging;

namespace Articulate
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseCloudFoundryHosting()
                .AddCloudFoundry()
//                .ConfigureAppConfiguration(builder =>
//                {
//                    // set spring app name to be same as name on the platform
//                    var config = builder.Build();
//                    builder.AddCloudFoundry();
//                    var options = new CloudFoundryApplicationOptions();
//                    config.GetSection("vcap:application").Bind(options);
//                    if (options.Name != null)
//                    {
//                        builder.AddInMemoryCollection(new Dictionary<string, string> {{"spring:application:name", options.Name}});
//                    }
//                })
                .ConfigureLogging((builderContext, loggingBuilder) =>
                {
                    loggingBuilder.AddConfiguration(builderContext.Configuration.GetSection("Logging"));
                    loggingBuilder.AddDynamicConsole();
                })
                .UseStartup<Startup>()
                .Build();
    }
    
    
}