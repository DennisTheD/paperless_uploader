using PaperlessClient.Mobile.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PaperlessClient.Mobile.Services.Abstraction
{
    public interface ICorrespondentService
    {
        IObservable<List<Correspondent>> GetAndFetchAllCorrespondents();
        Task<List<Correspondent>> GetAllCorrespondents();
    }
}
