using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Lykke.Service.ClientAccount.Client;
using Lykke.Service.ClientAccount.Client.AutorestClient.Models;
using Lykke.Service.OperationsHistory.Core.Domain;
using Lykke.Service.OperationsHistory.Core.Entities;
using Lykke.Service.OperationsHistory.Models;
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
        public static readonly string ClientNotExists = "Client doesn't exist";
        public static readonly string TakeOutOfRange = "Top parameter is out of range. Maximum value is 1000.";
        public static readonly string SkipOutOfRange = "Skip parameter is out of range (should be >= 0).";
        public static readonly string DateRangeError = "[dateFrom] can't be greater than or equal to [dateTo]";
        public static readonly string WalletNotExists = "Wallet doesn't exist";
        #endregion

        private readonly IHistoryCache _cache;
        private readonly IHistoryLogEntryRepository _repository;
        private readonly IClientAccountClient _clientAccountService;

        public OperationsHistoryController(IHistoryCache cache, IHistoryLogEntryRepository repository, IClientAccountClient clientAccountService)
        {
            _cache = cache;
            _repository = repository;
            _clientAccountService = clientAccountService;
        }

        [HttpGet("client/{clientId}")]
        [SwaggerOperation("GetByClientId")]
        [ProducesResponseType(typeof(IEnumerable<HistoryEntryClientResponse>), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int) HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetByClientId(
            string clientId, 
            [FromQuery] string operationType,
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
                return BadRequest(ErrorResponse.Create(ClientNotExists));
            }

            var wallets = await _clientAccountService.GetWalletsByClientIdAsync(clientId);

            var walletIds = wallets.Select(w => w.Type == nameof(WalletType.Trading) ? clientId : w.Id).Distinct();

            var result = new List<IHistoryLogEntryEntity>();

            await Task.WhenAll(
                walletIds
                    .Select(x => _cache.GetAsync(x, operationType, assetId)
                        .ContinueWith(t =>
                        {
                            lock (result)
                            {
                                result.AddRange(t.Result);
                            }
                        })));

            var pagedResult = result
                .OrderByDescending(x => x.DateTime)
                .Skip(skip)
                .Take(take);

            return Ok(Mapper.Map<IEnumerable<HistoryEntryClientResponse>>(pagedResult));
        }

        [HttpGet]
        [SwaggerOperation("GetByDates")]
        [ProducesResponseType(typeof(IEnumerable<HistoryEntryWalletResponse>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetByDates(
            [FromQuery] DateTime dateFrom, 
            [FromQuery] DateTime dateTo,
            [FromQuery] string operationType)
        {
            if (dateFrom >= dateTo)
            {
                return BadRequest(ErrorResponse.Create(DateRangeError));
            }

            var dateRangeResult = (await _repository.GetByDatesAsync(dateFrom, dateTo)).OrderByDescending(r => r.DateTime);

            return Ok(string.IsNullOrWhiteSpace(operationType)
                ? dateRangeResult
                : dateRangeResult.Where(x => x.OpType == operationType));
        }

        [HttpGet("wallet/{walletId}")]
        [SwaggerOperation("GetByWalletId")]
        [ProducesResponseType(typeof(IEnumerable<HistoryEntryWalletResponse>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetByWalletId(string walletId,
            [FromQuery] string operationType,
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
                return BadRequest(ErrorResponse.Create(WalletNotExists));
            }

            var id = wallet.Type == nameof(WalletType.Trading) ? wallet.ClientId : wallet.Id;

            var result = await _cache.GetAsync(id, operationType, assetId, new PaginationInfo {Take = take, Skip = skip});

            return Ok(Mapper.Map<IEnumerable<HistoryEntryWalletResponse>>(result));
        }
    }
}
