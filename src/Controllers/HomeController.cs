using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Articulate.Models;
using Articulate.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Steeltoe.CloudFoundry.Connector;
using Steeltoe.CloudFoundry.Connector.Services;
using Steeltoe.Common.Discovery;
using Steeltoe.Extensions.Configuration.CloudFoundry;

namespace Articulate.Controllers
{
    public class WithEnv : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var controller = (Controller) filterContext.Controller;
            controller.ViewBag.AppEnv = filterContext.HttpContext.RequestServices.GetService<AppEnv>();
            controller.ViewBag.HasEurekaBinding = filterContext.HttpContext.RequestServices.GetService<IConfiguration>().GetServiceInfos<EurekaServiceInfo>().Any();
            controller.ViewBag.CFApp = filterContext.HttpContext.RequestServices.GetService<IOptionsSnapshot<CloudFoundryApplicationOptions>>().Value;
        }
    }
    [WithEnv]
    public class HomeController : Controller
    {
        private readonly AttendeeContext _db;
        private readonly ILogger<HomeController> _log;
        private IOptionsSnapshot<CloudFoundryApplicationOptions> _app;

        public HomeController(AttendeeContext db, ILogger<HomeController> log, IOptionsSnapshot<CloudFoundryApplicationOptions> app)
        {
            _db = db;
            _log = log;
            _app = app;
        }
        
        public IActionResult Index()
        {
            return View();
        }

        [Route("/basics")]
        public IActionResult Kill(bool doIt)
        {
            if (doIt)
            {
                ViewBag.Killed = true;
                
                _log.LogWarning("*** The system is shutting down. ***");
                Task.Run(async () =>
                {
                    var name = Thread.CurrentThread.Name;
                    _log.LogWarning($"killing shortly {name}");
                    await Task.Delay(5000);
                    _log.LogWarning($"killed {name}");
                    Environment.Exit(0);
                });
            }

            return View("Basics");
        }

        [Route("/ssh-file")]
        public IActionResult WriteFile()
        {
            var fileName = "ers-ssh-demo.log";
            
            System.IO.File.WriteAllText(fileName,DateTime.Now.ToString("MM-dd-yy HH:mm:ss"));
            ViewBag.SSHFile = new FileInfo(fileName).FullName;
            return View("Basics");
        }

        [Route("/services")]
        public async Task<IActionResult> Attendees()
        {
            return View("Attendees", await _db.Attendees.ToListAsync());
        }
        [Route("/eureka")]
        public IActionResult ServiceDiscovery([FromServices]IDiscoveryClient discoveryClient)
        {

            var services = discoveryClient.Services
                .Select(serviceName => new DiscoveredService
                {
                    Name = serviceName, 
                    Urls = discoveryClient.GetInstances(serviceName).Select(x => x.Uri.ToString()).ToList()
                })
                .ToList();
            var uri = new Uri(_app.Value.CF_Api);
            var systemDomain = Regex.Replace(uri.Host, @"^.+?\.", string.Empty);
            ViewBag.MetricsUrl = $"https://metrics.{systemDomain}/apps/{_app.Value.Application_Id}/dashboard";
            return View("Eureka", services);
        }

        [Route("/ping")]
        public async Task<string> Ping([FromServices]IDiscoveryClient discoveryClient, string targets)
        {
            var pong = string.Empty;
            if (!string.IsNullOrEmpty(targets))
            {
                var httpClient = new HttpClient(new DiscoveryHttpClientHandler(discoveryClient));
                _log.LogTrace($"Ping received. Remaining targets: {targets}");
                var allTargets = targets.Split(",").Where(x => x != _app.Value.Name).ToList();
                
                if (allTargets.Any())
                {
                    var nextTarget = allTargets.First();
                    var remainingTargets = string.Join(",", allTargets.Skip(1));
                    try
                    {
                        _log.LogDebug($"Sending ping request to {nextTarget}");
                        pong = await httpClient.GetStringAsync($"https://{nextTarget}/ping/?targets={remainingTargets}");
                    }
                    catch (Exception e)
                    {
                        _log.LogError(e, $"Call to {nextTarget} failed");
                        pong = $"{nextTarget} failed to answer";
                    }
                }

            }
            return pong.Insert(0, $"Pong from {_app.Value.Name}\n");
        }
        [Route("/clean")]
        public async Task<IActionResult> Clean()
        {
            var attendees = await _db.Attendees.ToListAsync();
            _db.Attendees.RemoveRange(attendees);
            await _db.SaveChangesAsync();
            return await Attendees();
        }

        [HttpPost]
        [Route("/add-attendee")]
        public async Task<IActionResult> AddAttendee(string firstName, string lastName, string emailAddress)
        {
            var attendee = new Attendee()
            {
                FirstName = firstName,
                LastName = lastName,
                EmailAddress = emailAddress
            };
            _db.Attendees.Add(attendee);
            await _db.SaveChangesAsync();
            return await Attendees();
        }
        [Route("/bluegreen")]
        public IActionResult BlueGreen()
        {
            foreach (var envVar in Environment.GetEnvironmentVariables().Cast<DictionaryEntry>())
            {
                _log.LogInformation($"{envVar.Key}: {envVar.Value}");
            }

            return View();
        }

        [Route("/bluegreen-check")]
        public string[] BlueGreenCheck([FromServices]IOptionsSnapshot<CloudFoundryApplicationOptions> options)
        {
            return new []
            {
                options.Value.Application_Name, 
                options.Value.InstanceIndex.ToString()
            };
        }
    }
}