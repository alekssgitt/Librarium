namespace Librarium.Librarium.Application.DTO;

public class LoanV2Response
{
    public int LoanId { get; set; }
    public string BookTitle { get; set; } = null!;
    public DateTime LoanDate { get; set; }
    public DateTime? ReturnDate { get; set; }
    public string Status { get; set; } = null!;
}
