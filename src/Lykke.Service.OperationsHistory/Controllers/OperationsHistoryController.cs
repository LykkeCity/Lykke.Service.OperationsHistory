using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Lykke.Service.ClientAccount.Client;
using Lykke.Service.OperationsHistory.Core.Entities;
using Lykke.Service.OperationsHistory.Models;
using Lykke.Service.OperationsHistory.Services;
using Lykke.Service.OperationsHistory.Validation;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.SwaggerGen;

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

        [HttpGet("{clientId}")]
        [SwaggerOperation("GetByClientId")]
        [ProducesResponseType(typeof(IEnumerable<HistoryEntryResponse>), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int) HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetByClientId(
            string clientId, 
            [FromQuery] string operationType,
            [FromQuery] string assetId, 
            [FromQuery] int take, 
            [FromQuery] int skip)
        {
            if (!ParametersValidator.ValidateClientId(clientId))
            {
                return BadRequest(ErrorResponse.Create(nameof(clientId), ClientRequiredMsg));
            }
            if (!ParametersValidator.ValidateSkip(skip))
            {
                return BadRequest(ErrorResponse.Create(nameof(skip), SkipOutOfRange));
            }
            if (!ParametersValidator.ValidateTake(take))
            {
                return BadRequest(ErrorResponse.Create(nameof(take), TakeOutOfRange));
            }

            var client = await _clientAccountService.GetClientByIdAsync(clientId);
            if (client == null)
            {
                return BadRequest(ErrorResponse.Create(ClientNotExists));
            }

            return Ok(await _cache.GetAsync(clientId, operationType, assetId, take, skip));
        }

        [HttpGet]
        [SwaggerOperation("GetByDates")]
        [ProducesResponseType(typeof(IEnumerable<HistoryEntryResponse>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetByDates(
            [FromQuery] DateTime dateFrom, 
            [FromQuery] DateTime dateTo)
        {
            if (dateFrom >= dateTo)
            {
                return BadRequest(ErrorResponse.Create(DateRangeError));
            }

            return Ok(await _repository.GetByDatesAsync(dateFrom, dateTo));
        }
    }
}
