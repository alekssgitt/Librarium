using Librarium.Librarium.Application.DTO;
using Librarium.Librarium.Application.Interfaces;
using Librarium.Librarium.Data.Entities;

namespace Librarium.Librarium.Application.Services;

public class LibraryService(ILibraryRepository repository) : ILibraryService
{
    public async Task<IReadOnlyList<BookResponse>> GetBooksAsync(CancellationToken cancellationToken = default)
    {
        var books = await repository.GetBooksAsync(cancellationToken);

        return books
            .Select(book => new BookResponse
            {
                BookId = book.Id,
                Title = book.Title,
                Isbn = book.ISBN,
                PublicationYear = book.PublicationYear,
                Authors = book.Authors
                    .OrderBy(a => a.LastName)
                    .ThenBy(a => a.FirstName)
                    .Select(author => new AuthorResponse
                    {
                        AuthorId = author.Id,
                        FirstName = author.FirstName,
                        LastName = author.LastName,
                        Biography = author.Biography
                    })
                    .ToList()
            })
            .ToList();
    }

    public Task<IReadOnlyList<Member>> GetMembersAsync(CancellationToken cancellationToken = default)
    {
        return repository.GetMembersAsync(cancellationToken);
    }

    public async Task<Loan> CreateLoanAsync(CreateLoanRequest request, CancellationToken cancellationToken = default)
    {
        if (!await repository.BookExistsAsync(request.BookId, cancellationToken))
        {
            throw new KeyNotFoundException($"Book with id {request.BookId} was not found.");
        }

        if (!await repository.MemberExistsAsync(request.MemberId, cancellationToken))
        {
            throw new KeyNotFoundException($"Member with id {request.MemberId} was not found.");
        }

        var loan = new Loan
        {
            BookId = request.BookId,
            MemberId = request.MemberId,
            LoanDate = request.LoanDate ?? DateTime.UtcNow
        };

        return await repository.CreateLoanAsync(loan, cancellationToken);
    }

    public Task<IReadOnlyList<Loan>> GetLoansForMemberAsync(int memberId, CancellationToken cancellationToken = default)
    {
        return repository.GetLoansForMemberAsync(memberId, cancellationToken);
    }
}