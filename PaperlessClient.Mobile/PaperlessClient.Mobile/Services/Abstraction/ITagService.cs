using PaperlessClient.Mobile.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PaperlessClient.Mobile.Services.Abstraction
{
    public interface ITagService
    {
        IObservable<List<Tag>> GetAndFetchAllTags();
        Task<List<Tag>> GetAllTags();
    }
}
