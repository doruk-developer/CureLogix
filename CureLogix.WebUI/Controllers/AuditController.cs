using CureLogix.Business.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CureLogix.WebUI.Controllers
{
    [Authorize(Roles = "Admin")] // Sadece Admin görebilmeli
    public class AuditController : Controller
    {
        private readonly IAuditLogService _auditService;

        public AuditController(IAuditLogService auditService)
        {
            _auditService = auditService;
        }

        public IActionResult Index()
        {
            // DÜZELTME: x.ProcessDate yerine x.Date yaptık.
            var values = _auditService.TGetList().OrderByDescending(x => x.Date).ToList();
            return View(values);
        }
    }
}