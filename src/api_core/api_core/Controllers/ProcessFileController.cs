using api_core.DTO.Requests;
using api_core.DTO.Responses;
using Application.UseCases.ProcessSapData;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace api_core.Controllers
{
    [ApiController]
    [Route("api/files")]
    public class ProcessFileController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProcessFileController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("process-sap")]
        [ProducesResponseType(typeof(ProcessSapResponse), 200)]
        [ProducesResponseType(typeof(ValidationProblemDetails), 400)]
        public async Task<IActionResult> ProcessSapFile(
            [FromBody] ProcessSapRequest request)
        {
            // Convertir DTO a Command
            var command = new ProcessSapDataCommand(
                request.InputPath,
                request.OutputPath);

            // Ejecutar el caso de uso a través de MediatR
            var result = await _mediator.Send(command);

            // Convertir Result a Response DTO
            var response = new ProcessSapResponse(
                Success: result.Success,
                Message: result.Message,
                RecordsProcessed: result.RecordsProcessed,
                OutputPath: request.OutputPath,
                ProcessedAt: DateTime.UtcNow);

            return result.Success
                ? Ok(response)
                : BadRequest(response);
        }
    }
}
