using Microsoft.Data.SqlClient;

namespace CureLogix.WebUI.Middlewares
{
    public class DbFailSafeMiddleware
    {
        private readonly RequestDelegate _next;

        public DbFailSafeMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                // Hatanın SQL veya Bağlantı hatası olup olmadığına bak
                if (IsDatabaseError(ex))
                {
                    // Çerezi Sil (Böylece bir sonraki istekte Auth hatası vermez)
                    context.Response.Cookies.Delete(".AspNetCore.Identity.Application");

                    // Ana sayfaya yönlendir (Artık anonim olarak açılacak)
                    context.Response.Redirect("/");
                    return;
                }

                // Başka bir hataysa normal akışına (Error Page) bırak
                throw;
            }
        }

        private bool IsDatabaseError(Exception ex)
        {
            // İç içe geçmiş hataları kontrol et
            var currentEx = ex;
            while (currentEx != null)
            {
                if (currentEx is SqlException || currentEx.Message.Contains("network-related") || currentEx.Message.Contains("transport-level"))
                {
                    return true;
                }
                currentEx = currentEx.InnerException;
            }
            return false;
        }
    }
}