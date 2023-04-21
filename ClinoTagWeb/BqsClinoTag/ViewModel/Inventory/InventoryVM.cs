using BqsClinoTag.Models;

namespace BqsClinoTag.ViewModel.Inventory
{
    public class InventoryVM
    {
        public InventoryVM() {

            places = new List<Lieu>();

            tasks = new List<TaskVM>();

        }
        public List<Lieu> places { get; set; }

        public List<TaskVM> tasks { get; set; }
    }

    public class TaskVM
    {
        public int IdTask { get; set; }

        public string Description { get; set; } = string.Empty;

    }
}
