// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace Lykke.Service.OperationsHistory.AutorestClient
{
    using Models;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Extension methods for OperationsHistoryAPI.
    /// </summary>
    public static partial class OperationsHistoryAPIExtensions
    {
            /// <summary>
            /// Checks service is alive
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            public static IsAliveResponse IsAlive(this IOperationsHistoryAPI operations)
            {
                return operations.IsAliveAsync().GetAwaiter().GetResult();
            }

            /// <summary>
            /// Checks service is alive
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='cancellationToken'>
            /// The cancellation token.
            /// </param>
            public static async Task<IsAliveResponse> IsAliveAsync(this IOperationsHistoryAPI operations, CancellationToken cancellationToken = default(CancellationToken))
            {
                using (var _result = await operations.IsAliveWithHttpMessagesAsync(null, cancellationToken).ConfigureAwait(false))
                {
                    return _result.Body;
                }
            }

            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='clientId'>
            /// </param>
            /// <param name='take'>
            /// </param>
            /// <param name='skip'>
            /// </param>
            /// <param name='operationType'>
            /// </param>
            /// <param name='assetId'>
            /// </param>
            public static object GetByClientId(this IOperationsHistoryAPI operations, string clientId, int take, int skip, string operationType = default(string), string assetId = default(string))
            {
                return operations.GetByClientIdAsync(clientId, take, skip, operationType, assetId).GetAwaiter().GetResult();
            }

            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='clientId'>
            /// </param>
            /// <param name='take'>
            /// </param>
            /// <param name='skip'>
            /// </param>
            /// <param name='operationType'>
            /// </param>
            /// <param name='assetId'>
            /// </param>
            /// <param name='cancellationToken'>
            /// The cancellation token.
            /// </param>
            public static async Task<object> GetByClientIdAsync(this IOperationsHistoryAPI operations, string clientId, int take, int skip, string operationType = default(string), string assetId = default(string), CancellationToken cancellationToken = default(CancellationToken))
            {
                using (var _result = await operations.GetByClientIdWithHttpMessagesAsync(clientId, take, skip, operationType, assetId, null, cancellationToken).ConfigureAwait(false))
                {
                    return _result.Body;
                }
            }

            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='dateFrom'>
            /// </param>
            /// <param name='dateTo'>
            /// </param>
            /// <param name='operationType'>
            /// </param>
            public static object GetByDates(this IOperationsHistoryAPI operations, System.DateTime dateFrom, System.DateTime dateTo, string operationType = default(string))
            {
                return operations.GetByDatesAsync(dateFrom, dateTo, operationType).GetAwaiter().GetResult();
            }

            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='dateFrom'>
            /// </param>
            /// <param name='dateTo'>
            /// </param>
            /// <param name='operationType'>
            /// </param>
            /// <param name='cancellationToken'>
            /// The cancellation token.
            /// </param>
            public static async Task<object> GetByDatesAsync(this IOperationsHistoryAPI operations, System.DateTime dateFrom, System.DateTime dateTo, string operationType = default(string), CancellationToken cancellationToken = default(CancellationToken))
            {
                using (var _result = await operations.GetByDatesWithHttpMessagesAsync(dateFrom, dateTo, operationType, null, cancellationToken).ConfigureAwait(false))
                {
                    return _result.Body;
                }
            }

    }
}
