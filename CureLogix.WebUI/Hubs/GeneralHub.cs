using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;

namespace CureLogix.WebUI.Hubs
{
    public class GeneralHub : Hub
    {
        // 1. Önceki boş metot (İstersen silebilirsin, gerek yok)

        // 2. Konsey Odası Sohbet Metodu
        // DİKKAT: Bu metot "public class GeneralHub" parantezlerinin İÇİNDE olmalı.
        public async Task SendMessage(string user, string message)
        {
            // Mesajı al, formatla ve odadaki HERKESE (Caller dahil) gönder
            string formattedTime = DateTime.Now.ToString("HH:mm");

            // "ReceiveMessage" kanalından dinleyen herkese gönderiyoruz
            await Clients.All.SendAsync("ReceiveMessage", user, message, formattedTime);
        }
    }
}