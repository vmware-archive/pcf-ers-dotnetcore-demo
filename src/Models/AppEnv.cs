using System;
using System.Reflection;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Options;
using Steeltoe.Extensions.Configuration.CloudFoundry;

namespace Articulate.Models
{
    public class AppEnv
    {
        public AppEnv(IHttpContextAccessor context, 
            IOptionsSnapshot<CloudFoundryApplicationOptions> appInfo, 
            IOptionsSnapshot<CloudFoundryServicesOptions> services)
        {
            var connectionContext = context.HttpContext.Features.Get<IHttpConnectionFeature>();
            ContainerAddr = Environment.GetEnvironmentVariable("PORT") != null ? 
                $"{connectionContext.LocalIpAddress}:{connectionContext.LocalPort}" : "localhost";

            App = appInfo.Value;
            Services = services.Value;
            ClrVersion = GetCoreClrVersion();
            InstanceAddr = Environment.GetEnvironmentVariable("CF_INSTANCE_ADDR") ?? "localhost";

        }

        public string InstanceAddr { get; }

        public string ContainerAddr { get; }
        public CloudFoundryApplicationOptions App { get; }
        public CloudFoundryServicesOptions Services { get; }
        public string ClrVersion { get; }
        
        
        private static string coreCLRVersion = "NOT_YET_ASSESSED";

        public static string GetCoreClrVersion()
        {
            if (coreCLRVersion == "NOT_YET_ASSESSED")
            {
                Assembly assembly = typeof (object).GetTypeInfo().Assembly;
                Type type = (object) assembly != null ? assembly.GetType("System.AppDomain") : (Type) null;
                object obj1;
                if ((object) type == null)
                {
                    obj1 = (object) null;
                }
                else
                {
                    PropertyInfo property = type.GetProperty("CurrentDomain");
                    obj1 = (object) property != null ? property.GetValue((object) null) : (object) null;
                }
                object obj2 = obj1;
                object obj3;
                if ((object) type == null)
                {
                    obj3 = (object) null;
                }
                else
                {
                    MethodInfo method = type.GetMethod("GetData");
                    if ((object) method == null)
                        obj3 = (object) null;
                    else
                        obj3 = method.Invoke(obj2, (object[]) new string[1]
                        {
                            "FX_DEPS_FILE"
                        });
                }
                object obj4 = obj3;
                if (obj4 == null)
                {
                    coreCLRVersion = "";
                    return coreCLRVersion;
                }
                coreCLRVersion = GetCoreClrVersionImpl(obj4.ToString());
            }
            return coreCLRVersion;
        }

        internal static string GetCoreClrVersionImpl(string deps)
        {
            return Regex.Match(deps, "(?:(\\d+)\\.)?(?:(\\d+)\\.)?(?:(\\d+)\\.\\d+)").Value;
        }
        
    }

}