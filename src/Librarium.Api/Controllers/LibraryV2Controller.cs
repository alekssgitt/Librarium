using Librarium.Librarium.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Librarium.Librarium.Api.Controllers;

[ApiController]
[Route("api/v2/")]
public class LibraryV2Controller(ILibraryService service) : ControllerBase
{
    
    private const string GetAllLoansForMemberRoute = "get-loans/{memberId:int}";
    
    [HttpGet]
    [Route(GetAllLoansForMemberRoute)]
    public async Task<IActionResult> GetLoansForMember(int memberId, CancellationToken cancellationToken)
    {
        var loans = await service.GetLoansForMemberV2Async(memberId, cancellationToken);
        return Ok(loans);
    }
}
