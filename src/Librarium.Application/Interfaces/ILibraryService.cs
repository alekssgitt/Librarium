using Librarium.Librarium.Application.DTO;
using Librarium.Librarium.Data.Entities;

namespace Librarium.Librarium.Application.Interfaces;

public interface ILibraryService
{
    Task<IReadOnlyList<BookResponse>> GetBooksAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Member>> GetMembersAsync(CancellationToken cancellationToken = default);
    Task<Loan> CreateLoanAsync(CreateLoanRequest request, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Loan>> GetLoansForMemberAsync(int memberId, CancellationToken cancellationToken = default);
}