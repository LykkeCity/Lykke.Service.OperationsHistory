using System.Threading.Tasks;
using Lykke.Service.OperationsHistory.Models;
using Lykke.Service.OperationsHistory.Services;
using Lykke.Service.OperationsHistory.Validation;
using Microsoft.AspNetCore.Mvc;

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
        public static readonly string PageOutOfRange = "Out of range value";
        public static readonly string TopOutOfRange = "Top parameter is out of range. Maximum value is 1000.";
        public static readonly string SkipOutOfRange = "Skip parameter is out of range (should be >= 0).";
        #endregion

        private readonly IHistoryCache _cache;
        public OperationsHistoryController(IHistoryCache cache)
        {
            _cache = cache;
        }

        [HttpGet("allPaged")]
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
        public async Task<IActionResult> GetOperationsHistory([FromQuery] string clientId, [FromQuery] int top,
            [FromQuery] int skip)
        {
            if (!ParametersValidator.ValidateClientId(clientId))
            {
                return BadRequest(ErrorResponse.Create(nameof(clientId), ClientRequiredMsg));
            }
            if (!ParametersValidator.ValidatePageIndex(top))
            {
                return BadRequest(ErrorResponse.Create(nameof(top), TopOutOfRange));
            }
            if (!ParametersValidator.ValidatePageIndex(skip))
            {
                return BadRequest(ErrorResponse.Create(nameof(skip), SkipOutOfRange));
            }

            return Ok(await _cache.GetAllAsync(clientId, top, skip));
        }

        [HttpGet("allByOpTypeAndAssetPaged")]
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
            if (!ParametersValidator.ValidatePageIndex(skip))
            {
                return BadRequest(ErrorResponse.Create(nameof(skip), SkipOutOfRange));
            }
            if (!ParametersValidator.ValidatePageIndex(top))
            {
                return BadRequest(ErrorResponse.Create(nameof(top), TopOutOfRange));
            }

            return Ok(await _cache.GetAllAsync(clientId, assetId, operationType, top, skip));
        }

        [HttpGet("allByOpTypePaged")]
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
            if (!ParametersValidator.ValidatePageIndex(skip))
            {
                return BadRequest(ErrorResponse.Create(nameof(skip), SkipOutOfRange));
            }
            if (!ParametersValidator.ValidatePageIndex(top))
            {
                return BadRequest(ErrorResponse.Create(nameof(top), TopOutOfRange));
            }

            return Ok(await _cache.GetAllByOpTypeAsync(clientId, operationType, top, skip));
        }

        [HttpGet("allByAssetPaged")]
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
            if (!ParametersValidator.ValidatePageIndex(skip))
            {
                return BadRequest(ErrorResponse.Create(nameof(skip), SkipOutOfRange));
            }
            if (!ParametersValidator.ValidatePageIndex(top))
            {
                return BadRequest(ErrorResponse.Create(nameof(top), TopOutOfRange));
            }

            return Ok(await _cache.GetAllByAssetAsync(clientId, assetId, top, skip));
        }
    }
}
