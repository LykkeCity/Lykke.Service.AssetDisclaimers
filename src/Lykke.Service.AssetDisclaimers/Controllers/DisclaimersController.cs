using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Common;
using Common.Log;
using Lykke.Common.Api.Contract.Responses;
using Lykke.Service.AssetDisclaimers.Core.Domain;
using Lykke.Service.AssetDisclaimers.Core.Exceptions;
using Lykke.Service.AssetDisclaimers.Core.Services;
using Lykke.Service.AssetDisclaimers.Extensions;
using Lykke.Service.AssetDisclaimers.Models.Disclaimers;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Lykke.Service.AssetDisclaimers.Controllers
{
    [Route("api")]
    public class DisclaimersController : Controller
    {
        private readonly IDisclaimerService _disclaimerService;
        private readonly ILog _log;

        public DisclaimersController(
            IDisclaimerService disclaimerService,
            ILog log)
        {
            _disclaimerService = disclaimerService;
            _log = log;
        }
        
        /// <summary>
        /// Returns a collection of Lykke entity disclaimers.
        /// </summary>
        /// <param name="lykkeEntityId">The Lykke entity id.</param>
        /// <returns>A collection of Lykke entity disclaimers.</returns>
        /// <response code="200">A collection of Lykke entity disclaimers.</response>
        [HttpGet]
        [Route("lykkeentities/{lykkeEntityId}/disclaimers")]
        [SwaggerOperation("DisclaimersGetAll")]
        [ProducesResponseType(typeof(List<DisclaimerModel>), (int) HttpStatusCode.OK)]
        public async Task<IActionResult> GetAsync(string lykkeEntityId)
        {
            IReadOnlyList<IDisclaimer> disclaimers = await _disclaimerService.GetAsync(lykkeEntityId);

            var model = Mapper.Map<List<DisclaimerModel>>(disclaimers);

            return Ok(model);
        }

        /// <summary>
        /// Returns a Lykke entity disclaimer.
        /// </summary>
        /// <param name="lykkeEntityId">The Lykke entity id.</param>
        /// <param name="disclaimerId">The disclaimer id.</param>
        /// <returns>A Lykke entity disclaimer.</returns>
        /// <response code="200">A Lykke entity disclaimer.</response>
        /// <response code="404">Lykke entity disclaimer not found.</response>
        [HttpGet]
        [Route("lykkeentities/{lykkeEntityId}/disclaimers/{disclaimerId}")]
        [SwaggerOperation("DisclaimersGetById")]
        [ProducesResponseType(typeof(DisclaimerModel), (int) HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetAsync(string lykkeEntityId, string disclaimerId)
        {
            IDisclaimer disclaimer = await _disclaimerService.GetAsync(lykkeEntityId, disclaimerId);

            if (disclaimer == null)
                return NotFound();

            var model = Mapper.Map<DisclaimerModel>(disclaimer);

            return Ok(model);
        }

        /// <summary>
        /// Creates a Lykke entity disclaimer.
        /// </summary>
        /// <param name="model">The Lykke entity disclaimer creation information.</param>
        /// <returns>The Lykke entity disclaimer.</returns>
        /// <response code="204">The Lykke entity disclaimer.</response>
        /// <response code="400">Invalid model.</response>
        /// <response code="404">Lykke entity not found.</response>
        [HttpPost]
        [Route("lykkeentities/disclaimers")]
        [SwaggerOperation("DisclaimersAdd")]
        [ProducesResponseType(typeof(DisclaimerModel), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int) HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), (int) HttpStatusCode.NotFound)]
        public async Task<IActionResult> AddAsync([FromBody] CreateDisclaimerModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ErrorResponse().AddErrors(ModelState));

            IDisclaimer createDisclaimer;
            
            try
            {
                var disclaimer = Mapper.Map<Disclaimer>(model);

                createDisclaimer = await _disclaimerService.AddAsync(disclaimer);
            }
            catch (LykkeEntityNotFoundException exception)
            {
                await _log.WriteErrorAsync(nameof(DisclaimersController), nameof(AddAsync),
                    model.ToJson(), exception);
                return NotFound(ErrorResponse.Create(exception.Message));
            }

            return Ok(Mapper.Map<DisclaimerModel>(createDisclaimer));
        }
        
        /// <summary>
        /// Updates a Lykke entity disclaimer.
        /// </summary>
        /// <param name="model">The Lykke entity disclaimer update information.</param>
        /// <returns>No content.</returns>
        /// <response code="204">Lykke entity disclaimer successfully updated.</response>
        /// <response code="400">Invalid model.</response>
        /// <response code="404">Lykke entity not found.</response>
        [HttpPut]
        [Route("lykkeentities/disclaimers")]
        [SwaggerOperation("DisclaimersUpdate")]
        [ProducesResponseType((int) HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(ErrorResponse), (int) HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), (int) HttpStatusCode.NotFound)]
        public async Task<IActionResult> UpdateAsync([FromBody] EditDisclaimerModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ErrorResponse().AddErrors(ModelState));

            try
            {
                var disclaimer = Mapper.Map<Disclaimer>(model);

                await _disclaimerService.UpdateAsync(disclaimer);
            }
            catch (DisclaimerNotFoundException exception)
            {
                await _log.WriteErrorAsync(nameof(DisclaimersController), nameof(UpdateAsync),
                    model.ToJson(), exception);
                return NotFound(ErrorResponse.Create(exception.Message));
            }

            return NoContent();
        }
        
        /// <summary>
        /// Deletes a Lykke entity disclaimer.
        /// </summary>
        /// <param name="lykkeEntityId">The Lykke entity id.</param>
        /// <param name="disclaimerId">The disclaimer id.</param>
        /// <response code="204">Disclaimer successfully deleted.</response>
        /// <response code="400">Can not delete Lykke entity disclimer if it already approved by client.</response>
        [HttpDelete]
        [Route("lykkeentities/{lykkeEntityId}/disclaimers/{disclaimerId}")]
        [SwaggerOperation("EmployeesDelete")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> DeleteAsync(string lykkeEntityId, string disclaimerId)
        {
            try
            {
                await _disclaimerService.DeleteAsync(lykkeEntityId, disclaimerId);
            }
            catch (InvalidOperationException exception)
            {
                await _log.WriteErrorAsync(nameof(DisclaimersController), nameof(DeleteAsync),
                    new {lykkeEntityId}.ToJson(), exception);
                return BadRequest(ErrorResponse.Create(exception.Message));
            }

            return NoContent();
        }
    }
}
