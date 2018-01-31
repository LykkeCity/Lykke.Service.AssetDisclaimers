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
using Lykke.Service.AssetDisclaimers.Models.LykkeEntities;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Lykke.Service.AssetDisclaimers.Controllers
{
    [Route("api")]
    public class LykkeEntitiesController : Controller
    {
        private readonly ILykkeEntityService _lykkeEntityService;
        private readonly ILog _log;

        public LykkeEntitiesController(
            ILykkeEntityService lykkeEntityService,
            ILog log)
        {
            _lykkeEntityService = lykkeEntityService;
            _log = log;
        }

        /// <summary>
        /// Returns a collection of Lykke entities.
        /// </summary>
        /// <returns>A collection of Lykke entities.</returns>
        /// <response code="200">A collection of Lykke entities.</response>
        [HttpGet]
        [Route("lykkeentities")]
        [SwaggerOperation("LykkeEntitiesGetAll")]
        [ProducesResponseType(typeof(List<LykkeEntityModel>), (int) HttpStatusCode.OK)]
        public async Task<IActionResult> GetAsync()
        {
            IReadOnlyList<ILykkeEntity> lykkeEntities = await _lykkeEntityService.GetAsync();

            var model = Mapper.Map<List<LykkeEntityModel>>(lykkeEntities);

            return Ok(model);
        }

        /// <summary>
        /// Returns a Lykke entity.
        /// </summary>
        /// <param name="lykkeEntityId">The Lykke entity id.</param>
        /// <returns>The Lykke entity.</returns>
        /// <response code="200">The Lykke entity.</response>
        /// <response code="404">The Lykke entity not found.</response>
        [HttpGet]
        [Route("lykkeentities/{lykkeEntityId}")]
        [SwaggerOperation("LykkeEntitiesGetById")]
        [ProducesResponseType(typeof(LykkeEntityModel), (int) HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetAsync(string lykkeEntityId)
        {
            ILykkeEntity lykkeEntity = await _lykkeEntityService.GetAsync(lykkeEntityId);

            if (lykkeEntity == null)
                return NotFound();

            var model = Mapper.Map<LykkeEntityModel>(lykkeEntity);

            return Ok(model);
        }

        /// <summary>
        /// Adds a Lykke entity.
        /// </summary>
        /// <param name="model">The Lykke entity creation information.</param>
        /// <returns>The created Lykke entity.</returns>
        /// <response code="200">The Lykke entity.</response>
        /// <response code="400">Invalid model.</response>
        [HttpPost]
        [Route("lykkeentities")]
        [SwaggerOperation("LykkeEntitiesAdd")]
        [ProducesResponseType(typeof(LykkeEntityModel), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int) HttpStatusCode.BadRequest)]
        public async Task<IActionResult> AddAsync([FromBody] CreateLykkeEntityModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ErrorResponse().AddErrors(ModelState));

            try
            {
                var lykkeEntity = Mapper.Map<LykkeEntity>(model);

                ILykkeEntity createdLykkeEntity = await _lykkeEntityService.AddAsync(lykkeEntity);
                
                return Ok(Mapper.Map<LykkeEntityModel>(createdLykkeEntity));
            }
            catch (LykkeEntityAlreadyExistsException exception)
            {
                await _log.WriteErrorAsync(nameof(LykkeEntitiesController), nameof(AddAsync),
                    model.ToJson(), exception);
                return BadRequest(ErrorResponse.Create(exception.Message));
            }
        }

        /// <summary>
        /// Updates a Lykke entity.
        /// </summary>
        /// <param name="model">The Lykke entity update information.</param>
        /// <returns>No content.</returns>
        /// <response code="204">Lykke entity successfully updated.</response>
        /// <response code="400">Invalid model.</response>
        /// <response code="404">Lykke entity not found.</response>
        [HttpPut]
        [Route("lykkeentities")]
        [SwaggerOperation("LykkeEntitiesUpdate")]
        [ProducesResponseType((int) HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(ErrorResponse), (int) HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), (int) HttpStatusCode.NotFound)]
        public async Task<IActionResult> UpdateAsync([FromBody] EditLykkeEntityModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ErrorResponse().AddErrors(ModelState));

            try
            {
                var lykkeEntity = Mapper.Map<LykkeEntity>(model);

                await _lykkeEntityService.UpdateAsync(lykkeEntity);
            }
            catch (LykkeEntityNotFoundException exception)
            {
                await _log.WriteErrorAsync(nameof(LykkeEntitiesController), nameof(UpdateAsync),
                    model.ToJson(), exception);
                return NotFound(ErrorResponse.Create(exception.Message));
            }

            return NoContent();
        }

        /// <summary>
        /// Deletes a Lykke entity.
        /// </summary>
        /// <returns>No content.</returns>
        /// <param name="lykkeEntityId">The Lykke entity id.</param>
        /// <response code="204">Lykke entity successfully deleted.</response>
        /// <response code="400">Can not delete Lykke entity if one or more disclimers exists.</response>
        [HttpDelete]
        [Route("lykkeentities/{lykkeEntityId}")]
        [SwaggerOperation("LykkeEntitiesDelete")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> DeleteAsync(string lykkeEntityId)
        {
            try
            {
                await _lykkeEntityService.DeleteAsync(lykkeEntityId);
            }
            catch (InvalidOperationException exception)
            {
                await _log.WriteErrorAsync(nameof(LykkeEntitiesController), nameof(DeleteAsync),
                    new {lykkeEntityId}.ToJson(), exception);
                return BadRequest(ErrorResponse.Create(exception.Message));
            }

            return NoContent();
        }
    }
}
