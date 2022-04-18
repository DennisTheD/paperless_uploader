using PaperlessClient.Mobile.Models;
using PaperlessClient.Mobile.Services.Abstraction;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PaperlessClient.Mobile.ViewModels
{
    public class DocumentListViewModel : ListViewModelBase<Document>
    {
        public DocumentListViewModel(
            IDocumentService documentService
            , INotificationService notificationService) 
            : base(notificationService, documentService.GetDocuments, documentService.GetAndFetchDocuments, FilterDocuments)
        {
        }

        private static List<Document> FilterDocuments(string arg1, List<Document> arg2)
        {
            return arg2;
        }
    }
}
