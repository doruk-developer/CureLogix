using CureLogix.Business.Abstract;
using CureLogix.Entity.Concrete;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CureLogix.WebUI.Filters
{
    public class AuditLogAttribute : ActionFilterAttribute
    {
        private readonly string _description;

        // Kullanıcı [AuditLog("İlaç Silindi")] dediğinde bu çalışır
        public AuditLogAttribute(string description)
        {
            _description = description;
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            // Servisi DI konteynerından çekiyoruz (Filter içinde constructor injection zordur)
            var auditService = context.HttpContext.RequestServices.GetService<IAuditLogService>();

            var user = context.HttpContext.User.Identity?.Name ?? "Anonim";
            var ip = context.HttpContext.Connection.RemoteIpAddress?.ToString();
            var controller = context.RouteData.Values["controller"]?.ToString();
            var action = context.RouteData.Values["action"]?.ToString();

            // Ekstra verileri (ID gibi) yakalamak için
            var queryId = context.HttpContext.Request.Query["id"].ToString();
            var detailMsg = string.IsNullOrEmpty(queryId) ? _description : $"{_description} (Kayıt ID: {queryId})";

            var log = new AuditLog
            {
                UserName = user,
                ControllerName = controller,
                ActionType = action,
                Description = detailMsg,
                IpAddress = ip,
                ProcessDate = DateTime.Now
            };

            auditService.TAdd(log);
            base.OnActionExecuted(context);
        }
    }
}