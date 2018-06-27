using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Articulate.Models;
using Articulate.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Steeltoe.Extensions.Configuration.CloudFoundry;

namespace Articulate.Controllers
{
    public class WithEnv : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var controller = (Controller) filterContext.Controller;
            controller.ViewBag.AppEnv = filterContext.HttpContext.RequestServices.GetService<AppEnv>();
        }
    }
    [WithEnv]
    public class HomeController : Controller
    {
        private readonly AttendeeContext _db;
        private readonly ILogger<HomeController> _log;

        public HomeController(AttendeeContext db, ILogger<HomeController> log)
        {
            _db = db;
            _log = log;
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