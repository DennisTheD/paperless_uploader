using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PaperlessClient.Mobile.Services.Abstraction
{
    public interface INotificationService
    {
        Task Notify(string title, string message, string cancel = "OK");
        Task<bool> Notify(string title, string message, string accept, string cancel);
        Task NotifyIfInForeground(object sender, string title, string message, string cancel = "OK");
        Task<string> SelectActionIfInForeground(object sender, string title, string cancel = "Abbrechen", string destruction = null, params string[] options);
        Task<string> DisplayPromptIfInForeground(object sender, string title, string message);
    }
}
