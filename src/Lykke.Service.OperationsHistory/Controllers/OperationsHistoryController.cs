﻿using System;
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
using Lykke.Service.OperationsHistory.Core.Services;
using Lykke.Service.OperationsHistory.Services;
using Lykke.Service.OperationsHistory.Validation;
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

        private const int _cacheBatchPieceSize = 15;

        #endregion

        private readonly IHistoryOperationsCache _cache;
        private readonly IHistoryLogEntryRepository _repository;
        private readonly IClientAccountClient _clientAccountService;
        private readonly IHistoryOperationAdapter _adapter;

        public OperationsHistoryController(
            IHistoryOperationsCache cache, 
            IHistoryLogEntryRepository repository, 
            IClientAccountClient clientAccountService,
            IHistoryOperationAdapter adapter)
        {
            _cache = cache;
            _repository = repository;
            _clientAccountService = clientAccountService;
            _adapter = adapter;
        }

        /// <summary>
        /// Getting history by clientId
        /// </summary>
        /// <param name="clientId">Client identifier</param>
        /// <param name="operationType">The type of the operation, possible values: CashIn, CashOut, Trade</param>
        /// <param name="assetId">Asset identifier</param> 
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
            [FromQuery] HistoryOperationType? operationType,
            [FromQuery] string assetId, 
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

            var client = await _clientAccountService.GetClientByIdAsync(clientId);
            if (client == null)
            {
                return NotFound();
            }

            var wallets = await _clientAccountService.GetWalletsByClientIdAsync(clientId);

            var walletIds = wallets.Select(w => w.Type == nameof(WalletType.Trading) ? clientId : w.Id).Distinct();

            var result = new List<HistoryOperation>();

            foreach (var piece in walletIds.ToPieces(_cacheBatchPieceSize))
            {
                await Task.WhenAll(
                    piece.Select(x => _cache.GetAsync(x, operationType, assetId)
                        .ContinueWith(t =>
                        {
                            lock (result)
                            {
                                result.AddRange(t.Result);
                            }
                        })));
            }

            var pagedResult = result
                .OrderByDescending(x => x.DateTime)
                .Skip(skip)
                .Take(take);

            return Ok(pagedResult);
        }

        /// <summary>
        /// Getting history by date range, note: internal cache is not used here
        /// </summary>
        /// <param name="dateFrom">The date of the operation will be equal or greater than</param>
        /// <param name="dateTo">The date of the operation will be less than</param>
        /// <param name="operationType">The type of the operation, possible values: CashIn, CashOut, Trade</param>
        /// <returns></returns>
        [HttpGet]
        [SwaggerOperation("GetByDates")]
        [ProducesResponseType(typeof(IEnumerable<HistoryOperation>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetByDates(
            [FromQuery] DateTime dateFrom, 
            [FromQuery] DateTime dateTo,
            [FromQuery] HistoryOperationType? operationType,
            [FromQuery] string assetId)
        {
            if (dateFrom >= dateTo)
            {
                return BadRequest(ErrorResponse.Create(DateRangeError));
            }

            var dateRangeResult = (await _repository.GetByDatesAsync(dateFrom, dateTo)).OrderByDescending(r => r.DateTime);

            var adaptedOperations = await Task.WhenAll(dateRangeResult.Select(x => _adapter.Execute(x)));

            return Ok(adaptedOperations
                .Where(HistoryOperationFilterPredicates.IfTypeEquals(operationType))
                .Where(HistoryOperationFilterPredicates.IfAssetEquals(assetId)));
        }

        /// <summary>
        /// Getting history by wallet identifier
        /// </summary>
        /// <param name="walletId">Wallet identifier</param>
        /// <param name="operationType">The type of the operation, possible values: CashIn, CashOut, Trade</param>
        /// <param name="assetId">Asset identifier</param>
        /// <param name="take">How many maximum items have to be returned</param>
        /// <param name="skip">How many items skip before returning</param>
        /// <returns></returns>
        [HttpGet("wallet/{walletId}")]
        [SwaggerOperation("GetByWalletId")]
        [ProducesResponseType(typeof(IEnumerable<HistoryOperation>), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int) HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(void), (int) HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetByWalletId(string walletId,
            [FromQuery] HistoryOperationType? operationType,
            [FromQuery] string assetId,
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

            var id = wallet.Type == nameof(WalletType.Trading) ? wallet.ClientId : wallet.Id;

            var result =
                await _cache.GetAsync(id, operationType, assetId, new PaginationInfo {Take = take, Skip = skip});

            return Ok(result);
        }

        /// <summary>
        /// Getring history record by operation id
        /// </summary>
        /// <param name="walletId">Wallet identifie</param>
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

            var id = wallet.Type == nameof(WalletType.Trading) ? wallet.ClientId : wallet.Id;

            var walletOperations = await _cache.GetAsync(id);

            var operation = walletOperations.Where(x => x.Id.Equals(operationId)).FirstOrDefault();

            if (operation == null)
            {
                return NotFound();
            }

            return Ok(operation);
        }
    }
}
