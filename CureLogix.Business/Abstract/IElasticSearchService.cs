using CureLogix.Entity.DTOs.SearchDTOs;

namespace CureLogix.Business.Abstract
{
    public interface IElasticSearchService
    {
        Task CreateIndexAsync();
        Task DeleteIndexAsync();
        Task IndexMedicineAsync(MedicineSearchModel model);
        Task BulkIndexMedicinesAsync(List<MedicineSearchModel> medicines);
        Task<List<MedicineSearchModel>> SearchAsync(string keyword);
    }
}