namespace Librarium.Librarium.Application.DTO;

public class LoanResponse
{
    public int LoanId { get; set; }
    public string BookTitle { get; set; } = null!;
    public DateTime LoanDate { get; set; }
    public DateTime? ReturnDate { get; set; }
}
