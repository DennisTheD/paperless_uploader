using PaperlessClient.Mobile.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PaperlessClient.Mobile.Services.Abstraction
{
    public interface IDocumentTypeService
    {
        IObservable<List<DocumentType>> GetAndFetchAllDocumentTypes();
        Task<List<DocumentType>> GetAllDocumentTypes();
    }
}
