using CureLogix.Entity.Concrete;
using System.Collections.Generic;

namespace CureLogix.Business.Abstract
{
    public interface IWasteReportService : IGenericService<WasteReport>
    {
        // Hastanelerdeki miadı dolanları atık statüsüne (Type: 2) geçirir
        void MoveExpiredToWaste();
    }
}