using JobManager.Data;
using JobManager.Models;
using JobManager.Services;
using JobManager.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Policy;

namespace JobManager.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IJobInterface jobInterface;
        private readonly UserManager<ApplicationUser> userManager;

        public HomeController(ILogger<HomeController> logger,IJobInterface jobInterface, UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            this.jobInterface = jobInterface;
            this.userManager = userManager;
        }
        
        public async Task<IActionResult> Index()
        {
              
            var result = await jobInterface.GetJobsList().ConfigureAwait(false);
            if (!result.task.IsSuccess)
            {
                return View("Error", new ErrorViewModel
                {
                    ErrorCode = result.task.ErrorCode,
                    ErrorMessage = result.task.ErrorMessage
                });

            }
            return View(result.jobs);
        }


        [Authorize]
        public async Task<IActionResult> ListOfJobs()
        {
            var result = await jobInterface.GetJobsList().ConfigureAwait(false);
            if (!result.task.IsSuccess)
            {
                return View("Error", new ErrorViewModel
                {
                    ErrorCode = result.task.ErrorCode,
                    ErrorMessage = result.task.ErrorMessage
                });

            }
            return View(result.jobs);
        }

        [HttpPost]
        public async Task<IActionResult> PickJob(int Id)
        {
            var user = await userManager.GetUserAsync(User);
            var userId = user.Id;

            var result = await jobInterface.PickedJob(Id,userId).ConfigureAwait(false);
            if (!result.task.IsSuccess)
            {
                return View("Error", new ErrorViewModel
                {
                    ErrorCode = result.task.ErrorCode,
                    ErrorMessage = result.task.ErrorMessage
                });

            }

            return RedirectToAction(nameof(ListOfJobs));
        }


        [HttpPost]
        public async Task<IActionResult> UnPickedJob(int Id)
        {
            var result = await jobInterface.UnPickedJob(Id).ConfigureAwait(false);
            if (!result.task.IsSuccess)
            {
                return View("Error", new ErrorViewModel
                {
                    ErrorCode = result.task.ErrorCode,
                    ErrorMessage = result.task.ErrorMessage
                });

            }

            return RedirectToAction(nameof(ListOfJobs));
        }


        //[HttpPost]
        public async Task<IActionResult> Complete(int Id)
        {
            var user = await userManager.GetUserAsync(User);
            var userName = user.UserName;
            var userId = user.Id;
            var result = await jobInterface.Complete(Id,userId,userName).ConfigureAwait(false);
            if (!result.task.IsSuccess)
            {
                return View("Error", new ErrorViewModel
                {
                    ErrorCode = result.task.ErrorCode,
                    ErrorMessage = result.task.ErrorMessage
                });

            }
            return RedirectToAction(nameof(ListOfJobs));
        }

        public IActionResult Privacy()
        {
            return View();
        }



      

       
        public IActionResult Test()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}