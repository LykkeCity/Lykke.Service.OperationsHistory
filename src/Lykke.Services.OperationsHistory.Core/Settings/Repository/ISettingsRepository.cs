using System.Runtime.InteropServices.ComTypes;

namespace Lykke.Services.OperationsHistory.Core.Settings.Repository
{
    public interface ISettingsRepository<out T>
    {
        T Get();
    }
}