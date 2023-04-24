namespace BqsClinoTag.ViewModel.Activity
{
    public class ActivityVM
    {
        public ActivityVM() {
            datas = new List<ActivityItem>();
        }

        public List<ActivityItem> datas;

        public string comment { get; set; } = string.Empty!;

        public int passageId { get; set; } = 0;
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
}
