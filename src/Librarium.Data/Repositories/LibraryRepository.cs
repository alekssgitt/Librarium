using Librarium.Librarium.Application.Interfaces;
using Librarium.Librarium.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Librarium.Librarium.Data.Repositories;

public class LibraryRepository(LibraryDbContext context) : ILibraryRepository
{
    public async Task<IReadOnlyList<Book>> GetBooksAsync(CancellationToken cancellationToken = default)
    {
        return await context.Books
            .AsNoTracking()
            .Include(x => x.Authors)
            .OrderBy(x => x.Title)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Member>> GetMembersAsync(CancellationToken cancellationToken = default)
    {
        return await context.Members
            .AsNoTracking()
            .OrderBy(x => x.LastName)
            .ThenBy(x => x.FirstName)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Loan>> GetLoansForMemberAsync(int memberId, CancellationToken cancellationToken = default)
    {
        return await context.Loans
            .AsNoTracking()
            .Where(x => x.MemberId == memberId)
            .Include(x => x.Book)
            .Include(x => x.Member)
            .OrderByDescending(x => x.LoanDate)
            .ToListAsync(cancellationToken);
    }

    public Task<bool> BookExistsAsync(int bookId, CancellationToken cancellationToken = default)
    {
        return context.Books.AnyAsync(x => x.Id == bookId, cancellationToken);
    }

    public Task<bool> MemberExistsAsync(int memberId, CancellationToken cancellationToken = default)
    {
        return context.Members.AnyAsync(x => x.Id == memberId, cancellationToken);
    }

    public async Task<Loan> CreateLoanAsync(Loan loan, CancellationToken cancellationToken = default)
    {
        context.Loans.Add(loan);
        await context.SaveChangesAsync(cancellationToken);
        return loan;
    }
}