using MediatR;
using Microsoft.AspNetCore.Mvc;
using ReportService.Application;

namespace ReportService.Api.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class ReportController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ReportController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _mediator.Send(new GetAllReportsQuery());

            if (!result.IsSuccess)
                return BadRequest(new { Error = result.Error });

            return Ok(result.Value);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _mediator.Send(new GetReportByIdQuery(id));

            if (!result.IsSuccess)
                return NotFound(new { Error = result.Error });

            return Ok(result.Value);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateReportDto dto)
        {
            var result = await _mediator.Send(new CreateReportCommand(dto));

            if (!result.IsSuccess)
                return BadRequest(new { Error = result.Error });

            return CreatedAtAction(nameof(GetById), new { id = result.Value }, result.Value);
        }
    }
}
