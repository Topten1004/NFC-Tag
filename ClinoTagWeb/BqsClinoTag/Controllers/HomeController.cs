using BqsClinoTag.Grool;
using BqsClinoTag.Hubs;
using BqsClinoTag.Models;
using BqsClinoTag.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Threading.Tasks;

namespace BqsClinoTag.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly HttpClient _httpClient;
        private readonly CLINOTAGBQSContext _db;

        public HomeController(CLINOTAGBQSContext db, ILogger<HomeController> logger, HttpClient httpClient)
        {
            _db = db;
            _logger = logger;
            _httpClient = httpClient;
        }

        public IActionResult Logout()
        {
            ClearSessionAndCookies();
            return RedirectToAction(nameof(Login));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LoginAsync(Utilisateur model, string returnUrl)
        {
            ViewData["returnUrl"] = returnUrl;
            var pulsePointResponse = await AuthenticateWithPulsePointAsync(model);

            if (pulsePointResponse == null)
            {
                return RedirectToError("Third-party API login failed.", returnUrl);
            }

            return HandleLoginResponse(pulsePointResponse, model, returnUrl);
        }

        public IActionResult Login(string returnUrl)
        {
            if (User.Identity.IsAuthenticated && returnUrl == null)
            {
                return NotFound();
            }

            ViewData["returnUrl"] = returnUrl;
            ViewData["login"] = Request.Query["login"].FirstOrDefault() ?? "";
            return View();
        }

        public IActionResult Index() => View();

        public IActionResult Privacy() => View();

        public IActionResult Forbidden() => View();

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() => View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });

        public IActionResult ErrorPage(string msgErr, string urlRetour) => View("Erreur", new[] { msgErr, urlRetour });

        #region Helper Methods

        private async Task<PulsePointVM> AuthenticateWithPulsePointAsync(Utilisateur model)
        {
            var payload = new
            {
                username = model.Login,
                password = model.Mdp,
                projectId = 2
            };

            var jsonPayload = JsonConvert.SerializeObject(payload);
            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            try
            {
                var response = await _httpClient.PostAsync("https://api.pulsepoint.myrfid.nc/api/user/project/signin", content);

                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<PulsePointVM>(jsonResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error authenticating with PulsePoint");
            }

            return null;
        }

        private IActionResult HandleLoginResponse(PulsePointVM response, Utilisateur model, string returnUrl)
        {
            switch (response.Status)
            {
                case 1:
                    return HandleSuccessfulLogin(response, model, returnUrl);
                case -1:
                    return RedirectToError("Incorrect username, please signup on PulsePoint.", returnUrl);
                case 0:
                    return RedirectToError("Incorrect password, please check PulsePoint.", returnUrl);
                default:
                    return RedirectToError("Unexpected response from PulsePoint.", returnUrl);
            }
        }

        private IActionResult HandleSuccessfulLogin(PulsePointVM response, Utilisateur model, string returnUrl)
        {
            var token = TokenJWT.creerTokenJWT(response.User.Id.ToString(), model.Login, "ADMIN", _db);

            if (token == null)
            {
                return RedirectToError("Failed to generate JWT token.", returnUrl);
            }

            Response.Cookies.Append(
                "Demo-ClinoTag-Access-Token",
                new JwtSecurityTokenHandler().WriteToken(token),
                new CookieOptions { HttpOnly = true, SameSite = SameSiteMode.Strict }
            );

            return returnUrl != null ? Redirect(returnUrl) : RedirectToAction(nameof(Index));
        }

        private IActionResult RedirectToError(string errorMessage, string returnUrl)
        {
            return RedirectToAction(nameof(ErrorPage), new { msgErr = errorMessage, urlRetour = returnUrl ?? Request.Path });
        }

        private void ClearSessionAndCookies()
        {
            HttpContext.Session.Clear();
            Response.Cookies.Delete("Demo-ClinoTag-Access-Token");
        }

        #endregion
    }
}
