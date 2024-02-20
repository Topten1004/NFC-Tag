namespace BqsClinoTag.ViewModel
{
    public class PassageVM
    {
        public PassageVM()
        {
            Tasks = new List<TaskAPIVM>();
        }

        public int Id { get; set; }

        public string Agent { get; set; } = string.Empty;

        public DateTime LogTime { get; set; }

        public List<TaskAPIVM> Tasks { get; set; } = new List<TaskAPIVM>();

    }
}