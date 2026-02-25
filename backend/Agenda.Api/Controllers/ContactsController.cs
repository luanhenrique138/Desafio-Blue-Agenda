using Microsoft.AspNetCore.Mvc;
using Agenda.Application.DTOs.Requests;
using Agenda.Application.DTOs.Responses;
using Agenda.Application.Services;

namespace Agenda.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContactsController : ControllerBase
    {
        private readonly IContactService _contactService;
        public ContactsController(IContactService contactService) 
        {
            _contactService = contactService;
        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(ContactResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ContactResponse>> GetById(Guid id) 
        {
            var result = await _contactService.GetByIdAsync(id);

            if (result is null)
            {
                return NotFound(new ProblemDetails
                {
                    Title = "Contato não encontrado",
                    Status = StatusCodes.Status400BadRequest,
                    Detail = $"Não existe contato com id '{id}'",
                    Type = "https://httpstatuses.com/404"

                });
            }

            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<ContactResponse>> Create(CreateContactRequest request) 
        {
            var result = await _contactService.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPut("{id:guid}")]
        [ProducesResponseType(typeof(ContactResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]

        public async Task<ActionResult<ContactResponse>> Update(Guid id,UpdateContactRequest request)
        {
            var result = await _contactService.UpdateAsync(id, request);
            if(result is null)
            {
                return NotFound(new ProblemDetails
                {
                    Title = "Contato não encontrado",
                    Status = StatusCodes.Status404NotFound,
                    Detail = $"Não existe contato com id '{id}'."
                });
            }
            
            return Ok(result);
        }

        [HttpDelete("{id:guid}")]
        [ProducesResponseType(typeof(ContactResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]

        public async Task<IActionResult> Delete(Guid id) 
        {
            var deleted = await _contactService.DeleteAsync(id);

            if (!deleted)
            {
                return NotFound(new ProblemDetails
                {
                    Title = "Contato não encontrado",
                    Status = StatusCodes.Status404NotFound,
                    Detail = $"Não existe contato com id '{id}'."
                });
            }

            return NoContent();
        }

        [HttpGet]
        public async Task<ActionResult<List<ContactResponse>>> GetContacts([FromQuery] string? search)
        {
            var result = await _contactService.GetAllAsync(search);
            return Ok(result);
        }
    }
}
