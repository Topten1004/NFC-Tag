using System.ComponentModel.DataAnnotations.Schema;

namespace BqsClinoTag.ViewModel.Acknowledge
{
    public class AcknowledgeVM
    {

        public AcknowledgeVM() {

            Notifications = new List<NotificationVM>();
        }
        public int NotificationCount { get; set; }

        public List<NotificationVM> Notifications { get; set; }
    }

    public class NotificationVM
    {
        public int IdAcknowledge { get; set; }
        public string Lieu { get; set; } = null!;

        public DateTime NotificationDate { get; set; }

        public string Notification { get; set; } = null!;

    }
}
