using System;

namespace CureLogix.Entity.Concrete
{
    public class AuditLog
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string ActionType { get; set; }
        public string ControllerName { get; set; }
        public string Description { get; set; }
        public DateTime ProcessDate { get; set; }
        public string IpAddress { get; set; }
    }
}