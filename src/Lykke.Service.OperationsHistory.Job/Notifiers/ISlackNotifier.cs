using System.Threading.Tasks;

namespace Lykke.Service.OperationsHistory.Job.Notifiers
{
    public interface ISlackNotifier
    {
        Task SendAsync(string type, string sender, string message);
    }

    public static class SlackNotifierExt
    {
        public static Task SendInfoAsync(this ISlackNotifier src, string message)
        {
            return src.SendAsync("Info", ":exclamation:", message);
        }

        public static Task SendErrorAsync(this ISlackNotifier src, string message)
        {
            return src.SendAsync("Errors", ":exclamation:", message);
        }

        public static Task SendWarningAsync(this ISlackNotifier src, string message)
        {
            return src.SendAsync("Warning", ":warning:", message);
        }
    }
}