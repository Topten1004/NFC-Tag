using BqsClinoTag.Models;

namespace BqsClinoTag.ViewModel.Inventory
{
    public class InventoryVM
    {
        public InventoryVM() {

            places = new List<InventoryItem>();

            tasks = new List<TaskVM>();

        }
        public List<InventoryItem> places { get; set; }

        public List<TaskVM> tasks { get; set; }

        public string comment { get; set; } = string.Empty!;

        public string photo { get; set; } = string.Empty!;
        public int passageId { get; set; } = 0;

        public int? flag { get; set; } = 0; // 0: none, 1: comment, 2: photo

        public string? lieu { get; set; } = string.Empty!;
    }

    public class InventoryItem
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
