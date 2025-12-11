using api_core.DTO.Requests;
using Application.Interfaces;
using Application.UseCases.ProcessAndStoreSapData;
using Application.UseCases.ProcessSapData;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace api_core.Controllers
{
    [ApiController]
    [Route("api/process")]
    public class ProcessController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProcessController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("store")]
        public async Task<IActionResult> ProcessAndStore([FromBody] ProcessAndStoreRequest request)
        {
            var command = new ProcessAndStoreSapDataCommand(
                request.InputPath,
                request.SaveToDatabase,
                request.GenerateExcel,
                request.OutputPath);

            var result = await _mediator.Send(command);

            return result.Success
                ? Ok(result)  // Devuelve directamente el resultado
                : BadRequest(result);
        }

    }

}
