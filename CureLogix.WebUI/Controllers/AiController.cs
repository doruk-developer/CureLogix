using CureLogix.Business.Abstract;
using Microsoft.AspNetCore.Mvc;

namespace CureLogix.WebUI.Controllers
{
    public class AiController : Controller
    {
        private readonly IAiForecastService _aiService;

        public AiController(IAiForecastService aiService)
        {
            _aiService = aiService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Predict(string city, string medicine)
        {
            // Gelecek 6 ayın tahminini yapıp JSON dönelim
            var predictions = new List<object>();
            var currentMonth = DateTime.Now.Month;

            for (int i = 1; i <= 6; i++)
            {
                int targetMonth = (currentMonth + i) % 12;
                if (targetMonth == 0) targetMonth = 12;

                var result = _aiService.PredictDemand(city, medicine, targetMonth);

                predictions.Add(new
                {
                    Month = System.Globalization.DateTimeFormatInfo.CurrentInfo.GetMonthName(targetMonth),
                    Value = Math.Round(result.PredictedDemand)
                });
            }

            return Json(predictions);
        }
    }
}