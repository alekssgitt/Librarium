using Librarium.Librarium.Data.Entities;

namespace Librarium.Librarium.Application.Interfaces;

public interface ILibraryRepository
{
    Task<IReadOnlyList<Book>> GetBooksAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Member>> GetMembersAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Loan>> GetLoansForMemberAsync(int memberId, CancellationToken cancellationToken = default);
    Task<bool> BookExistsAsync(int bookId, CancellationToken cancellationToken = default);
    Task<bool> MemberExistsAsync(int memberId, CancellationToken cancellationToken = default);
    Task<Loan> CreateLoanAsync(Loan loan, CancellationToken cancellationToken = default);
}