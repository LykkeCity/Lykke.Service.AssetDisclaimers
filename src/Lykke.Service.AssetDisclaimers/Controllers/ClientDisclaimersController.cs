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
using Lykke.Service.AssetDisclaimers.Models.ClientDisclaimers;
using Lykke.Service.AssetDisclaimers.Models.Disclaimers;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Lykke.Service.AssetDisclaimers.Controllers
{
    [Route("api")]
    public class ClientDisclaimersController : Controller
    {
        private readonly IClientDisclaimerService _clientDisclaimerService;
        private readonly IDisclaimerService _disclaimerService;
        private readonly ILog _log;

        public ClientDisclaimersController(
            IClientDisclaimerService clientDisclaimerService,
            IDisclaimerService disclaimerService,
            ILog log)
        {
            _clientDisclaimerService = clientDisclaimerService;
            _disclaimerService = disclaimerService;
            _log = log;
        }
        
        /// <summary>
        /// Returns client approved disclaimers.
        /// </summary>
        /// <param name="clientId">The client id.</param>
        /// <returns>A collection of disclaimers.</returns>
        /// <response code="200">A collection of disclaimers.</response>
        [HttpGet]
        [Route("clients/{clientId}/disclaimers/approved")]
        [SwaggerOperation("ClientDisclaimersGetApproved")]
        [ProducesResponseType(typeof(List<ClientDisclaimerModel>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetApprovedAsync(string clientId)
        {
            IReadOnlyList<IClientDisclaimer> clienApprovedDisclaimers =
                await _clientDisclaimerService.GetApprovedAsync(clientId);

            var model = new List<ClientDisclaimerModel>();

            foreach (IClientDisclaimer clientDisclaimer in clienApprovedDisclaimers)
            {
                IDisclaimer disclaimer = await _disclaimerService.FindAsync(clientDisclaimer.DisclaimerId);

                if (disclaimer == null)
                    continue;

                var clientDisclaimerModel = Mapper.Map<ClientDisclaimerModel>(disclaimer);
                clientDisclaimerModel.ApprovedDate = clientDisclaimer.ApprovedDate;

                model.Add(clientDisclaimerModel);
            }

            return Ok(model);
        }
        
        /// <summary>
        /// Returns client disclaimers requires approval.
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
            IReadOnlyList<IClientDisclaimer> clienPendingDisclaimers = await _clientDisclaimerService.GetPendingAsync(clientId);

            var disclaimers = new List<IDisclaimer>();

            foreach (IClientDisclaimer clientDisclaimer in clienPendingDisclaimers)
            {
                IDisclaimer disclaimer = await _disclaimerService.FindAsync(clientDisclaimer.DisclaimerId);

                if (disclaimer != null)
                    disclaimers.Add(disclaimer);
            }

            if (disclaimers.Any())
            {
                await _log.WriteInfoAsync(nameof(ClientDisclaimersController), nameof(GetPendingAsync), clientId, disclaimers.ToJson());
            }

            var model = Mapper.Map<List<DisclaimerModel>>(disclaimers);

            return Ok(model);
        }
        
        /// <summary>
        /// Finds an actual for today tradable disclaimer by higher priority Lykke entity and requires client approval if it has not yet been approved.
        /// </summary>
        /// <param name="clientId">The client id.</param>
        /// <param name="lykkeEntityId1">The Lykke entity id.</param>
        /// <param name="lykkeEntityId2">The Lykke entity id.</param>
        /// <returns>A check result.</returns>
        /// <response code="200">A check result.</response>
        /// <response code="404">The Lykke entity not found.</response>
        [HttpPost]
        [Route("clients/{clientId}/disclaimers/tradable")]
        [SwaggerOperation("ClientDisclaimersCheckTradable")]
        [ProducesResponseType(typeof(CheckResultModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> CheckTradableAsync(string clientId, string lykkeEntityId1, string lykkeEntityId2)
        {
            try
            {
                bool requiresApproval =
                    await _clientDisclaimerService.CheckTradableAsync(clientId, lykkeEntityId1, lykkeEntityId2);

                return Ok(new CheckResultModel {RequiresApproval = requiresApproval});
            }
            catch (LykkeEntityNotFoundException exception)
            {
                await _log.WriteErrorAsync(nameof(ClientDisclaimersController), nameof(CheckTradableAsync),
                    new
                    {
                        clientId,
                        lykkeEntityId1,
                        lykkeEntityId2
                    }.ToJson(), exception);
                return NotFound(ErrorResponse.Create(exception.Message));
            }
        }
        
        /// <summary>
        /// Finds an actual for today deposit disclaimer by Lykke entity and requires client approval if it has not yet been approved.
        /// </summary>
        /// <param name="clientId">The client id.</param>
        /// <param name="lykkeEntityId">The Lykke entity id.</param>
        /// <returns>A check result.</returns>
        /// <response code="200">A check result.</response>
        /// <response code="404">The Lykke entity not found.</response>
        [HttpPost]
        [Route("clients/{clientId}/disclaimers/deposit")]
        [SwaggerOperation("ClientDisclaimersCheckDeposit")]
        [ProducesResponseType(typeof(CheckResultModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> CheckDepositAsync(string clientId, string lykkeEntityId)
        {
            try
            {
                bool requiresApproval =
                    await _clientDisclaimerService.CheckDepositAsync(clientId, lykkeEntityId);

                return Ok(new CheckResultModel {RequiresApproval = requiresApproval});
            }
            catch (LykkeEntityNotFoundException exception)
            {
                await _log.WriteErrorAsync(nameof(ClientDisclaimersController), nameof(CheckDepositAsync),
                    new
                    {
                        clientId,
                        lykkeEntityId
                    }.ToJson(), exception);
                return NotFound(ErrorResponse.Create(exception.Message));
            }
        }
        
        /// <summary>
        /// Finds an actual for today withdrawal disclaimer by Lykke entity and requires client approval if it has not yet been approved.
        /// </summary>
        /// <param name="clientId">The client id.</param>
        /// <param name="lykkeEntityId">The Lykke entity id.</param>
        /// <returns>A check result.</returns>
        /// <response code="200">A check result.</response>
        /// <response code="404">The Lykke entity not found.</response>
        [HttpPost]
        [Route("clients/{clientId}/disclaimers/withdrawal")]
        [SwaggerOperation("ClientDisclaimersCheckWithdrawal")]
        [ProducesResponseType(typeof(CheckResultModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> CheckWithdrawalAsync(string clientId, string lykkeEntityId)
        {
            try
            {
                bool requiresApproval =
                    await _clientDisclaimerService.CheckWithdrawalAsync(clientId, lykkeEntityId);

                return Ok(new CheckResultModel {RequiresApproval = requiresApproval});
            }
            catch (LykkeEntityNotFoundException exception)
            {
                await _log.WriteErrorAsync(nameof(ClientDisclaimersController), nameof(CheckWithdrawalAsync),
                    new
                    {
                        clientId,
                        lykkeEntityId
                    }.ToJson(), exception);
                return NotFound(ErrorResponse.Create(exception.Message));
            }
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
                await _log.WriteWarningAsync(nameof(ClientDisclaimersController), nameof(ApproveAsync),
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
