using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;

namespace CureLogix.WebUI.Filters
{
	public class DemoRestrictionFilter : IActionFilter
	{
		private readonly IConfiguration _configuration;
		public DemoRestrictionFilter(IConfiguration configuration) { _configuration = configuration; }

		public void OnActionExecuting(ActionExecutingContext context)
		{
			// 🛡️ SADECE CANLI (SHOWCASE) MODUNDA ÇALIŞIR
			bool isShowcase = _configuration.GetValue<bool>("AppSettings:IsShowcaseMode");
			if (!isShowcase) return; // Yereldeyse sistemi hiç kısıtlama.

			string method = context.HttpContext.Request.Method;

			// Eğer işlem veri değiştirme (POST/PUT/DELETE) ise...
			if (method != "GET")
			{
				// Login/Logout işlemlerini muaf tut (Kapıdan geçebilsinler)
				var controller = context.RouteData.Values["controller"]?.ToString();
				if (controller == "Login") return;

				// 🛡️ BEYAZ LİSTE: Sadece 'admin@curelogix.com' yazma yetkisine sahiptir.
				var userEmail = context.HttpContext.User.FindFirstValue(ClaimTypes.Email);

				if (userEmail != "admin@curelogix.com")
				{
					// Admin dışındaki herkesi (Demo User dahil) engelle ve uyar.
					context.Result = new ContentResult()
					{
						Content = "<script>alert('⛔ VİTRİN KISITLAMASI:\\n\\nBu bir genel inceleme sürümüdür. Veri bütünlüğünü korumak için Süper Admin dışındaki hesaplar (Demo/User) sadece okuma yapabilir.'); history.go(-1);</script>",
						ContentType = "text/html"
					};
				}
			}
		}
		public void OnActionExecuted(ActionExecutedContext context) { }
	}
}