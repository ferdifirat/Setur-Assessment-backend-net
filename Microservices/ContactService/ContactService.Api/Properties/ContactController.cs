using ContactService.Application;
using ContactService.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ContactService.Api.Properties
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class ContactController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ContactController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateContactDto dto)
        {
            var result = await _mediator.Send(new CreateContactCommand(dto, User.Identity?.Name));

            if (!result.IsSuccess)
                return BadRequest(new { Error = result.Error });

            return CreatedAtAction(nameof(GetById), new { id = result.Value }, result.Value);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _mediator.Send(new GetContactByIdQuery(id));

            if (!result.IsSuccess)
                return NotFound(new { Error = result.Error });

            return Ok(result.Value);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _mediator.Send(new GetAllContactsQuery());

            if (!result.IsSuccess)
                return BadRequest(new { Error = result.Error });

            return Ok(result.Value);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] CreateContactDto dto)
        {
            var result = await _mediator.Send(new UpdateContactCommand(id, dto, User.Identity?.Name));

            if (!result.IsSuccess)
                return NotFound(new { Error = result.Error });

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _mediator.Send(new DeleteContactCommand(id, User.Identity?.Name));

            if (!result.IsSuccess)
                return NotFound(new { Error = result.Error });

            return NoContent();
        }

        [HttpPost("information")]
        public async Task<IActionResult> AddContactInformation([FromBody] CreateContactInformationDto dto)
        {
            var result = await _mediator.Send(new CreateContactInformationCommand(dto));

            if (!result.IsSuccess)
                return BadRequest(new { Error = result.Error });

            return CreatedAtAction(nameof(GetById), new { id = dto.ContactId }, result.Value);
        }

        [HttpDelete("information/{id}")]
        public async Task<IActionResult> DeleteContactInformation(Guid id)
        {
            var result = await _mediator.Send(new DeleteContactInformationCommand(id));

            if (!result.IsSuccess)
                return NotFound(new { Error = result.Error });

            return NoContent();
        }

        [HttpPost("reports/request")]
        public async Task<IActionResult> RequestReport()
        {
           // event will added

            return Accepted();
        }
    }
}
