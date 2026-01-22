using CureLogix.Business.Abstract;
using CureLogix.Entity.Concrete;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CureLogix.WebUI.Filters
{
    public class AuditLogAttribute : ActionFilterAttribute
    {
        private readonly string _activity;

        public AuditLogAttribute(string activity)
        {
            _activity = activity;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            // --- FAIL-SAFE KORUMA ---
            // Veritabanı yoksa loglama yapma ama programı da durdurma.
            try
            {
                var service = context.HttpContext.RequestServices.GetService<IAuditLogService>();
                var username = context.HttpContext.User.Identity?.Name ?? "Anonim";
                var ip = context.HttpContext.Connection.RemoteIpAddress?.ToString();

                var log = new AuditLog
                {
                    Activity = _activity,
                    UserName = username,
                    Date = DateTime.Now,
                    IpAddress = ip
                };

                service.TAdd(log);
            }
            catch
            {
                // DB Bağlantısı yok.
                // HİÇBİR ŞEY YAPMA. Hatayı yut.
                // Akış bozulmasın, sayfa açılsın.
            }

            base.OnActionExecuting(context);
        }
    }
}