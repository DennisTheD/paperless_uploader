using PaperlessClient.Mobile.Models;
using PaperlessClient.Mobile.Services.Abstraction;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace PaperlessClient.Mobile.Services
{
    public class FileUploadQueueService : IFileUploadQueueService
    {
        private ConcurrentQueue<FileUploadRequest> fileUploadRequests;

        public FileUploadQueueService()
        {
            fileUploadRequests = new ConcurrentQueue<FileUploadRequest>();
        }

        public void AddTask(FileUploadRequest fileUploadRequest)
            => fileUploadRequests.Enqueue(fileUploadRequest);

        public bool GetTask(out FileUploadRequest fileUploadRequest)
            => fileUploadRequests.TryDequeue(out fileUploadRequest);
    }
}
