using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Common;
using Common.Log;
using Lykke.Common.Api.Contract.Responses;
using Lykke.Service.AssetDisclaimers.Core.Domain;
using Lykke.Service.AssetDisclaimers.Core.Exceptions;
using Lykke.Service.AssetDisclaimers.Core.Services;
using Lykke.Service.AssetDisclaimers.Models.Disclaimers;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Lykke.Service.AssetDisclaimers.Controllers
{
    [Route("api")]
    public class ClientDisclaimersController : Controller
    {
        private readonly IClientDisclaimerService _clientDisclaimerService;
        private readonly ILog _log;

        public ClientDisclaimersController(
            IClientDisclaimerService clientDisclaimerService,
            ILog log)
        {
            _clientDisclaimerService = clientDisclaimerService;
            _log = log;
        }
        
        /// <summary>
        /// Returns a collection of disclaimers pending to approve.
        /// </summary>
        /// <param name="clientId">The client id.</param>
        /// <returns>A collection of disclaimers.</returns>
        /// <response code="200">A collection of disclaimers.</response>
        [HttpGet]
        [Route("clients/{clientId}/disclaimers/pending")]
        [SwaggerOperation("ClientDisclaimersGetPending")]
        [ProducesResponseType(typeof(List<DisclaimerModel>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetPendingAsync(string clientId)
        {
            IReadOnlyList<IDisclaimer> disclaimers = await _clientDisclaimerService.GetPendingAsync(clientId);

            var model = Mapper.Map<List<DisclaimerModel>>(disclaimers);

            return Ok(model);
        }
        
        /// <summary>
        /// Returns a collection of disclaimers for specified entities not appreved by client.
        /// </summary>
        /// <param name="clientId">The client id.</param>
        /// <param name="lykkeEntityId1">The Lykke entity id.</param>
        /// <param name="lykkeEntityId2">The Lykke entity id.</param>
        /// <returns>A collection of disclaimers.</returns>
        /// <response code="200">A collection of disclaimers.</response>
        /// <response code="404">The Lykke entity not found.</response>
        [HttpGet]
        [Route("clients/{clientId}/disclaimers")]
        [SwaggerOperation("ClientDisclaimersGetNotApproved")]
        [ProducesResponseType(typeof(List<DisclaimerModel>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetNotApprovedAsync(string clientId, string lykkeEntityId1, string lykkeEntityId2)
        {
            IReadOnlyList<IDisclaimer> disclaimers;
            
            try
            {
                disclaimers = await _clientDisclaimerService.GetNotApprovedAsync(clientId,
                    new List<string>
                        {
                            lykkeEntityId1,
                            lykkeEntityId2
                        }.Where(o => !string.IsNullOrEmpty(o))
                        .ToList());
            }
            catch (LykkeEntityNotFoundException exception)
            {
                await _log.WriteErrorAsync(nameof(ClientDisclaimersController), nameof(GetNotApprovedAsync),
                    new
                    {
                        clientId,
                        lykkeEntityId1,
                        lykkeEntityId2
                    }.ToJson(), exception);
                return NotFound(ErrorResponse.Create(exception.Message));
            }
            
            var model = Mapper.Map<List<DisclaimerModel>>(disclaimers);

            return Ok(model);
        }

        /// <summary>
        /// Adds a disclimer to client. This disclaimer awaits the client's approval. 
        /// </summary>
        /// <param name="clientId">The client id.</param>
        /// <param name="disclaimerId">The disclaimer id.</param>
        /// <returns>No content.</returns>
        /// <response code="204">A disclaimer added to client.</response>
        /// <response code="404">The disclaimer not found.</response>
        [HttpPost]
        [Route("clients/{clientId}/disclaimers/{disclaimerId}")]
        [SwaggerOperation("ClientDisclaimersAdd")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> AddPendingAsync(string clientId, string disclaimerId)
        {
            try
            {
                await _clientDisclaimerService.AddPendingAsync(clientId, disclaimerId);
            }
            catch (DisclaimerNotFoundException exception)
            {
                await _log.WriteErrorAsync(nameof(ClientDisclaimersController), nameof(AddPendingAsync),
                    new
                    {
                        clientId,
                        disclaimerId
                    }.ToJson(), exception);
                return NotFound(ErrorResponse.Create(exception.Message));
            }

            return NoContent();
        }
        
        /// <summary>
        /// Sets client disclimer as approved. 
        /// </summary>
        /// <param name="clientId">The client id.</param>
        /// <param name="disclaimerId">The disclaimer id.</param>
        /// <returns>No content.</returns>
        /// <response code="204">The client disclaimer successfully approved.</response>
        /// <response code="404">The disclaimer not found.</response>
        [HttpPost]
        [Route("clients/{clientId}/disclaimers/{disclaimerId}/approve")]
        [SwaggerOperation("InvoicesGet")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> ApproveAsync(string clientId, string disclaimerId)
        {
            try
            {
                await _clientDisclaimerService.ApproveAsync(clientId, disclaimerId);
            }
            catch (DisclaimerNotFoundException exception)
            {
                await _log.WriteErrorAsync(nameof(ClientDisclaimersController), nameof(ApproveAsync),
                    new
                    {
                        clientId,
                        disclaimerId
                    }.ToJson(), exception);
                return NotFound(ErrorResponse.Create(exception.Message));
            }

            return NoContent();
        }
        
        /// <summary>
        /// Sets client disclimer as declined. 
        /// </summary>
        /// <param name="clientId">The client id.</param>
        /// <param name="disclaimerId">The disclaimer id.</param>
        /// <returns>No content.</returns>
        /// <response code="204">The client disclaimer successfully declined.</response>
        /// <response code="404">The disclaimer not found.</response>
        [HttpPost]
        [Route("clients/{clientId}/disclaimers/{disclaimerId}/decline")]
        [SwaggerOperation("InvoicesGet")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> DeclineAsync(string clientId, string disclaimerId)
        {
            await _clientDisclaimerService.DeclineAsync(clientId, disclaimerId);

            return NoContent();
        }
    }
}
