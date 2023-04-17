using BqsClinoTag.Grool;
using BqsClinoTag.Models;
using BqsClinoTag.Settings;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using MimeKit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace BqsClinoTag.Services
{
    public class MailService : IMailService
    {
        private readonly MailSettings _mailSettings;
        public MailService(IOptions<MailSettings> mailSettings)
        {
            _mailSettings = mailSettings.Value;
        }

        private async Task SendEmailAsync(MimeMessage email)
        {
            using var smtp = new SmtpClient();
#if DEBUG
            DisableCertificateValidation();
#endif
            smtp.Connect(_mailSettings.Smtp, _mailSettings.PortSmtp, SecureSocketOptions.StartTls);
            smtp.Authenticate(_mailSettings.Mail, _mailSettings.Password);

            await smtp.SendAsync(email);
            smtp.Disconnect(true);
        }

        static void DisableCertificateValidation()
        {
            // Disabling certificate validation can expose you to a man-in-the-middle attack
            // which may allow your encrypted message to be read by an attacker
            // https://stackoverflow.com/a/14907718/740639
            ServicePointManager.ServerCertificateValidationCallback =
                delegate (
                    object s,
                    X509Certificate certificate,
                    X509Chain chain,
                    SslPolicyErrors sslPolicyErrors
                ) {
                    return true;
                };
        }

        public async Task EnvoiEmailNotificationAsync(NotificationRequest request)
        {
            string FilePath = Directory.GetCurrentDirectory() + @"\wwwroot\mail\MessageEmail.html";
            StreamReader str = new StreamReader(FilePath);
            string MailText = str.ReadToEnd();
            str.Close();

            var email = new MimeMessage();
            email.Headers.Add(HeaderId.From, _mailSettings.Mail);
            email.From[0].Name = _mailSettings.DisplayName;
            emailMultiDestinataires(ref email, request.utilisateur.Email + ";" + _mailSettings.Bcc);
            //email.Bcc.Add(MailboxAddress.Parse());
            email.Subject = "BQS - Notification d'utilisation expirée de matériel à " + DateTime.Now.ToString("g");

            MailText = MailText.Replace("[titre]", "Notification d'utilisation expirée de matériel.");

            String msg = "<table><tr><th>Materiel</th><th>Date/Heure début</th><th>Date/Heure expiration</th><th>Agent<th></tr>";
            foreach(Utilisation u in request.lUtilisation)
            {
                msg += $"<tr><td>{u.IdMaterielNavigation.Nom}</td><td>{u.DhDebut}</td><td>{u.DhDebut.AddMinutes(u.IdMaterielNavigation.Expiration)}</td><td>{u.IdAgentNavigation.Nom}</td></tr>";
            }
            msg += "</table>";

            MailText = MailText.Replace("[message]", msg);

            var plain = new TextPart(MimeKit.Text.TextFormat.Plain) { Text = $"BQS - Notification - https://bqs-clinotag.square.nc" };
            var html = new TextPart(MimeKit.Text.TextFormat.Html) { Text = MailText };

            var alternative = new MultipartAlternative();
            alternative.Add(plain);
            alternative.Add(html);

            var multipart = new Multipart("mixed");
            multipart.Add(alternative);

            email.Body = multipart;

            await SendEmailAsync(email);
        }

        //public async Task EnvoiEmailReleveAsync(ReleveRequest request)
        //{
        //    string FilePath;
        //    FilePath = Directory.GetCurrentDirectory() + @"\wwwroot\mail\ReleveEmail.html";
        //    StreamReader str = new StreamReader(FilePath);
        //    string MailText = str.ReadToEnd();
        //    str.Close();

        //    MailText = MailText.Replace("[magasin]", request.unMagasin.NomMagasin);
        //    MailText = MailText.Replace("[date]", DateTime.Now.ToShortDateString());

        //    if (request.lFacture.Any(f => f.MsgReleve != null))
        //        MailText = MailText.Replace("[message_releve]", "<p class='msg-totem'><b>Message de Totem :</b><br>" + request.lFacture.First(f => f.MsgReleve != null).MsgReleve + "</p>");
        //    else MailText = MailText.Replace("[message_releve]", "");

        //    String sReleve = "";
        //    int montant = 0;
        //    foreach(Facture uneF in request.lFacture.OrderByDescending(f => f.DhCreation))
        //    {
        //        string mpetdate = "";
        //        foreach (Mpf unMPDate in uneF.Mpfs)
        //            mpetdate += $"{unMPDate.IdModePaiementNavigation.Libelle} le {unMPDate.DhPaiement.ToShortDateString()}<br>";

        //        sReleve += $"<tr><td>{uneF.NumFacture}</td><td>{uneF.DhCreation.ToShortDateString()}</td><td>" +
        //            $"{uneF.Montant.ToString("### ### ###")}</td><td>{uneF.DateDebut.ToShortDateString()} au {uneF.DateFin.ToShortDateString()}" +
        //            $"</td><td>{(uneF.Montant - uneF.Mpfs.Sum(m => m.Montant)).ToString("### ### ##0")}" +
        //            $"</td><td>{mpetdate}</td></tr>";
        //        montant += (uneF.Montant - uneF.Mpfs.Sum(m => m.Montant));
        //    }

        //    MailText = MailText.Replace("[tableau_releve]", sReleve);
        //    MailText = MailText.Replace("[montant_total]", montant.ToString("### ### ##0"));

        //    var email = new MimeMessage();
        //    //email.Sender = MailboxAddress.Parse(_mailSettings.From);
        //    email.Headers.Add(HeaderId.From, _mailSettings.From);
        //    email.From[0].Name = _mailSettings.DisplayName;
        //    emailMultiDestinataires(ref email, request.unMagasin.IdResponsableNavigation.Email);
        //    email.Bcc.Add(MailboxAddress.Parse(_mailSettings.Bcc));
        //    email.Subject = $"Totem Distribution - Relevé du {DateTime.Now.ToShortDateString()} pour {request.unMagasin.NomMagasin}";

        //    var plain = new TextPart(MimeKit.Text.TextFormat.Plain) { Text = $"Totem Distribution - Relevé du {DateTime.Now.ToShortDateString()} - https://bqs-clinotag.square.nc" };
        //    var html = new TextPart(MimeKit.Text.TextFormat.Html) { Text = MailText };

        //    var alternative = new MultipartAlternative();
        //    alternative.Add(plain);
        //    alternative.Add(html);

        //    var multipart = new Multipart("mixed");
        //    multipart.Add(alternative);
    
        //    email.Body = multipart;

        //    await SendEmailAsync(email);
        //}

        //public async Task EnvoiEmailFactureAsync(FactureRequest request)
        //{
        //    string FilePath;
        //    FilePath = Directory.GetCurrentDirectory() + @"\wwwroot\mail\FactureEmail.html";
        //    StreamReader str = new StreamReader(FilePath);
        //    string MailText = str.ReadToEnd();
        //    str.Close();

        //    MailText = MailText.Replace("[magasin]", request.uneFacture.IdMagasinNavigation.NomMagasin);
        //    MailText = MailText.Replace("[date]", request.uneFacture.DhCreation.ToShortDateString());
        //    MailText = MailText.Replace("[num]", request.uneFacture.NumFacture);
        //    MailText = MailText.Replace("[montant]", request.uneFacture.Montant.ToString("### ### ###"));

        //    var email = new MimeMessage();
        //    //email.Sender = MailboxAddress.Parse(_mailSettings.From);
        //    email.Headers.Add(HeaderId.From, _mailSettings.From);
        //    email.From[0].Name = _mailSettings.DisplayName;
        //    emailMultiDestinataires(ref email, request.uneFacture.IdMagasinNavigation.IdResponsableNavigation.Email);
        //    email.Bcc.Add(MailboxAddress.Parse(_mailSettings.Bcc));
        //    email.Subject = $"Totem Distribution - Facture {request.uneFacture.NumFacture} du {request.uneFacture.DhCreation.ToShortDateString()}";

        //    // create an image attachment for the file located at path
        //    var attachment = new MimePart("application/pdf")
        //    {
        //        Content = new MimeContent(new MemoryStream(request.uneFacture.Fichier), ContentEncoding.Default),
        //        ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
        //        ContentTransferEncoding = ContentEncoding.Base64,
        //        FileName = $"totem.facture.{request.uneFacture.NumFacture}.pdf"
        //    };

        //    var plain = new TextPart(MimeKit.Text.TextFormat.Plain) { Text = $"Totem Distribution - Facture {request.uneFacture.NumFacture} - https://bqs-clinotag.square.nc" };
        //    var html = new TextPart(MimeKit.Text.TextFormat.Html) { Text = MailText };

        //    var alternative = new MultipartAlternative();
        //    alternative.Add(plain);
        //    alternative.Add(html);

        //    var multipart = new Multipart("mixed");
        //    multipart.Add(alternative);
        //    multipart.Add(attachment);

        //    email.Body = multipart;

        //    await SendEmailAsync(email);
        //}

        public async Task EmailMessage(MessageRequest request)
        {
            string FilePath = Directory.GetCurrentDirectory() + @"\wwwroot\mail\MessageEmail.html";
            StreamReader str = new StreamReader(FilePath);
            string MailText = str.ReadToEnd();
            str.Close();

            var email = new MimeMessage();
            email.Headers.Add(HeaderId.From, _mailSettings.Mail);
            email.From[0].Name = _mailSettings.DisplayName;
            emailMultiDestinataires(ref email, _mailSettings.Bcc);
            //email.Bcc.Add(MailboxAddress.Parse());
            email.Subject = $"Message depuis bqs-clinotag.square.nc le {DateTime.Now.ToShortDateString()} à {DateTime.Now.ToLongTimeString()}";

            MailText = MailText.Replace("[titre]", "Message depuis bqs-clinotag.square.nc");
            MailText = MailText.Replace("[message]", request.FromEmail + "<br>" + request.IpEmetteur + "<br>" + request.Message);

            var plain = new TextPart(MimeKit.Text.TextFormat.Plain) { Text = $"Totem Distribution - Message - https://bqs-clinotag.square.nc" };
            var html = new TextPart(MimeKit.Text.TextFormat.Html) { Text = MailText };

            var alternative = new MultipartAlternative();
            alternative.Add(plain);
            alternative.Add(html);

            var multipart = new Multipart("mixed");
            multipart.Add(alternative);

            email.Body = multipart;

            await SendEmailAsync(email);
        }

        //public async Task EmailRetour(RetourRequest request)
        //{
        //    string FilePath = Directory.GetCurrentDirectory() + @"\wwwroot\mail\RetourEmail.html";
        //    StreamReader str = new StreamReader(FilePath);
        //    string MailText = str.ReadToEnd();
        //    str.Close();

        //    var email = new MimeMessage();
        //    email.Headers.Add(HeaderId.From, _mailSettings.From);
        //    email.From[0].Name = _mailSettings.DisplayName;
        //    emailMultiDestinataires(ref email, request.ToEmail);
        //    email.Bcc.Add(MailboxAddress.Parse(_mailSettings.Bcc));
        //    email.Subject = $"Totem - Retour {request.NomMagasin} du {DateTime.Now.ToShortDateString()} à {DateTime.Now.ToShortTimeString()}";

        //    MailText = MailText.Replace("[magasin]", request.NomMagasin);
        //    MailText = MailText.Replace("[message]", request.Message); //.Replace("[email]", request.ToEmail);

        //    var plain = new TextPart(MimeKit.Text.TextFormat.Plain) { Text = $"Totem Distribution - Retour {request.NomMagasin} - https://bqs-clinotag.square.nc" };
        //    var html = new TextPart(MimeKit.Text.TextFormat.Html) { Text = MailText };

        //    var alternative = new MultipartAlternative();
        //    alternative.Add(plain);
        //    alternative.Add(html);

        //    var multipart = new Multipart("mixed");
        //    multipart.Add(alternative);

        //    email.Body = multipart;

        //    await SendEmailAsync(email);
        //}

        private void emailMultiDestinataires(ref MimeMessage email, string toEmail)
        {
            string[] adresses = toEmail.Split(';').Select(sValue => sValue.Trim()).ToArray();
            foreach(String s in adresses) email.To.Add(MailboxAddress.Parse(s));            
        }

        //public async Task EmailLivraison(LivraisonRequest request)
        //{
        //    string FilePath = Directory.GetCurrentDirectory() + @"\wwwroot\mail\LivraisonEmail.html";
        //    StreamReader str = new StreamReader(FilePath);
        //    string MailText = str.ReadToEnd();
        //    str.Close();

        //    var email = new MimeMessage();
        //    //email.Sender = MailboxAddress.Parse(_mailSettings.Mail);
        //    email.Headers.Add(HeaderId.From, _mailSettings.From);
        //    email.From[0].Name = _mailSettings.DisplayName;
        //    emailMultiDestinataires(ref email, request.ToEmail); //"grool.sarl@gmail.com");  //
        //    email.Bcc.Add(MailboxAddress.Parse(_mailSettings.Bcc));
        //    email.Subject = $"Totem - Livraison {request.NomMagasin} du {DateTime.Now.ToShortDateString()} à {DateTime.Now.ToShortTimeString()}";

        //    MailText = MailText.Replace("[magasin]", request.NomMagasin);
        //    MailText = MailText.Replace("[message]", request.Message);

        //    var plain = new TextPart(MimeKit.Text.TextFormat.Plain) { Text = $"Totem Distribution - Livraison {request.NomMagasin} - https://bqs-clinotag.square.nc" };
        //    var html = new TextPart(MimeKit.Text.TextFormat.Html) { Text = MailText };

        //    var alternative = new MultipartAlternative();
        //    alternative.Add(plain);
        //    alternative.Add(html);

        //    var multipart = new Multipart("mixed");
        //    multipart.Add(alternative);

        //    email.Body = multipart;

        //    await SendEmailAsync(email);
        //}

        public async Task SendWelcomeEmailAsync(WelcomeRequest request)
        {
            string FilePath = Directory.GetCurrentDirectory() + @"\wwwroot\mail\WelcomeEmail.html";
            StreamReader str = new StreamReader(FilePath);
            string MailText = str.ReadToEnd();
            str.Close();
            MailText = MailText.Replace("[username]", request.UserName).Replace("[email]", request.ToEmail);
            var email = new MimeMessage();
            email.Sender = MailboxAddress.Parse(_mailSettings.Mail);
            emailMultiDestinataires(ref email, request.ToEmail);
            email.Subject = $"Welcome {request.UserName}";
            var builder = new BodyBuilder();
            builder.HtmlBody = MailText;
            email.Body = builder.ToMessageBody();

            await SendEmailAsync(email);
        }

        public async Task OubliMotDePasse(OubliMotDePasseRequest request, HttpContext httpContext)
        {
            string FilePath = Directory.GetCurrentDirectory() + @"\wwwroot\mail\MessageEmail.html";
            StreamReader str = new StreamReader(FilePath);
            string MailText = str.ReadToEnd();
            str.Close();

            var email = new MimeMessage();
            email.Headers.Add(HeaderId.From, _mailSettings.Mail);
            email.From[0].Name = _mailSettings.DisplayName;
            emailMultiDestinataires(ref email, request.EmailRecuperation);
            email.Bcc.Add(MailboxAddress.Parse(_mailSettings.Bcc));

            email.Bcc.Add(MailboxAddress.Parse(_mailSettings.Bcc));
            email.Subject = $"Totem - Récupération des identifiants le {DateTime.Now.ToShortDateString()} à {DateTime.Now.ToLongTimeString()}";

            String urlLogin = Network.GetBaseUrl(httpContext) + "/Account/Login?login=" + request.Login;

            var builder = new BodyBuilder();
            MailText = MailText.Replace("[titre]", "Récupération des identifiants");
            MailText = MailText.Replace("[message]", 
                "Vous avez demandé une récupération des identifiants pour vous connecter sur Totem<br><br>Identifiant : <b>" + 
                request.Login + "</b><br>Mot de passe : <b>" + request.MotDePasse + "</b><br><br>" +
                "<b>Vous pouvez vous connecter à partir de ce lien : <a href='" + urlLogin + "'>https://bqs-clinotag.square.nc</a></b>");

            var plain = new TextPart(MimeKit.Text.TextFormat.Plain) { Text = $"Totem Distribution - Récupération des identifiants - https://bqs-clinotag.square.nc" };
            var html = new TextPart(MimeKit.Text.TextFormat.Html) { Text = MailText };

            var alternative = new MultipartAlternative();
            alternative.Add(plain);
            alternative.Add(html);

            var multipart = new Multipart("mixed");
            multipart.Add(alternative);

            email.Body = multipart;

            await SendEmailAsync(email);
        }

        //public async Task SendEmailAsync(MailRequest mailRequest)
        //{
        //    var email = new MimeMessage();
        //    email.Sender = MailboxAddress.Parse(_mailSettings.Mail);
        //    email.To.Add(MailboxAddress.Parse(mailRequest.ToEmail));
        //    email.Subject = mailRequest.Subject;
        //    var builder = new BodyBuilder();
        //    if (mailRequest.Attachments != null)
        //    {
        //        byte[] fileBytes;
        //        foreach (var file in mailRequest.Attachments)
        //        {
        //            if (file.Length > 0)
        //            {
        //                using (var ms = new MemoryStream())
        //                {
        //                    file.CopyTo(ms);
        //                    fileBytes = ms.ToArray();
        //                }
        //                builder.Attachments.Add(file.FileName, fileBytes, ContentType.Parse(file.ContentType));
        //            }
        //        }
        //    }
        //    builder.HtmlBody = mailRequest.Body;
        //    email.Body = builder.ToMessageBody();
        //    await SendEmailAsync(email);
        //}
    }
}
