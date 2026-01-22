using System;
using System.ComponentModel.DataAnnotations;

namespace CureLogix.Entity.Concrete
{
    public class AuditLog
    {
        [Key]
        public int Id { get; set; }

        public string Activity { get; set; }  // Attribute içindeki .Activity buraya denk gelecek

        public DateTime Date { get; set; }    // Attribute içindeki .Date buraya denk gelecek

        public string UserName { get; set; }

        public string IpAddress { get; set; }
    }
}