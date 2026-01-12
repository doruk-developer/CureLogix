using System;

namespace CureLogix.Entity.Concrete
{
    public class ErrorLog
    {
        public int Id { get; set; }
        public string ErrorMessage { get; set; }
        public string StackTrace { get; set; }
        public DateTime ErrorDate { get; set; }
        public string RequestPath { get; set; }
    }
}