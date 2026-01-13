using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CureLogix.WebUI.Controllers
{
    [Authorize]
    public class DocumentController : Controller
    {
        private readonly IWebHostEnvironment _env;

        public DocumentController(IWebHostEnvironment env)
        {
            _env = env;
        }

        public IActionResult Index()
        {
            // Klasördeki dosyaları listele (Simülasyon için veritabanı yerine klasöre bakıyoruz)
            var path = Path.Combine(_env.WebRootPath, "uploads", "documents");
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);

            var files = Directory.GetFiles(path).Select(Path.GetFileName).ToList();
            return View(files);
        }

        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return Json(new { success = false, message = "Dosya boş." });

            // 1. Dosya Yolu Hazırla
            var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "documents");
            if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

            // 2. Benzersiz İsim Oluştur (Çakışmayı önlemek için)
            // Örn: guid_Rontgen.jpg
            var uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            // 3. Dosyayı Kaydet (Stream)
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return Json(new { success = true, message = "Yüklendi" });
        }

        // Dosya Silme (Opsiyonel - Temizlik için)
        public IActionResult Delete(string fileName)
        {
            var path = Path.Combine(_env.WebRootPath, "uploads", "documents", fileName);
            if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
            }
            return RedirectToAction("Index");
        }
    }
}