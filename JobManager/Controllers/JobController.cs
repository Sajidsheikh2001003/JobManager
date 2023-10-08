using JobManager.Data;
using JobManager.Models;
using JobManager.Services;
using JobManager.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace JobManager.Controllers
{


    [Authorize(Roles ="Admin")]
    public class JobController : Controller
    {
        private readonly IJobInterface jobInterface;
        private readonly IWebHostEnvironment webHostEnvironment;
        private readonly UserManager<ApplicationUser> userManager;

        public JobController(IJobInterface jobInterface, IWebHostEnvironment webHostEnvironment,
            UserManager<ApplicationUser> userManager)
        {
            this.jobInterface = jobInterface;
            this.webHostEnvironment = webHostEnvironment;
            this.userManager = userManager;
        }
        public async Task<IActionResult> Index(string? errorMessage, int? errorCode)
        {
            return View();
        }


        public async Task<IActionResult> ListOfJobs(string? errorMessage, int? errorCode)
        {
            AddJobViewModel model = new AddJobViewModel();


            //var user = await userManager.GetUserAsync(User);
            //var userName = user.UserName;
            var result = await jobInterface.Get(model).ConfigureAwait(false);
            if (!result.task.IsSuccess)
            {
                return View("Error", new ErrorViewModel
                {
                    ErrorCode = result.task.ErrorCode,
                    ErrorMessage = result.task.ErrorMessage
                });
            }
            if (errorCode is not null)
            {
                ModelState.AddModelError("", errorMessage);
                return View(model);
            }
            model = result.model;
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int Id)
        {
            var result = await jobInterface.Delete(Id).ConfigureAwait(false);
            if (!result.IsSuccess)
            {
                return View("Error", new ErrorViewModel
                {
                    ErrorCode = result.ErrorCode,
                    ErrorMessage = result.ErrorMessage
                });
            }
            return RedirectToAction(nameof(ListOfJobs));
        }


        [HttpPost]
        public async Task<IActionResult> Add(AddJobViewModel model)
        {

            if (ModelState.IsValid)
            {
                string path = Path.Combine(webHostEnvironment.WebRootPath, "Files");
                var resultPdf = await jobInterface.UploadPDFFile(model.File).ConfigureAwait(false);
                if (!resultPdf.task.IsSuccess)
                {
                    return View("Error", new ErrorViewModel
                    {
                        ErrorCode = resultPdf.task.ErrorCode,
                        ErrorMessage = resultPdf.task.ErrorMessage
                    });
                }

                var filePath = resultPdf.FilePath;
                var fileName = resultPdf.FileName;

                var user = await userManager.GetUserAsync(User);
                var userId = user.Id;
               

                var result = await jobInterface.Add(model, filePath,userId).ConfigureAwait(false);
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
            var errors = ModelState.Where(x => x.Value.Errors.Count > 0).Select(x => new { x.Key, x.Value.Errors });
            string errorMessage = string.Empty;
            foreach (var item in errors)
            {
                errorMessage += item.Key.ToString() + " field is required.\n";
            }

            return RedirectToAction(nameof(Index),new {errorMessage,errorCode = 1000});

         
        }
    }
}
