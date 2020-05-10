using System.Threading.Tasks;

namespace MeetupSurvey.Core.Device
{
    public interface IWebNavigationService
    {
        Task NavigateAsync(string url);
        Task ComposeEmailAsync(string subject, string body, params string[] to);
    }
}