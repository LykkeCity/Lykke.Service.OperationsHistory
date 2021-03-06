using System;
using System.Net.Http;

namespace Lykke.Service.OperationsHistory.Core.Settings.Repository
{
    public class SettingsRepositoryRemote<T>: ISettingsRepository<T>
    {
        private readonly string _url;
        public SettingsRepositoryRemote(string url)
        {
            _url = url;
        }
        public T Get()
        {
            var httpClient = new HttpClient { BaseAddress = new Uri(_url) };
            var settings = httpClient.GetStringAsync("").Result;

            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(settings);
        }
    }
}
