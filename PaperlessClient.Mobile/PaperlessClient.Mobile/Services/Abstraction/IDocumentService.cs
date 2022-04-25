using PaperlessClient.Mobile.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PaperlessClient.Mobile.Services.Abstraction
{
    public interface IDocumentService
    {
        IObservable<ApiListResponse<Document>> GetAndFetchDocuments();
        Task<ApiListResponse<Document>> GetDocuments(int page);
        Task<ApiListResponse<Document>> SearchDocuments(string query, int page);
    }
}
