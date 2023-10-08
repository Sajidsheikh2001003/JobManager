namespace JobManager.ViewModels
{
    public class CheckViewModel
    {
        public CheckViewModel()
        {
            PickedList = new List<PickedViewModel>();
            UnPickedList = new List<UnPickedViewModel>();
        }

        public List<PickedViewModel> PickedList { get; set; }
        public List<UnPickedViewModel> UnPickedList { get; set; }
    }
}
