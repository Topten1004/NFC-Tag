using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BqsClinoTag.Models
{
    public class NotificationRequest
    {
        public List<Utilisation>? lUtilisation { get; set; }
        public Utilisateur? utilisateur { get; set; }
    }

    public class OubliMotDePasseRequest
    {
        public string Login { get; set; }
        public string MotDePasse { get; set; }
        public string EmailRecuperation { get; set; }
        public string IpEmetteur { get; set; }
    }

    public class MessageRequest
    {
        public string FromEmail { get; set; }
        public string IpEmetteur { get; set; }
        public string Message { get; set; }
    }

    public class WelcomeRequest
    {
        public string ToEmail { get; set; }
        public string UserName { get; set; }
    }
}
