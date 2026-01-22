using CureLogix.Business.Abstract;
using CureLogix.DataAccess.Abstract;
using CureLogix.Entity.Concrete;
using System.Linq;

namespace CureLogix.Business.Concrete
{
    public class AuditLogManager : GenericManager<AuditLog>, IAuditLogService
    {
        public AuditLogManager(IGenericRepository<AuditLog> repository) : base(repository) { }

        // EKLEME İŞLEMİNİ ÖZELLEŞTİRİYORUZ
        public new void TAdd(AuditLog t)
        {
            // 1. Yeni logu ekle
            base.TAdd(t);

            // 2. OTOMATİK TEMİZLİK (ROLLING LOGS)
            // Eğer kayıt sayısı 100'ü geçtiyse en eskileri sil
            // DÜZELTME: x.ProcessDate yerine x.Date yazdık
            var allLogs = _repository.GetList().OrderByDescending(x => x.Date).ToList();

            if (allLogs.Count > 100)
            {
                // En güncel 100 taneyi tut (Skip), geri kalanları (eski olanları) al
                var logsToDelete = allLogs.Skip(100).ToList();

                foreach (var oldLog in logsToDelete)
                {
                    _repository.Delete(oldLog);
                }
            }
        }
    }
}