namespace BqsClinoTag.ViewModel.Acknowledge
{
    public class AcknowledgeWebVM
    {
        public int AcknowledegeId { get; set; }

        public DateTime NotificationDate { get; set; }

        public DateTime AcknowledgeDate { get; set; }

        public string Lieu { get; set; } = string.Empty;

        public string Notification { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public string Contact { get; set; } = string.Empty;

        public string AcknowledgeBy { get; set; } = string.Empty;

        public int Duration { get; set; }  
    }
}
