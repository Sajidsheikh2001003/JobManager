using JobManager.Services;
using System.ComponentModel.DataAnnotations;

namespace JobManager.ViewModels
{
    public class AddJobViewModel
    {
        public AddJobViewModel()
        {
            JobsList = new List<JobsViewModel>();
            PeekViewModelList = new List<PickedViewModel>();
            UnPickedList = new List<UnPickedViewModel>();
        }

        public string? UserName { get; set; }
        public List<PickedViewModel> PeekViewModelList { get; set; }
        public List<UnPickedViewModel> UnPickedList { get; set; }
        [Display(Name ="Job Title")]
        public string Text { get; set; }
        [ValidatePdfFile]
        public IFormFile File { get; set; }
        //public string FilePath { get; set; }
        public byte  IsCompleted { get; set; }
        public DateTime CompletedOn { get; set; }


        public List<JobsViewModel> JobsList { get; set; }
    }


    public enum JobStatus
    {
        UnPicked = 0,
        Picked = 1,
        Completed = 2
    }



    public class JobsViewModel
    {
        public int? Id { get; set; }
        public string? UserName { get; set; }
        public string FilePath { get; set; }
        public string Text { get; set; }
        public string JobName { get; set; }
        public byte IsCompleted { get; set; }
        public DateTime CompletedOn { get; set; }
        public string? UserId { get; set; }
        public int JobsId { get; set; }
    }
}
