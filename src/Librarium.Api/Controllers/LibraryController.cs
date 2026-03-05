using Librarium.Librarium.Application.DTO;
using Librarium.Librarium.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Librarium.Librarium.Api.Controllers;

[ApiController]
[Route("api")]
public class LibraryController(ILibraryService service) : ControllerBase
{
    
    private const string GetAllBooksRoute = "get-books";
    private const string GetAllMembersRoute = "get-members";
    private const string CreateLoanRoute = "create-loan";
    private const string GetAllLoansForMemberRoute = "get-loans/{memberId:int}";
    
    
    
    [HttpGet]
    [Route(GetAllBooksRoute)]
    public async Task<IActionResult> GetBooks(CancellationToken cancellationToken)
    {
        var books = await service.GetBooksAsync(cancellationToken);
        return Ok(books);
    }

    [HttpGet]
    [Route(GetAllMembersRoute)]
    public async Task<IActionResult> GetMembers(CancellationToken cancellationToken)
    {
        var members = await service.GetMembersAsync(cancellationToken);
        return Ok(members);
    }

    [HttpPost]
    [Route(CreateLoanRoute)]
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
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet]
    [Route(GetAllLoansForMemberRoute)]
    public async Task<IActionResult> GetLoansForMember(int memberId, CancellationToken cancellationToken)
    {
        var loans = await service.GetLoansForMemberAsync(memberId, cancellationToken);
        return Ok(loans);
    }
}