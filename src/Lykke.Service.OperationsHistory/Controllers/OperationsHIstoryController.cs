using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Lykke.Service.OperationsHistory.Models;
using Microsoft.AspNetCore.Mvc;

namespace Lykke.Service.OperationsHistory.Controllers
{
    /// <summary>
    /// Controller for operations history
    /// </summary>
    [Route("api/[controller]")]
    public class OperationsHistoryController: Controller
    {
        [HttpGet("{clientId}")]
        public async Task<IActionResult> GetOperationsHistory(string clientId, int page)
        {
            if (string.IsNullOrWhiteSpace(clientId))
            {
                return BadRequest(ErrorResponse.Create(nameof(clientId), "Client id is required"));
            }
            if (page < 0)
            {
                return BadRequest(ErrorResponse.Create(nameof(page), "Out of range value"));
            }

            return Ok($"Testing clientid = {clientId}, page = {page}");
        }
    }
}
