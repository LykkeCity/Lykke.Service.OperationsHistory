namespace Lykke.Service.OperationsHistory.Core.Settings.Repository
{
    public interface ISettingsRepository<out T>
    {
        T Get();
    }
}