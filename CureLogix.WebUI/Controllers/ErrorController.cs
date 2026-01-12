using CureLogix.DataAccess.Concrete;
using CureLogix.Entity.Concrete;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace CureLogix.WebUI.Controllers
{
    [AllowAnonymous] // Giriş yapmamış kullanıcı da hatayı görebilmeli
    public class ErrorController : Controller
    {
        private readonly CureLogixContext _context;

        public ErrorController(CureLogixContext context)
        {
            _context = context;
        }

        // 404 - Sayfa Bulunamadı
        public IActionResult Page404()
        {
            return View();
        }

        // 500 - Sunucu Hatası (Kod Patlaması)
        public IActionResult Page500()
        {
            // Hata detaylarını yakala
            var exceptionHandlerPathFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();

            if (exceptionHandlerPathFeature != null)
            {
                // Veritabanına Logla
                var errorLog = new ErrorLog
                {
                    ErrorMessage = exceptionHandlerPathFeature.Error.Message,
                    StackTrace = exceptionHandlerPathFeature.Error.StackTrace,
                    RequestPath = exceptionHandlerPathFeature.Path,
                    ErrorDate = DateTime.Now
                };

                _context.ErrorLogs.Add(errorLog);
                _context.SaveChanges();
            }

            return View();
        }
    }
}