using CureLogix.Business.Abstract;
using CureLogix.Entity.DTOs.SearchDTOs;
using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.Core.Bulk;
using Elastic.Clients.Elasticsearch.QueryDsl;
using Microsoft.Extensions.Configuration;

namespace CureLogix.Business.Concrete
{
    public class ElasticSearchManager : IElasticSearchService
    {
        private readonly ElasticsearchClient _client;
        private const string IndexName = "medicines";

        public ElasticSearchManager(IConfiguration configuration)
        {
            var url = configuration["ConnectionStrings:ElasticConnection"] ?? "http://localhost:9200";
            // Alan isimlerini C# tarafındaki gibi (Name, Id) koruması için
            var settings = new ElasticsearchClientSettings(new Uri(url))
                .DefaultIndex(IndexName)
                .DefaultFieldNameInferrer(p => p);
            _client = new ElasticsearchClient(settings);
        }

        public async Task CreateIndexAsync()
        {
            var exists = await _client.Indices.ExistsAsync(IndexName);
            if (!exists.Exists) await _client.Indices.CreateAsync(IndexName);
        }

        public async Task DeleteIndexAsync()
        {
            // İndeksi zorla siliyoruz ki eski bozuk mappingler temizlensin
            await _client.Indices.DeleteAsync(IndexName);
        }

        public async Task IndexMedicineAsync(MedicineSearchModel model)
        {
            await _client.IndexAsync(model, i => i.Id(model.Id.ToString()));
        }

        public async Task BulkIndexMedicinesAsync(List<MedicineSearchModel> medicines)
        {
            // 🚀 Her dökümanı kendi ID'siyle (Id.ToString) zorla kaydediyoruz.
            // Artık 'count: 1' gelme ihtimali matematiksel olarak bitti.
            var response = await _client.BulkAsync(b => b
                .Index(IndexName)
                .IndexMany(medicines, (descriptor, s) => descriptor.Id(s.Id.ToString()))
            );

            if (!response.IsSuccess())
            {
                throw new Exception($"Elastic hatası: {response.DebugInformation}");
            }

            await _client.Indices.RefreshAsync(IndexName);
        }

        public async Task<List<MedicineSearchModel>> SearchAsync(string keyword)
        {
            // Arama mantığını da basitleştirelim (Önce veriyi görelim)
            var response = await _client.SearchAsync<MedicineSearchModel>(s => s
                .Indices(IndexName)
                .From(0).Size(50)
                .Query(q => q
                    .Wildcard(w => w.Field("Name").Value($"*{keyword}*")) // Joker arama
                )
            );
            return response.IsValidResponse ? response.Documents.ToList() : new List<MedicineSearchModel>();
        }
    }
}