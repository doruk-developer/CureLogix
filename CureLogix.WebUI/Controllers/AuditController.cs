using CureLogix.Business.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CureLogix.WebUI.Controllers
{
    [Authorize] // Sadece giriş yapanlar görebilir
    public class AuditController : Controller
    {
        private readonly IAuditLogService _auditService;

        public AuditController(IAuditLogService auditService)
        {
            _auditService = auditService;
        }

        public IActionResult Index()
        {
            // Logları tarihe göre tersten sırala (En yeni en üstte)
            var values = _auditService.TGetList().OrderByDescending(x => x.ProcessDate).ToList();
            return View(values);
        }
    }
}