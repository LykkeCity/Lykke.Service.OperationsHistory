using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Common;
using Lykke.Service.ClientAccount.Client;
using Lykke.Service.ClientAccount.Client.AutorestClient.Models;
using Lykke.Service.OperationsHistory.Core;
using Lykke.Service.OperationsHistory.Core.Domain;
using Lykke.Service.OperationsHistory.Core.Entities;
using Lykke.Service.OperationsHistory.Core.Entities.MigrationFlag;
using Lykke.Service.OperationsHistory.Core.Services;
using Lykke.Service.OperationsHistory.Mongo;
using Lykke.Service.OperationsHistory.Services;
using Lykke.Service.OperationsHistory.Validation;
using Lykke.Service.OperationsRepository.Contract;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.SwaggerGen;
using ErrorResponse = Lykke.Service.OperationsHistory.Models.ErrorResponse;

namespace Lykke.Service.OperationsHistory.Controllers
{
    /// <summary>
    /// Controller for operations history
    /// </summary>
    [Route("api/[controller]")]
    public class OperationsHistoryController: Controller
    {
        #region error messages
        public static readonly string ClientRequiredMsg = "Client id is required";
        public static readonly string TakeOutOfRange = "Top parameter is out of range, should be [1..1000].";
        public static readonly string SkipOutOfRange = "Skip parameter is out of range, should be >= 0.";
        public static readonly string DateRangeError = "[dateFrom] can't be greater than or equal to [dateTo]";
        #endregion

        #region consts

        private const int CacheBatchPieceSize = 15;

        #endregion

        private readonly IHistoryOperationsCache _cache;
        private readonly IHistoryLogEntryRepository _historyLogRepository;
        private readonly IClientAccountClient _clientAccountService;
        private readonly IHistoryOperationAdapter _adapter;
        private readonly IOperationsHistoryRepository _operationsHistoryRepository;
        private readonly IMigrationFlagsRepository _migrationFlagsRepository;

        public OperationsHistoryController(
            IHistoryOperationsCache cache, 
            IHistoryLogEntryRepository historyLogRepository, 
            IClientAccountClient clientAccountService,
            IHistoryOperationAdapter adapter,
            IOperationsHistoryRepository operationsHistoryRepository,
            IMigrationFlagsRepository migrationFlagsRepository)
        {
            _cache = cache;
            _historyLogRepository = historyLogRepository;
            _clientAccountService = clientAccountService;
            _adapter = adapter;
            _operationsHistoryRepository = operationsHistoryRepository;
            _migrationFlagsRepository = migrationFlagsRepository;
        }

        /// <summary>
        /// Getting history by clientId
        /// </summary>
        /// <param name="clientId">Client identifier</param>
        /// <param name="operationTypes">The type of the operation, possible values: CashIn, CashOut, Trade</param>
        /// <param name="assetId">Asset identifier</param>
        /// <param name="assetPairId">AssetPair identifier</param>
        /// <param name="take">How many maximum items have to be returned</param>
        /// <param name="skip">How many items skip before returning</param>
        /// <returns></returns>
        [HttpGet("client/{clientId}")]
        [SwaggerOperation("GetByClientId")]
        [ProducesResponseType(typeof(IEnumerable<HistoryOperation>), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int) HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetByClientId(
            string clientId, 
            [FromQuery] HistoryOperationType[] operationTypes,
            [FromQuery] string assetId,
            [FromQuery] string assetPairId,
            [FromQuery] int? take, 
            [FromQuery] int skip)
        {
            if (!ParametersValidator.ValidateSkip(skip))
            {
                return BadRequest(ErrorResponse.Create(SkipOutOfRange));
            }
            if (take.HasValue && !ParametersValidator.ValidateTake(take.Value))
            {
                return BadRequest(ErrorResponse.Create(TakeOutOfRange));
            }

            var client = await _clientAccountService.GetClientByIdAsync(clientId);
            if (client == null)
            {
                return NotFound();
            }

            if (await _migrationFlagsRepository.ClientWasMigrated(clientId))
            {
                var mongoresult = await _operationsHistoryRepository.GetByClientIdAsync(
                    clientId,
                    null,
                    operationTypes ?? new HistoryOperationType[0],
                    assetId,
                    assetPairId,
                    take,
                    skip);

                return Ok(mongoresult.Select(x => x.ToHistoryOperation()));
            }

            var wallets = await _clientAccountService.GetWalletsByClientIdAsync(clientId);

            var walletIds = wallets.Select(w => w.Type == nameof(WalletType.Trading) ? clientId : w.Id).Distinct();

            var result = new List<HistoryOperation>();

            foreach (var piece in walletIds.ToPieces(CacheBatchPieceSize))
            {
                await Task.WhenAll(
                    piece.Select(x => _cache.GetAsync(x, operationTypes, assetId, assetPairId)
                        .ContinueWith(t =>
                        {
                            lock (result)
                            {
                                result.AddRange(t.Result);
                            }
                        })));
            }

            var pagedResultWithoutTake = result
                .OrderByDescending(x => x.DateTime)
                .Skip(skip);

            return Ok(take.HasValue ? pagedResultWithoutTake.Take(take.Value) : pagedResultWithoutTake);
        }

        /// <summary>
        /// Getting history by date range, note: internal cache is not used here
        /// </summary>
        /// <param name="dateFrom">The date of the operation will be equal or greater than</param>
        /// <param name="dateTo">The date of the operation will be less than</param>
        /// <param name="operationType">The type of the operation, possible values: CashIn, CashOut, Trade</param>
        /// <param name="assetId">Id of the asset</param>
        /// <param name="assetPairId">AssetPair identifier</param>
        /// <returns></returns>
        [HttpGet]
        [SwaggerOperation("GetByDates")]
        [ProducesResponseType(typeof(IEnumerable<HistoryOperation>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetByDates(
            [FromQuery] DateTime dateFrom, 
            [FromQuery] DateTime dateTo,
            [FromQuery] HistoryOperationType? operationType,
            [FromQuery] string assetId,
            [FromQuery] string assetPairId)
        {
            if (dateFrom >= dateTo)
            {
                return BadRequest(ErrorResponse.Create(DateRangeError));
            }

            var dateRangeResult = (await _historyLogRepository.GetByDatesAsync(dateFrom, dateTo)).OrderByDescending(r => r.DateTime);

            var adaptedOperations = await Task.WhenAll(dateRangeResult.Select(x => _adapter.ExecuteAsync(x)));

            return Ok(adaptedOperations
                .Where(HistoryOperationFilterPredicates.IfTypeEquals(operationType))
                .Where(HistoryOperationFilterPredicates.IfAssetEquals(assetId))
                .Where(HistoryOperationFilterPredicates.IfAssetPairEquals(assetPairId)));
        }

        /// <summary>
        /// Getting history by wallet identifier
        /// </summary>
        /// <param name="walletId">Wallet identifier</param>
        /// <param name="operationTypes">The type of the operation, possible values: CashIn, CashOut, Trade</param>
        /// <param name="assetId">Asset identifier</param>
        /// <param name="assetPairId">AssetPair identifier</param>
        /// <param name="take">How many maximum items have to be returned</param>
        /// <param name="skip">How many items skip before returning</param>
        /// <returns></returns>
        [HttpGet("wallet/{walletId}")]
        [SwaggerOperation("GetByWalletId")]
        [ProducesResponseType(typeof(IEnumerable<HistoryOperation>), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int) HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(void), (int) HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetByWalletId(string walletId,
            [FromQuery] HistoryOperationType[] operationTypes,
            [FromQuery] string assetId,
            [FromQuery] string assetPairId,
            [FromQuery] int take,
            [FromQuery] int skip)
        {
            if (!ParametersValidator.ValidateSkip(skip))
            {
                return BadRequest(ErrorResponse.Create(SkipOutOfRange));
            }
            if (!ParametersValidator.ValidateTake(take))
            {
                return BadRequest(ErrorResponse.Create(TakeOutOfRange));
            }

            var wallet = await _clientAccountService.GetWalletAsync(walletId);
            if (wallet == null)
            {
                return NotFound();
            }
            
            if (await _migrationFlagsRepository.ClientWasMigrated(wallet.ClientId))
            {
                var mongoresult = await _operationsHistoryRepository.GetByClientIdAsync(
                    wallet.ClientId,
                    walletId,
                    operationTypes ?? new HistoryOperationType[0],
                    assetId,
                    assetPairId,
                    take,
                    skip);

                return Ok(mongoresult.Select(x => x.ToHistoryOperation()));
            }

            var id = wallet.Type == nameof(WalletType.Trading) ? wallet.ClientId : wallet.Id;
            
            var result =
                await _cache.GetAsync(id, operationTypes, assetId, assetPairId, new PaginationInfo {Take = take, Skip = skip});

            return Ok(result);
        }

        /// <summary>
        /// Getting history record by operation id
        /// </summary>
        /// <param name="walletId">Wallet identifier</param>
        /// <param name="operationId">Operation identifier</param>
        /// <returns></returns>
        [HttpGet("wallet/{walletId}/operation/{operationId}")]
        [SwaggerOperation("GetByOperationId")]
        [ProducesResponseType(typeof(HistoryOperation), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(void), (int) HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetByOperationId(string walletId, string operationId)
        {
            var wallet = await _clientAccountService.GetWalletAsync(walletId);
            if (wallet == null)
            {
                return NotFound();
            }

            if (await _migrationFlagsRepository.ClientWasMigrated(wallet.ClientId))
            {
                return Ok((await _operationsHistoryRepository.GetByIdAsync(wallet.ClientId, operationId)).ToHistoryOperation());
            }

            var id = wallet.Type == nameof(WalletType.Trading) ? wallet.ClientId : wallet.Id;

            var walletOperations = await _cache.GetAsync(id);

            var operation = walletOperations.FirstOrDefault(x => x.Id.Equals(operationId));

            if (operation == null)
            {
                return NotFound();
            }

            return Ok(operation);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="operationId"></param>
        /// <returns></returns>
        [HttpDelete("client/{clientId}/operation/{operationId}")]
        [ProducesResponseType(typeof(HistoryOperation), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(void), (int) HttpStatusCode.NotFound)]
        public async Task<IActionResult> DeleteOperationId(string clientId, string operationId)
        {
            var wallets = await _clientAccountService.GetWalletsByClientIdAsync(clientId);
            
            if (wallets == null || !wallets.Any())
            {
                return NotFound();
            }

            var operation = new HistoryOperation();
            string walletId = null;

            foreach (var wallet in wallets)
            {
                var id = wallet.Type == nameof(WalletType.Trading) ? wallet.ClientId : wallet.Id;
                
                var walletOperations = await _cache.GetAsync(id);

                operation = walletOperations.FirstOrDefault(x => x.Id.Equals(operationId));

                if (operation == null) continue;
                
                walletId = id;
                break;
            }

            if (walletId == null)
            {
                return NotFound();
            }

            await _historyLogRepository.DeleteIfExistsAsync(walletId, operationId);

            await _cache.RemoveIfLoaded(walletId, operationId);

            await _operationsHistoryRepository.DeleteIfExistsAsync(clientId, operationId);

            return Ok(operation);
        }
    }
}
