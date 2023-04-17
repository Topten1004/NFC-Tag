using BqsClinoTag.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BqsClinoTag.Services
{
    public interface IMailService
    {
        Task EnvoiEmailNotificationAsync(NotificationRequest request);
        Task OubliMotDePasse(OubliMotDePasseRequest request, HttpContext httpContext);
        Task SendWelcomeEmailAsync(WelcomeRequest request);
        Task EmailMessage(MessageRequest request);
    }
}
