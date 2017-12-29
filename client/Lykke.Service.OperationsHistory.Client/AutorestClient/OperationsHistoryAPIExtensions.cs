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

            /// <summary>
            /// Getting history by clientId
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='clientId'>
            /// Client identifier
            /// </param>
            /// <param name='take'>
            /// How many maximum items have to be returned
            /// </param>
            /// <param name='skip'>
            /// How many items skip before returning
            /// </param>
            /// <param name='operationType'>
            /// The type of the operation, possible values: CashIn, CashOut, Trade
            /// </param>
            /// <param name='assetId'>
            /// Asset identifier
            /// </param>
            public static object GetByClientId(this IOperationsHistoryAPI operations, string clientId, int take, int skip, string operationType = default(string), string assetId = default(string))
            {
                return operations.GetByClientIdAsync(clientId, take, skip, operationType, assetId).GetAwaiter().GetResult();
            }

            /// <summary>
            /// Getting history by clientId
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='clientId'>
            /// Client identifier
            /// </param>
            /// <param name='take'>
            /// How many maximum items have to be returned
            /// </param>
            /// <param name='skip'>
            /// How many items skip before returning
            /// </param>
            /// <param name='operationType'>
            /// The type of the operation, possible values: CashIn, CashOut, Trade
            /// </param>
            /// <param name='assetId'>
            /// Asset identifier
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

            /// <summary>
            /// Getting history by date range, note: internal cache is not used here
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='dateFrom'>
            /// The date of the operation will be equal or greater than
            /// </param>
            /// <param name='dateTo'>
            /// The date of the operation will be less than
            /// </param>
            /// <param name='operationType'>
            /// The type of the operation, possible values: CashIn, CashOut, Trade
            /// </param>
            /// <param name='assetId'>
            /// </param>
            public static object GetByDates(this IOperationsHistoryAPI operations, System.DateTime dateFrom, System.DateTime dateTo, string operationType = default(string), string assetId = default(string))
            {
                return operations.GetByDatesAsync(dateFrom, dateTo, operationType, assetId).GetAwaiter().GetResult();
            }

            /// <summary>
            /// Getting history by date range, note: internal cache is not used here
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='dateFrom'>
            /// The date of the operation will be equal or greater than
            /// </param>
            /// <param name='dateTo'>
            /// The date of the operation will be less than
            /// </param>
            /// <param name='operationType'>
            /// The type of the operation, possible values: CashIn, CashOut, Trade
            /// </param>
            /// <param name='assetId'>
            /// </param>
            /// <param name='cancellationToken'>
            /// The cancellation token.
            /// </param>
            public static async Task<object> GetByDatesAsync(this IOperationsHistoryAPI operations, System.DateTime dateFrom, System.DateTime dateTo, string operationType = default(string), string assetId = default(string), CancellationToken cancellationToken = default(CancellationToken))
            {
                using (var _result = await operations.GetByDatesWithHttpMessagesAsync(dateFrom, dateTo, operationType, assetId, null, cancellationToken).ConfigureAwait(false))
                {
                    return _result.Body;
                }
            }

            /// <summary>
            /// Getting history by wallet identifier
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='walletId'>
            /// Wallet identifier
            /// </param>
            /// <param name='take'>
            /// How many maximum items have to be returned
            /// </param>
            /// <param name='skip'>
            /// How many items skip before returning
            /// </param>
            /// <param name='operationType'>
            /// The type of the operation, possible values: CashIn, CashOut, Trade
            /// </param>
            /// <param name='assetId'>
            /// Asset identifier
            /// </param>
            public static object GetByWalletId(this IOperationsHistoryAPI operations, string walletId, int take, int skip, string operationType = default(string), string assetId = default(string))
            {
                return operations.GetByWalletIdAsync(walletId, take, skip, operationType, assetId).GetAwaiter().GetResult();
            }

            /// <summary>
            /// Getting history by wallet identifier
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='walletId'>
            /// Wallet identifier
            /// </param>
            /// <param name='take'>
            /// How many maximum items have to be returned
            /// </param>
            /// <param name='skip'>
            /// How many items skip before returning
            /// </param>
            /// <param name='operationType'>
            /// The type of the operation, possible values: CashIn, CashOut, Trade
            /// </param>
            /// <param name='assetId'>
            /// Asset identifier
            /// </param>
            /// <param name='cancellationToken'>
            /// The cancellation token.
            /// </param>
            public static async Task<object> GetByWalletIdAsync(this IOperationsHistoryAPI operations, string walletId, int take, int skip, string operationType = default(string), string assetId = default(string), CancellationToken cancellationToken = default(CancellationToken))
            {
                using (var _result = await operations.GetByWalletIdWithHttpMessagesAsync(walletId, take, skip, operationType, assetId, null, cancellationToken).ConfigureAwait(false))
                {
                    return _result.Body;
                }
            }

            /// <summary>
            /// Getring history record by operation id
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='walletId'>
            /// Wallet identifie
            /// </param>
            /// <param name='operationId'>
            /// Operation identifier
            /// </param>
            public static HistoryOperation GetByOperationId(this IOperationsHistoryAPI operations, string walletId, string operationId)
            {
                return operations.GetByOperationIdAsync(walletId, operationId).GetAwaiter().GetResult();
            }

            /// <summary>
            /// Getring history record by operation id
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='walletId'>
            /// Wallet identifie
            /// </param>
            /// <param name='operationId'>
            /// Operation identifier
            /// </param>
            /// <param name='cancellationToken'>
            /// The cancellation token.
            /// </param>
            public static async Task<HistoryOperation> GetByOperationIdAsync(this IOperationsHistoryAPI operations, string walletId, string operationId, CancellationToken cancellationToken = default(CancellationToken))
            {
                using (var _result = await operations.GetByOperationIdWithHttpMessagesAsync(walletId, operationId, null, cancellationToken).ConfigureAwait(false))
                {
                    return _result.Body;
                }
            }

    }
}
