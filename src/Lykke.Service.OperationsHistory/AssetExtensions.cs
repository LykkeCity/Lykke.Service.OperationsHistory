using Lykke.Service.Assets.Client.Models;

namespace Lykke.Service.OperationsHistory
{
    public static class AssetExtensions
    {
        public static int GetDisplayAccuracy(this Asset asset)
        {
            return asset.DisplayAccuracy ?? asset.Accuracy;
        }
    }
}