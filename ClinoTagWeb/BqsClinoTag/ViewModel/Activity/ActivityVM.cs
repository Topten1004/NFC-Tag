namespace BqsClinoTag.ViewModel.Activity
{
    public class ActivityVM
    {
        public ActivityVM() {
            datas = new List<ActivityItem>();
            tasks = new List<TaskVM>();
        }

        public List<ActivityItem> datas;

        public List<TaskVM> tasks { get; set; }

        public string comment { get; set; } = string.Empty!;

        public string photo { get; set; } = string.Empty!;

        public int passageId { get; set; } = 0;

        public int? flag { get; set; } = 0; // 0: Normal 1: Comment, 2: Photo

        public string? lieu { get; set; } = string.Empty!;

    }

    public class ActivityItem
    {
        public int IdLieu { get; set; }
        public string Nom { get; set; } = null!;
        public int IdClient { get; set; }
        public string UidTag { get; set; } = null!;

        public int ActionType { get; set; } = 0!;

        public int Progress { get; set; } = 0!;

        public int IsComment { get; set; } = 0!;

        public int IsCamera { get; set; } = 0!;

        public int PassageId { get; set; }
    }

    public class TaskVM
    {
        public int IdTask { get; set; }

        public string Description { get; set; } = string.Empty;
    }
}
