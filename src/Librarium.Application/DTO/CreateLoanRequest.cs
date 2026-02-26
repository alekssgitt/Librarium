namespace Librarium.Librarium.Application.DTO;

public class CreateLoanRequest
{
    public int BookId { get; set; }
    public int MemberId { get; set; }
    public DateTime? LoanDate { get; set; }
}
