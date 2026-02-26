using Librarium.Librarium.Application.DTO;
using Librarium.Librarium.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Librarium.Librarium.Api.Controllers;

[ApiController]
[Route("api")]
public class LibraryController(ILibraryService service) : ControllerBase
{
    [HttpGet("books")]
    public async Task<IActionResult> GetBooks(CancellationToken cancellationToken)
    {
        var books = await service.GetBooksAsync(cancellationToken);
        return Ok(books);
    }

    [HttpGet("members")]
    public async Task<IActionResult> GetMembers(CancellationToken cancellationToken)
    {
        var members = await service.GetMembersAsync(cancellationToken);
        return Ok(members);
    }

    [HttpPost("loans")]
    public async Task<IActionResult> CreateLoan([FromBody] CreateLoanRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var loan = await service.CreateLoanAsync(request, cancellationToken);
            return Ok(loan);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpGet("loans/{memberId:int}")]
    public async Task<IActionResult> GetLoansForMember(int memberId, CancellationToken cancellationToken)
    {
        var loans = await service.GetLoansForMemberAsync(memberId, cancellationToken);
        return Ok(loans);
    }
}