using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Lykke.Service.OperationsHistory.Core.Entities;
using Lykke.Service.OperationsHistory.Models;
using Lykke.Service.OperationsHistory.Services;
using Lykke.Service.OperationsHistory.Validation;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Swashbuckle.SwaggerGen.Annotations;

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
        public static readonly string OpTypeRequired = "Operation type parameter is required";
        public static readonly string AssetRequired = "Asset id parameter is required";
        public static readonly string IdRequired = "Id parameter is required";
        public static readonly string PageOutOfRange = "Out of range value";
        public static readonly string TopOutOfRange = "Top parameter is out of range. Maximum value is 1000.";
        public static readonly string SkipOutOfRange = "Skip parameter is out of range (should be >= 0).";
        #endregion

        private readonly IHistoryCache _cache;
        private readonly IHistoryLogEntryRepository _repository;

        public OperationsHistoryController(IHistoryCache cache, IHistoryLogEntryRepository repository)
        {
            _cache = cache;
            _repository = repository;
        }

        [HttpGet("allPaged")]
        [SwaggerOperation("GetOperationsHistoryAllPaged")]
        [ProducesResponseType(typeof(IEnumerable<HistoryEntryResponse>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetOperationsHistory([FromQuery]string clientId, [FromQuery]int page = 1)
        {
            if (!ParametersValidator.ValidateClientId(clientId))
            {
                return BadRequest(ErrorResponse.Create(nameof(clientId), ClientRequiredMsg));
            }
            if (!ParametersValidator.ValidatePageIndex(page))
            {
                return BadRequest(ErrorResponse.Create(nameof(page), PageOutOfRange));
            }

            return Ok(await _cache.GetAllPagedAsync(clientId, page));
        }

        [HttpGet("all")]
        [SwaggerOperation("GetOperationsHistoryAll")]
        [ProducesResponseType(typeof(IEnumerable<HistoryEntryResponse>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetOperationsHistory([FromQuery] string clientId, [FromQuery] int top,
            [FromQuery] int skip)
        {
            if (!ParametersValidator.ValidateClientId(clientId))
            {
                return BadRequest(ErrorResponse.Create(nameof(clientId), ClientRequiredMsg));
            }
            if (!ParametersValidator.ValidateTop(top))
            {
                return BadRequest(ErrorResponse.Create(nameof(top), TopOutOfRange));
            }
            if (!ParametersValidator.ValidateSkip(skip))
            {
                return BadRequest(ErrorResponse.Create(nameof(skip), SkipOutOfRange));
            }

            return Ok(await _cache.GetAllAsync(clientId, top, skip));
        }

        [HttpGet("allByOpTypeAndAssetPaged")]
        [SwaggerOperation("GetOperationsHistoryAllByOpTypeAndAssetPaged")]
        [ProducesResponseType(typeof(IEnumerable<HistoryEntryResponse>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetOperationsHistory([FromQuery] string clientId,
            [FromQuery] string operationType, [FromQuery] string assetId,
            [FromQuery] int page = 1)
        {
            if (!ParametersValidator.ValidateClientId(clientId))
            {
                return BadRequest(ErrorResponse.Create(nameof(clientId), ClientRequiredMsg));
            }
            if (!ParametersValidator.ValidateOperationType(operationType))
            {
                return BadRequest(ErrorResponse.Create(nameof(operationType), OpTypeRequired));
            }
            if (!ParametersValidator.ValidateAssetId(assetId))
            {
                return BadRequest(ErrorResponse.Create(nameof(assetId), AssetRequired));
            }
            if (!ParametersValidator.ValidatePageIndex(page))
            {
                return BadRequest(ErrorResponse.Create(nameof(page), PageOutOfRange));
            }

            return Ok(await _cache.GetAllPagedAsync(clientId, assetId, operationType, page));
        }

        [HttpGet("allByOpTypeAndAsset")]
        [SwaggerOperation("GetOperationsHistoryAllByOpTypeAndAsset")]
        [ProducesResponseType(typeof(IEnumerable<HistoryEntryResponse>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetOperationsHistory([FromQuery] string clientId,
            [FromQuery] string operationType, [FromQuery] string assetId,
            [FromQuery] int top, [FromQuery] int skip)
        {
            if (!ParametersValidator.ValidateClientId(clientId))
            {
                return BadRequest(ErrorResponse.Create(nameof(clientId), ClientRequiredMsg));
            }
            if (!ParametersValidator.ValidateOperationType(operationType))
            {
                return BadRequest(ErrorResponse.Create(nameof(operationType), OpTypeRequired));
            }
            if (!ParametersValidator.ValidateAssetId(assetId))
            {
                return BadRequest(ErrorResponse.Create(nameof(assetId), AssetRequired));
            }
            if (!ParametersValidator.ValidateSkip(skip))
            {
                return BadRequest(ErrorResponse.Create(nameof(skip), SkipOutOfRange));
            }
            if (!ParametersValidator.ValidateTop(top))
            {
                return BadRequest(ErrorResponse.Create(nameof(top), TopOutOfRange));
            }

            return Ok(await _cache.GetAllAsync(clientId, assetId, operationType, top, skip));
        }

        [HttpGet("allByOpTypePaged")]
        [SwaggerOperation("GetOperationsHistoryAllByOpTypePaged")]
        [ProducesResponseType(typeof(IEnumerable<HistoryEntryResponse>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetOperationsHistoryByOpType([FromQuery] string clientId, 
            [FromQuery] string operationType, [FromQuery] int page = 1)
        {
            if (!ParametersValidator.ValidateClientId(clientId))
            {
                return BadRequest(ErrorResponse.Create(nameof(clientId), ClientRequiredMsg));
            }
            if (!ParametersValidator.ValidateOperationType(operationType))
            {
                return BadRequest(ErrorResponse.Create(nameof(operationType), OpTypeRequired));
            }
            if (!ParametersValidator.ValidatePageIndex(page))
            {
                return BadRequest(ErrorResponse.Create(nameof(page), PageOutOfRange));
            }

            return Ok(await _cache.GetAllByOpTypePagedAsync(clientId, operationType, page));
        }

        [HttpGet("allByOpType")]
        [SwaggerOperation("GetOperationsHistoryAllByOpType")]
        [ProducesResponseType(typeof(IEnumerable<HistoryEntryResponse>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetOperationsHistoryByOpType([FromQuery] string clientId,
            [FromQuery] string operationType, [FromQuery] int top, [FromQuery] int skip)
        {
            if (!ParametersValidator.ValidateClientId(clientId))
            {
                return BadRequest(ErrorResponse.Create(nameof(clientId), ClientRequiredMsg));
            }
            if (!ParametersValidator.ValidateOperationType(operationType))
            {
                return BadRequest(ErrorResponse.Create(nameof(operationType), OpTypeRequired));
            }
            if (!ParametersValidator.ValidateSkip(skip))
            {
                return BadRequest(ErrorResponse.Create(nameof(skip), SkipOutOfRange));
            }
            if (!ParametersValidator.ValidateTop(top))
            {
                return BadRequest(ErrorResponse.Create(nameof(top), TopOutOfRange));
            }

            return Ok(await _cache.GetAllByOpTypeAsync(clientId, operationType, top, skip));
        }

        [HttpGet("allByAssetPaged")]
        [SwaggerOperation("GetOperationsHistoryAllByAssetPaged")]
        [ProducesResponseType(typeof(IEnumerable<HistoryEntryResponse>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetOperationsHistoryByAsset([FromQuery] string clientId,
            [FromQuery] string assetId, [FromQuery] int page = 1)
        {
            if (!ParametersValidator.ValidateClientId(clientId))
            {
                return BadRequest(ErrorResponse.Create(nameof(clientId), ClientRequiredMsg));
            }
            if (!ParametersValidator.ValidateAssetId(assetId))
            {
                return BadRequest(ErrorResponse.Create(nameof(assetId), AssetRequired));
            }
            if (!ParametersValidator.ValidatePageIndex(page))
            {
                return BadRequest(ErrorResponse.Create(nameof(page), PageOutOfRange));
            }

            return Ok(await _cache.GetAllByAssetPagedAsync(clientId, assetId, page));
        }

        [HttpGet("allByAsset")]
        [SwaggerOperation("GetOperationsHistoryAllByAsset")]
        [ProducesResponseType(typeof(IEnumerable<HistoryEntryResponse>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetOperationsHistoryByAsset([FromQuery] string clientId,
            [FromQuery] string assetId, [FromQuery] int top, [FromQuery] int skip)
        {
            if (!ParametersValidator.ValidateClientId(clientId))
            {
                return BadRequest(ErrorResponse.Create(nameof(clientId), ClientRequiredMsg));
            }
            if (!ParametersValidator.ValidateAssetId(assetId))
            {
                return BadRequest(ErrorResponse.Create(nameof(assetId), AssetRequired));
            }
            if (!ParametersValidator.ValidateSkip(skip))
            {
                return BadRequest(ErrorResponse.Create(nameof(skip), SkipOutOfRange));
            }
            if (!ParametersValidator.ValidateTop(top))
            {
                return BadRequest(ErrorResponse.Create(nameof(top), TopOutOfRange));
            }

            return Ok(await _cache.GetAllByAssetAsync(clientId, assetId, top, skip));
        }

        /// <summary>
        /// Updated the record in a history by id provided with edit model provided
        /// </summary>
        /// <returns></returns>
        [HttpPost("update")]
        [SwaggerOperation("UpdateOperationsHistory")]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> UpdateOperationsHistory([FromBody] EditHistoryEntryModel editModel)
        {
            if (!ParametersValidator.ValidateId(editModel.Id))
            {
                return BadRequest(ErrorResponse.Create(nameof(editModel.Id), IdRequired));
            }

            var recordsById = await _repository.GetById(editModel.Id);
            foreach (var historyLogEntryEntity in recordsById)
            {
                dynamic parsedData = JObject.Parse(historyLogEntryEntity.CustomData);

                parsedData.State = editModel.State;
                parsedData.BlockChainHash = editModel.BlockChainHash;

                var updatedJson = parsedData.ToString(Formatting.None);
                await _repository.UpdateAsync(editModel.Id, updatedJson);
            }

            return Ok();
        }
    }
}
