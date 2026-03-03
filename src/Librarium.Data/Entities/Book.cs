namespace Librarium.Librarium.Data.Entities;

public class Book
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public string ISBN { get; set; } = null!;
    public int PublicationYear { get; set; }

    public ICollection<Loan> Loans { get; set; } = new List<Loan>();
    public ICollection<Author> Authors { get; set; } = new List<Author>();
}