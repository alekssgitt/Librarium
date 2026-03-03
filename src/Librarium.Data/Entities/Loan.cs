namespace Librarium.Librarium.Data.Entities;

public class Loan
{
    public int Id { get; set; }
    public int BookId { get; set; }
    public int MemberId { get; set; }
    public DateTime LoanDate { get; set; }
    public DateTime? ReturnDate { get; set; }
    public string Status { get; set; } = "Active";

    public Book Book { get; set; } = null!;
    public Member Member { get; set; } = null!;
}