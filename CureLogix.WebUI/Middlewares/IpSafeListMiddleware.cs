using System.Net;

namespace CureLogix.WebUI.Middlewares
{
    public class IpSafeListMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly string[] _safeList;

        public IpSafeListMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            _next = next;
            // Ayar dosyasından izinli IP'leri çekiyoruz
            _safeList = configuration.GetSection("IpSafeList").Get<string[]>();
        }

        public async Task Invoke(HttpContext context)
        {
            // 1. İstemcinin IP adresini al
            var remoteIp = context.Connection.RemoteIpAddress;

            // Not: Localhost bazen null gelebilir veya ::1 olabilir.
            string clientIp = remoteIp?.ToString() ?? "::1";

            // 2. Kontrol Et: Listede var mı?
            bool isAllowed = _safeList.Contains(clientIp);

            // Geliştirici dostu: Eğer Localhost ise her zaman izin ver (Test yaparken kendimizi kilitlemeyelim)
            if (clientIp == "::1" || clientIp == "127.0.0.1")
            {
                isAllowed = true;
            }

            if (!isAllowed)
            {
                // 3. YASAKLA: Listede yoksa 403 hatası fırlat ve işlemi kes.
                context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                await context.Response.WriteAsync($"ERISIM ENGELLENDI! IP Adresiniz ({clientIp}) yetkili hastane aginda degil.");
                return; // Buradan sonrasına (Controller'a) gitmez.
            }

            // 4. İZİN VER: Bir sonraki aşamaya geç
            await _next(context);
        }
    }
}