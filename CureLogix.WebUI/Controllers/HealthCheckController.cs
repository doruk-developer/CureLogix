using CureLogix.DataAccess.Concrete;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CureLogix.WebUI.Controllers
{
    [Authorize]
    public class HealthCheckController : Controller
    {
        private readonly CureLogixContext _context;
        private readonly IConfiguration _configuration; // <-- BURAYI EKLE

        // Constructor'a IConfiguration ekliyoruz
        public HealthCheckController(CureLogixContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpGet]
        public async Task<IActionResult> DbStatus()
        {

            var isDemo = _configuration.GetValue<bool>("AppSettings:DemoMode");

            if (isDemo) return Json(new { status = "Demo" });

            try
            {
                bool isAlive = await _context.Database.CanConnectAsync();
                return Json(new
                {
                    status = isAlive ? "Online" : "Offline",
                    timestamp = DateTime.Now.ToString("HH:mm:ss")
                });
            }
            catch
            {
                return Json(new { status = "Offline" });
            }
        }
    }
}