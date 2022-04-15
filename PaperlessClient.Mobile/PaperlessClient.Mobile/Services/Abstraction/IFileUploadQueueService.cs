using PaperlessClient.Mobile.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace PaperlessClient.Mobile.Services.Abstraction
{
    public interface IFileUploadQueueService
    {
        void AddTask(FileUploadRequest fileUploadRequest);
        bool GetTask(out FileUploadRequest fileUploadRequest);
    }
}
