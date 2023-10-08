namespace JobManager.ViewModels
{
    public class PickedViewModel
    {
        public string FilePath { get; set; }
        public string Text { get; set; }
        public string JobName { get; set; }
        public byte IsCompleted { get; set; }
        public DateTime CompletedOn { get; set; }
        public string? UserId { get; set; }
        public int JobsId { get; set; }
        public bool PickedStatus { get; set; }
    }

    public class UnPickedViewModel
    {
        public string FilePath { get; set; }
        public string Text { get; set; }
        public string JobName { get; set; }
        public byte IsCompleted { get; set; }
        public DateTime CompletedOn { get; set; }
        public string? UserId { get; set; }
        public int JobsId { get; set; }
        public bool PickedStatus { get; set; }

    }
}
