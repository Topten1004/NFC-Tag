namespace BqsClinoTag.ViewModel
{
    public class PassageLogVM
    {
        public PassageLogVM() {

            LocationTasks = new List<LocationTaskVM>();
            PassageLogs = new List<PassageVM>();
        }

        public List<LocationTaskVM> LocationTasks { get; set; } = new List<LocationTaskVM>();
        public List<PassageVM> PassageLogs { get; set; } = new List<PassageVM>();
    }
}
