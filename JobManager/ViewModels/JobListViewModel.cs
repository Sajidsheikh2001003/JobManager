namespace JobManager.ViewModels
{
    public class JobListViewModel
    {
        public int? Id { get; set; }
        public string? ProtectedId { get; set; }
        public string UserId { get; set; }
        public string JobTitle { get; set; }
        //public IFormFile File { get; set; }
        public string FilePath { get; set; }
        public byte IsCompleted { get; set; }
        //public DateTime CompletedOn { get; set; }
        //public bool UnPickedJobs { get; set; }
    }
}
