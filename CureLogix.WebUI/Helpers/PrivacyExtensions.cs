namespace CureLogix.WebUI.Helpers
{
    public static class PrivacyExtensions
    {
        // E-Posta Maskeleme Metodu
        // Örnek: "kemal.sayar@gmail.com" -> "ke***ar@gmail.com"
        public static string MaskEmail(this string email)
        {
            if (string.IsNullOrEmpty(email)) return "";
            if (!email.Contains("@")) return email;

            var parts = email.Split('@');
            var name = parts[0];
            var domain = parts[1];

            if (name.Length <= 2)
            {
                return name.Substring(0, 1) + "***@" + domain;
            }
            else
            {
                var firstChars = name.Substring(0, 2);
                var lastChar = name.Substring(name.Length - 1, 1);
                var stars = new string('*', name.Length - 3);

                return $"{firstChars}{stars}{lastChar}@{domain}";
            }
        }

        // İsim Soyisim Maskeleme (İhtiyaç olursa)
        // Örnek: "Ali Vefa" -> "A** V***"
        public static string MaskName(this string fullName)
        {
            if (string.IsNullOrEmpty(fullName)) return "";

            var parts = fullName.Split(' ');
            var maskedParts = new List<string>();

            foreach (var part in parts)
            {
                if (part.Length > 1)
                    maskedParts.Add(part.Substring(0, 1) + new string('*', part.Length - 1));
                else
                    maskedParts.Add(part);
            }

            return string.Join(" ", maskedParts);
        }
    }
}