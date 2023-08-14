using BqsClinoTag.Grool;
using BqsClinoTag.Hubs;
using BqsClinoTag.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;

namespace BqsClinoTag.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        readonly CLINOTAGBQSContext _db = new CLINOTAGBQSContext();

        public HomeController(CLINOTAGBQSContext db, ILogger<HomeController> logger)
        {
            _db = db;
            _logger = logger;
        }

        public ActionResult Logout()
        {
            HttpContext.Session.Clear();
            Response.Cookies.Delete("Demo-ClinoTag-Access-Token");
            return RedirectToAction("Login", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(Utilisateur model, string returnUrl)
        {
            ViewData["returnUrl"] = returnUrl;
            try
            {
                var token = TokenJWT.creerTokenJWT(model.Login, model.Mdp, _db);
                if (token != null)
                {
                    Response.Cookies.Append(
                        "Demo-ClinoTag-Access-Token",
                        new JwtSecurityTokenHandler().WriteToken(token),
                        new CookieOptions() { HttpOnly = true, SameSite = SameSiteMode.Strict }
                    );
                }
                else
                {
                    if (returnUrl == null) returnUrl = Request.Path;
                    return RedirectToAction("Erreur", "Home", new { msgErr = "Connexion refusée, mot de passe et/ou login incorrects.", urlRetour = returnUrl });
                }
            }
            catch (Exception ex)
            {
                if (returnUrl == null) returnUrl = Request.Path;
                return RedirectToAction("Erreur", "Home", new { msgErr = ex.Message });
            }

            if (returnUrl != null) return Redirect(returnUrl);
            return RedirectToAction("Index", "Home");
        }

        public ActionResult Login(string returnUrl)
        {
            if (User.Identity.IsAuthenticated && returnUrl == null) return NotFound();
            //ViewBag.ReturnUrl = returnUrl;
            ViewData["returnUrl"] = returnUrl;
            var login = Request.Query.Where(q => q.Key == "login").FirstOrDefault();
            if (login.Key != null) ViewData["login"] = login.Value;
            else ViewData["login"] = "";
            return View();
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Interdit()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult Erreur(string msgErr, string urlRetour)
        {
            return View("Erreur", new string[] { msgErr, urlRetour });
        }
    }
}