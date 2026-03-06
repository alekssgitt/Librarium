namespace Librarium.Librarium.Application.DTO;

public class BookResponse
{
    public int BookId { get; set; }
    public string Title { get; set; } = null!;
    public string? Isbn { get; set; }
    public int PublicationYear { get; set; }
    public IReadOnlyList<AuthorResponse> Authors { get; set; } = [];
}
