using Librarium.Librarium.Application.DTO;
using Librarium.Librarium.Data.Entities;

namespace Librarium.Librarium.Application.Interfaces;

public interface ILibraryService
{
    Task<IReadOnlyList<BookResponse>> GetBooksAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Member>> GetMembersAsync(CancellationToken cancellationToken = default);
    Task<Loan> CreateLoanAsync(CreateLoanRequest request, CancellationToken cancellationToken = default);
    Task<bool> RetireBookAsync(int bookId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<LoanResponse>> GetLoansForMemberAsync(int memberId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<LoanV2Response>> GetLoansForMemberV2Async(int memberId, CancellationToken cancellationToken = default);
}