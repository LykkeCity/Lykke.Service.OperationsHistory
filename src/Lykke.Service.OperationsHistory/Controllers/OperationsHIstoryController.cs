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
        private readonly string _clientRequiredMsg = "Client id is required";
        private readonly string _opTypeRequired = "Operation type parameter is required";
        private readonly string _assetRequired = "Asset id parameter is required";
        private readonly string _pageOutOfRange = "Out of range value";
        #endregion

        private readonly IHistoryCache _cache;
        public OperationsHistoryController(IHistoryCache cache)
        {
            _cache = cache;
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetOperationsHistory([FromQuery]string clientId, [FromQuery]int page = 1)
        {
            if (!ParametersValidator.ValidateClientId(clientId))
            {
                return BadRequest(ErrorResponse.Create(nameof(clientId), _clientRequiredMsg));
            }
            if (!ParametersValidator.ValidatePageIndex(page))
            {
                return BadRequest(ErrorResponse.Create(nameof(page), _pageOutOfRange));
            }

            return Ok(await _cache.GetAllAsync(clientId, page));
        }

        [HttpGet("allByOpTypeAndAssetId")]
        public async Task<IActionResult> GetOperationsHistory([FromQuery] string clientId,
            [FromQuery] string operationType, [FromQuery] string assetId,
            [FromQuery] int page = 1)
        {
            if (!ParametersValidator.ValidateClientId(clientId))
            {
                return BadRequest(ErrorResponse.Create(nameof(clientId), _clientRequiredMsg));
            }
            if (!ParametersValidator.ValidateOperationType(operationType))
            {
                return BadRequest(ErrorResponse.Create(nameof(operationType), _opTypeRequired));
            }
            if (!ParametersValidator.ValidateAssetId(assetId))
            {
                return BadRequest(ErrorResponse.Create(nameof(assetId), _assetRequired));
            }
            if (!ParametersValidator.ValidatePageIndex(page))
            {
                return BadRequest(ErrorResponse.Create(nameof(page), _pageOutOfRange));
            }

            return Ok(await _cache.GetAllAsync(clientId, assetId, operationType, page));
        }

        [HttpGet("allByOpType")]
        public async Task<IActionResult> GetOperationsHistoryByOpType([FromQuery] string clientId, 
            [FromQuery] string operationType, [FromQuery] int page = 1)
        {
            if (!ParametersValidator.ValidateClientId(clientId))
            {
                return BadRequest(ErrorResponse.Create(nameof(clientId), _clientRequiredMsg));
            }
            if (!ParametersValidator.ValidateOperationType(operationType))
            {
                return BadRequest(ErrorResponse.Create(nameof(operationType), _opTypeRequired));
            }
            if (!ParametersValidator.ValidatePageIndex(page))
            {
                return BadRequest(ErrorResponse.Create(nameof(page), _pageOutOfRange));
            }

            return Ok(await _cache.GetAllByOpTypeAsync(clientId, operationType, page));
        }

        [HttpGet("allByAsset")]
        public async Task<IActionResult> GetOperationsHistoryByAsset([FromQuery] string clientId,
            [FromQuery] string assetId, [FromQuery] int page = 1)
        {
            if (!ParametersValidator.ValidateClientId(clientId))
            {
                return BadRequest(ErrorResponse.Create(nameof(clientId), _clientRequiredMsg));
            }
            if (!ParametersValidator.ValidateAssetId(assetId))
            {
                return BadRequest(ErrorResponse.Create(nameof(assetId), _assetRequired));
            }
            if (!ParametersValidator.ValidatePageIndex(page))
            {
                return BadRequest(ErrorResponse.Create(nameof(page), _pageOutOfRange));
            }

            return Ok(await _cache.GetAllByAssetAsync(clientId, assetId, page));
        }
    }
}
