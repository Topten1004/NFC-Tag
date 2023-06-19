using BqsClinoTag.ViewModel.Acknowledge;

namespace BqsClinoTag.Hubs
{
    public interface INotificationHub
    {
        Task NewNotification(NotificationVM model);

        Task SendNotification(AcknowledgeVM model);
    }
}
