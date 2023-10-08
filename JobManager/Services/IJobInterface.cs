using EasyQ.Models;
using JobManager.Data;
using JobManager.ViewModels;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using NuGet.Packaging.Signing;
using NuGet.Versioning;
using System.Threading.Tasks;

namespace JobManager.Services
{


    public interface IJobInterface
    {
        Task<(ErrorCodeModel task, string FilePath, string FileName)> UploadPDFFile(IFormFile file);
        Task<(ErrorCodeModel task,int Id)> Add(AddJobViewModel model, string filePath,string userId);
        //Task<(ErrorCodeModel task, List<JobsViewModel> jobs)> GetLinks();
        Task<(ErrorCodeModel task, List<JobListViewModel> jobs)> GetJobsList();
        Task<(ErrorCodeModel task, AddJobViewModel model)> Get(AddJobViewModel model);
        Task<ErrorCodeModel> Delete(int Id);
       
        Task<(ErrorCodeModel task, int Id)> PickedJob(int Id,string userId);
        Task<(ErrorCodeModel task, int Id)> UnPickedJob(int Id);
        Task<(ErrorCodeModel task, int Id)> Complete(int Id, string userId, string UserName);

    }
    public class IJobRepository : IJobInterface
    {
        private readonly IWebHostEnvironment webHostEnvironment;
        private readonly ApplicationDbContext dbContext;
        private readonly IDataProtector protector;

        public IJobRepository(IWebHostEnvironment webHostEnvironment, ApplicationDbContext dbContext,
            IDataProtectionProvider dataProtectionProvider, DataProtectionPurposeString dataProtectionPurposeString)
        {
            this.webHostEnvironment = webHostEnvironment;
            this.dbContext = dbContext;
            this.protector = dataProtectionProvider.CreateProtector(dataProtectionPurposeString.Code);
        }

        public async Task<(ErrorCodeModel task, int Id)> Add(AddJobViewModel model,string filePath,string userId)
        {
            ErrorCodeModel task = new(isSuccess: false, errorCode: 6001, errorMessage: String.Empty);
            var id = 0;
            using (var transaction = await dbContext.Database.BeginTransactionAsync())
            {

                Jobs addJob = new Jobs
                {
                    FilePath = filePath,
                    CompletedOn = DateTime.Now,
                    IsCompleted = (byte)JobStatus.UnPicked,
                    JobName = model.Text,
                   // UserId = userId
                };
                id = addJob.Id;
                await dbContext.Jobs.AddAsync(addJob);
                await dbContext.SaveChangesAsync();
                await transaction.CommitAsync();
                task.IsSuccess = true;
                return (task, id);
            }
        }

        public async Task<(ErrorCodeModel task, int Id)> Complete(int Id,string userId,string UserName)
        {
            ErrorCodeModel task = new ErrorCodeModel(false, 0, string.Empty);
            if (Id == 0)
            {
                task.IsSuccess = false;
                task.ErrorMessage = "Id not found";
            }
            try
            {

                var job = await dbContext.Jobs.FindAsync(Id);
                if (job != null)
                {
                    job.IsCompleted = (byte)JobStatus.Completed;
                    job.CompletedBy = UserName;
                }
                await dbContext.SaveChangesAsync();
                task.IsSuccess = true;
            }
            catch (Exception ex)
            {
                task.ErrorCode = 4003;
                task.ErrorMessage = $"Error Code:{task.ErrorCode} : Failed to Get Pick Job. Error : {ex.Message}";
            }

            return (task, Id);
        }

        public async Task<ErrorCodeModel> Delete(int Id)
        {
            ErrorCodeModel task = new(isSuccess: false, errorCode: 6001, errorMessage: String.Empty);
            using (var transaction = await dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    if (Id == 0)
                    {
                        throw new ArgumentNullException(nameof(Id), "Invalid Id");
                    }
                    //  var _id = Convert.ToInt32(protector.Unprotect(id));
                    var job = await dbContext.Jobs.FindAsync(Id);
                    if (job != null)
                    {
                        dbContext.Jobs.Remove(job);
                        await dbContext.SaveChangesAsync();
                        await transaction.CommitAsync();
                        task.IsSuccess = true;
                    }

                    else
                    {
                        task.ErrorCode = 120000;
                        task.IsSuccess = false;
                        task.ErrorMessage = $"Error Code:{task.ErrorCode} : Record not found. Id:{Id}";
                    }
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    task.ErrorCode = 120001;
                    task.ErrorMessage = $"Error Code:{task.ErrorCode} : Failed to Delete . Error : {ex.Message}";
                    task.IsSuccess = false;
                }
            }
            return task;
        }

        public async Task<(ErrorCodeModel task, AddJobViewModel model)> Get(AddJobViewModel model)
        {
            ErrorCodeModel task = new ErrorCodeModel(false, 0, string.Empty);
            try
            {

                var jobs = await dbContext.Jobs.ToListAsync();
                model.JobsList.AddRange(jobs.Select(j => new JobsViewModel
                {
                    CompletedOn = j.CompletedOn,
                    FilePath = j.FilePath,
                    IsCompleted = j.IsCompleted,
                    Text = j.JobName,
                    Id = j.Id,
                    JobName = j.JobName,
                    JobsId = j.Id,
                    UserId = j.UserId,
                    UserName = j.CompletedBy

                }).ToList());
                task.IsSuccess = true;
            }
            catch (Exception ex)
            {
                task.ErrorCode = 4003;
                task.ErrorMessage = $"Error Code:{task.ErrorCode} : Failed to Get Details. Error : {ex.Message}";
            }
            return (task, model);



        }

        public async Task<(ErrorCodeModel task, List<JobListViewModel> jobs)> GetJobsList()
        {
            List<JobListViewModel> model = new List<JobListViewModel>();
            ErrorCodeModel task = new ErrorCodeModel(false, 0, string.Empty);
            try
            {
                var list = await (from j in dbContext.Jobs
                                  select new JobListViewModel
                                  {
                                      FilePath = j.FilePath,
                                      Id = j.Id,
                                      IsCompleted = j.IsCompleted,
                                      JobTitle = j.JobName,
                                      UserId = j.UserId
                                  }).AsNoTracking().ToListAsync();
                model = list;
                task.IsSuccess = true;
            }
            catch (Exception ex)
            {
                task.ErrorCode = 4003;
                task.ErrorMessage = $"Error Code:{task.ErrorCode} : Failed to Get Details. Error : {ex.Message}";
            }

            return (task, model);

        }

        //public async Task<(ErrorCodeModel task, List<JobsViewModel> jobs)> GetLinks()
        //{


        //    var job = await (from j in dbContext.Jobs
        //                     select new JobsViewModel
        //                     {
        //                         FilePath = j.FilePath,

        //                     }).AsNoTracking().ToListAsync();
        //    throw new NotImplementedException();
        //}

        public async Task<(ErrorCodeModel task, int Id)> PickedJob(int Id,string userId)
        {
            ErrorCodeModel task = new ErrorCodeModel(false, 0, string.Empty);
            if (Id == 0)
            {
                task.IsSuccess = false;
                task.ErrorMessage = "Id not found";
            }
            if(string.IsNullOrEmpty(userId))
            {
                task.IsSuccess = false;
                task.ErrorMessage = "User Id Not Found";
            }
            try
            {
                
                var job = await dbContext.Jobs.Where(s=>s.Id == Id && s.UserId == null).FirstOrDefaultAsync();
                if (job != null)
                {
                    
                    job.IsCompleted = (byte)JobStatus.Picked;
                    job.UserId = userId;
                    await dbContext.SaveChangesAsync();
                    task.IsSuccess = true;
                }
                else
                {
                    task.IsSuccess = false;
                    task.ErrorMessage = "Job not available, please reload the page";
                }
            }
            catch (Exception ex)
            {
                task.ErrorCode = 4003;
                task.ErrorMessage = $"Error Code:{task.ErrorCode} : Failed to Get Pick Job. Error : {ex.Message}";
            }

            return (task, Id);
        }

        public async Task<(ErrorCodeModel task, int Id)> UnPickedJob(int Id)
        {
            ErrorCodeModel task = new ErrorCodeModel(false, 0, string.Empty);
            if (Id == 0)
            {
                task.IsSuccess = false;
                task.ErrorMessage = "Id not found";
            }
            try
            {

                var job = await dbContext.Jobs.FindAsync(Id);
                if (job != null)
                {
                    job.IsCompleted = (byte)JobStatus.UnPicked;
                    job.UserId = null;
                }
                await dbContext.SaveChangesAsync();
                task.IsSuccess = true;
            }
            catch (Exception ex)
            {
                task.ErrorCode = 4003;
                task.ErrorMessage = $"Error Code:{task.ErrorCode} : Failed to Get Pick Job. Error : {ex.Message}";
            }

            return (task, Id);
        }



        //public async Task<(ErrorCodeModel task, AddJobViewModel model)> GetPickedJobs(AddJobViewModel model)
        //{
        //    ErrorCodeModel task = new ErrorCodeModel(false, 0, string.Empty);

        //    //AddJobViewModel model = new AddJobViewModel();
        //    try
        //    {

        //        var jobs = await dbContext.PickedJobs.ToListAsync();
        //        model.PeekViewModelList.AddRange(jobs.Select(j => new PickedViewModel
        //        {
        //            CompletedOn = j.CompletedOn,
        //            FilePath = j.FilePath,
        //            IsCompleted = j.IsCompleted,
        //            Text = j.JobName,
        //            //Id = j.Id

        //        }).ToList());
        //        task.IsSuccess = true;
        //        task.IsSuccess = true;
        //    }
        //    catch (Exception ex)
        //    {
        //        task.ErrorCode = 4003;
        //        task.ErrorMessage = $"Error Code:{task.ErrorCode} : Failed to Get Details. Error : {ex.Message}";
        //    }
        //    return (task, model);
        //}
        //public async Task<(ErrorCodeModel task, CheckViewModel model)> GetPickedJob(CheckViewModel model)
        //{
        //    ErrorCodeModel task = new ErrorCodeModel(false, 0, string.Empty);
        //    try
        //    {
        //        //UnpickedJobs true means job is un picked
        //        PickedViewModel unPickedView = new PickedViewModel();
        //        var jobs = await dbContext.Jobs.ToListAsync();

        //        var check = await(from j in dbContext.Jobs
        //                          where j.UnPickedJobs == false
        //                          select new PickedViewModel
        //                          {
        //                              CompletedOn = j.CompletedOn,
        //                              FilePath = j.FilePath,
        //                              IsCompleted = j.IsCompleted,
        //                              JobName = j.JobName,
        //                              JobsId = j.Id,
        //                              Text = j.JobName,
        //                              UserId = j.UserId
        //                          }).AsNoTracking().ToListAsync();

        //        model.PickedList.AddRange(jobs.Select(j => new PickedViewModel
        //        {
        //            CompletedOn = j.CompletedOn,
        //            FilePath = j.FilePath,
        //            IsCompleted = j.IsCompleted,
        //            Text = j.JobName,
        //            JobsId = j.Id,
        //            JobName = j.JobName,
        //            PickedStatus = false

        //        }).ToList());
        //        task.IsSuccess = true;
        //    }
        //    catch (Exception ex)
        //    {
        //        task.ErrorCode = 4003;
        //        task.ErrorMessage = $"Error Code:{task.ErrorCode} : Failed to Get Details. Error : {ex.Message}";
        //    }
        //    return (task, model);

        //}


        //public async Task<(ErrorCodeModel task, CheckViewModel model)> GetUnPickedJob(CheckViewModel model)
        //{
        //    ErrorCodeModel task = new ErrorCodeModel(false, 0, string.Empty);
        //    try
        //    {
        //        //UnpickedJobs true means job is un picked
        //        UnPickedViewModel unPickedView = new UnPickedViewModel();
        //        var jobs = await dbContext.Jobs.ToListAsync();

        //        var check = await(from j in dbContext.Jobs
        //                          where j.UnPickedJobs == true
        //                          select new UnPickedViewModel
        //                          {
        //                              CompletedOn = j.CompletedOn,
        //                              FilePath= j.FilePath,
        //                              IsCompleted = j.IsCompleted,
        //                              JobName = j.JobName,
        //                              JobsId = j.Id,
        //                              Text = j.JobName,
        //                              UserId = j.UserId 
        //                          } ).AsNoTracking().ToListAsync();
        //        if(check!=null)
        //        {
        //            model.UnPickedList.AddRange(jobs.Select(j => new UnPickedViewModel
        //            {
        //                CompletedOn = j.CompletedOn,
        //                FilePath = j.FilePath,
        //                IsCompleted = j.IsCompleted,
        //                Text = j.JobName,
        //                JobsId = j.Id,
        //                JobName= j.JobName,
        //                PickedStatus = true

        //            }).ToList());
        //            task.IsSuccess = true;
        //        }


        //    }
        //    catch (Exception ex)
        //    {
        //        task.ErrorCode = 4003;
        //        task.ErrorMessage = $"Error Code:{task.ErrorCode} : Failed to Get Details. Error : {ex.Message}";
        //    }
        //    return (task, model);

        //}

        //public async Task<(ErrorCodeModel task, PickedViewModel peekModel)> Picked(int id, string userId)
        //{
        //    ErrorCodeModel task = new ErrorCodeModel(isSuccess: false, errorCode: 4000, errorMessage: string.Empty);
        //    PickedViewModel model = new PickedViewModel();
        //    AddJobViewModel listAdd = new AddJobViewModel();
        //    if (userId == null)
        //    {
        //        task.ErrorCode = 4003;
        //        task.ErrorMessage = $"Error Code:{task.ErrorCode} : Failed to Get User Id.";
        //    }
        //    var userid = userId;
        //    try
        //    {



        //        var listJob = await (from j in dbContext.Jobs
        //                             where j.Id == id
        //                             select new PickedViewModel
        //                             {
        //                                 CompletedOn = j.CompletedOn,
        //                                 FilePath = j.FilePath,
        //                                 IsCompleted = j.IsCompleted,
        //                                 JobName = j.JobName,
        //                                 JobsId = j.Id,
        //                                  Text = j.JobName,
        //                                 UserId = userid
        //                             }).AsNoTracking().FirstOrDefaultAsync();

        //        if (listJob != null)
        //        {
        //            model = listJob;
        //            PickedJobs peekJob = new PickedJobs
        //            {
        //                CompletedOn = model.CompletedOn,
        //                FilePath = model.FilePath,
        //                JobsId = model.JobsId,
        //                JobName = model.JobName,
        //                IsCompleted = model.IsCompleted,
        //                UserId = userId,

        //            };
        //            await dbContext.PickedJobs.AddAsync(peekJob);
        //            await dbContext.SaveChangesAsync();




        //            var job = await dbContext.Jobs.FindAsync(model.JobsId);

        //            if (peekJob != null)
        //            {
        //                dbContext.Jobs.Remove(job);
        //                await dbContext.SaveChangesAsync();
        //                task.IsSuccess = true;
        //            }


        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        task.ErrorCode = 120001;
        //        task.ErrorMessage = $"Error Code:{task.ErrorCode} : Failed to Delete . Error : {ex.Message}";
        //        task.IsSuccess = false;
        //    }

        //    return (task, model);
        //}

        //public async Task<ErrorCodeModel> JobPickedFalse(int id)
        //{
        //    ErrorCodeModel task = new ErrorCodeModel(isSuccess: false, errorCode: 4000, errorMessage: string.Empty);
        //    try
        //    {

        //        if (id == 0)
        //        {
        //            task.ErrorMessage = "Id Not Found";
        //            task.IsSuccess = false;
        //        }
        //        var picked = await dbContext.Jobs.FindAsync(id);
        //        if(picked == null)
        //        {
        //            task.ErrorCode = 6005;
        //            task.ErrorMessage = $"Error Code: {task.ErrorCode} : Record not found. Id {id}";
        //        }
        //        else
        //        {
        //            //link.ImageUrl = model.ImageUrl;
        //            picked.UnPickedJobs = false;

        //            await dbContext.SaveChangesAsync();
        //            task.IsSuccess = true;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        task.ErrorCode = 6006;
        //        task.ErrorMessage = $"Error Code:{task.ErrorCode} : Failed to Picked Job. Error : {ex.Message}";
        //    }

        //    return task;
        //}

        public async Task<(ErrorCodeModel task, string FilePath, string FileName)> UploadPDFFile(IFormFile file)
        {
            ErrorCodeModel task = new ErrorCodeModel(isSuccess: false, errorCode: 4000, errorMessage: string.Empty);
            string fileName = Path.GetFileName(file.FileName);
            string newFileName = Guid.NewGuid().ToString() + "_" + fileName;
            string localFilePath = Path.Combine("Files", newFileName);
            string path = Path.Combine(webHostEnvironment.WebRootPath, "Files");
            string filePath = Path.Combine(path, newFileName);

            try
            {
                using (FileStream stream = new FileStream(filePath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }
                task.IsSuccess = true;
            }
            catch (Exception ex)
            {
                task.ErrorMessage = $"Error Code:{task.ErrorCode} : Failed to upload file. Error : {ex.Message}";
            }
            return (task, localFilePath, fileName);
        }
    }
}
