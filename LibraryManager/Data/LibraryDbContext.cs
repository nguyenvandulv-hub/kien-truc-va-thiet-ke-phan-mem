using LibraryManager.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace LibraryManager.Data
{
    public class LibraryDbContext : DbContext
    {
        public LibraryDbContext(DbContextOptions<LibraryDbContext> options) : base(options) { }

        public DbSet<Book> Books { get; set; }
        public DbSet<Borrower> Borrowers { get; set; }
        public DbSet<BorrowRecord> BorrowRecords { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ── Book ────────────────────────────────────────────────
            modelBuilder.Entity<Book>(entity =>
            {
                entity.ToTable("books");
                entity.HasKey(b => b.Id);

                entity.Property(b => b.Title).IsRequired().HasMaxLength(255);
                entity.Property(b => b.Author).IsRequired().HasMaxLength(150);
                entity.Property(b => b.ISBN).HasMaxLength(20);
                entity.Property(b => b.Quantity).HasDefaultValue(0);
                entity.Property(b => b.AvailableQuantity).HasDefaultValue(0);
                entity.Property(b => b.IsActive).HasDefaultValue(true);
                entity.Property(b => b.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasIndex(b => b.Title).HasDatabaseName("idx_books_title");
                entity.HasIndex(b => b.ISBN).IsUnique().HasDatabaseName("idx_books_isbn");
            });

            // ── Borrower ─────────────────────────────────────────────
            modelBuilder.Entity<Borrower>(entity =>
            {
                entity.ToTable("borrowers");
                entity.HasKey(b => b.Id);

                entity.Property(b => b.FullName).IsRequired().HasMaxLength(100);
                entity.Property(b => b.Email).HasMaxLength(100);
                entity.Property(b => b.Phone).HasMaxLength(20);
                entity.Property(b => b.IsActive).HasDefaultValue(true);
                entity.Property(b => b.MembershipDate).HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(b => b.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasIndex(b => b.FullName).HasDatabaseName("idx_borrowers_fullname");
                entity.HasIndex(b => b.Email).IsUnique().HasDatabaseName("idx_borrowers_email");
            });

            // ── BorrowRecord ──────────────────────────────────────────
            modelBuilder.Entity<BorrowRecord>(entity =>
            {
                entity.ToTable("borrow_records");
                entity.HasKey(r => r.Id);

                entity.Property(r => r.BorrowDate).HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(r => r.IsReturned).HasDefaultValue(false);
                entity.Property(r => r.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

                // FK: BorrowRecord → Book
                entity.HasOne(r => r.Book)
                      .WithMany(b => b.BorrowRecords)
                      .HasForeignKey(r => r.BookId)
                      .OnDelete(DeleteBehavior.Restrict);

                // FK: BorrowRecord → Borrower
                entity.HasOne(r => r.Borrower)
                      .WithMany(b => b.BorrowRecords)
                      .HasForeignKey(r => r.BorrowerId)
                      .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
