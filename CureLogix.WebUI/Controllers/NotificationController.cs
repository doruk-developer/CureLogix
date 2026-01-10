using CureLogix.WebUI.Hubs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace CureLogix.WebUI.Controllers
{
    public class NotificationController : Controller
    {
        private readonly IHubContext<GeneralHub> _hubContext;

        // HubContext'i enjekte ediyoruz ki buradan tüm kullanıcılara mesaj atabilelim
        public NotificationController(IHubContext<GeneralHub> hubContext)
        {
            _hubContext = hubContext;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        // Alarmı Tetikleyen Metot
        [HttpPost]
        public async Task<IActionResult> TriggerAlarm(string hospital, string medicine)
        {
            string message = $"🚨 ACİL DURUM: {hospital} tarafında '{medicine}' stoğu TÜKENDİ!";

            // Tüm bağlı kullanıcılara (All) 'ReceiveCriticalStock' adında bir sinyal gönder
            await _hubContext.Clients.All.SendAsync("ReceiveCriticalStock", message);

            return Ok(); // Başarılı (Sayfa yenilenmez, Ajax ile çağıracağız)
        }
    }
}