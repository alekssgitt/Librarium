using Librarium.Librarium.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Librarium.Librarium.Data;

public class LibraryDbContext(DbContextOptions<LibraryDbContext> options) : DbContext(options)
{
    public DbSet<Book> Books => Set<Book>();
    public DbSet<Member> Members => Set<Member>();
    public DbSet<Loan> Loans => Set<Loan>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Book>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Title).IsRequired().HasMaxLength(256);
            entity.Property(x => x.ISBN).IsRequired().HasMaxLength(32);
            entity.HasIndex(x => x.ISBN).IsUnique();
            entity.Property(x => x.PublicationYear).IsRequired();
        });

        modelBuilder.Entity<Member>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.FirstName).IsRequired().HasMaxLength(128);
            entity.Property(x => x.LastName).IsRequired().HasMaxLength(128);
            entity.Property(x => x.Email).IsRequired().HasMaxLength(256);
            entity.HasIndex(x => x.Email).IsUnique();
        });

        modelBuilder.Entity<Loan>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.LoanDate).IsRequired();

            entity.HasOne(x => x.Book)
                .WithMany(x => x.Loans)
                .HasForeignKey(x => x.BookId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.Member)
                .WithMany(x => x.Loans)
                .HasForeignKey(x => x.MemberId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}