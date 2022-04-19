using PaperlessClient.Mobile.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PaperlessClient.Mobile.Services.Abstraction
{
    public interface IDocumentService
    {
        bool MoreResultsAvailable { get; }
        IObservable<List<Document>> GetAndFetchDocuments();
        Task<List<Document>> GetDocuments(int page);
    }
}
