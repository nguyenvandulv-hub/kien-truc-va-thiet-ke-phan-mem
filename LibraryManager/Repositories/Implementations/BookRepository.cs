using LibraryManager.Data;
using LibraryManager.Models.Entities;
using LibraryManager.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LibraryManager.Repositories.Implementations
{
    public class BookRepository : IBookRepository
    {
        private readonly LibraryDbContext _context;

        public BookRepository(LibraryDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Book>> GetAllAsync()
        {
            return await _context.Books
                .Where(b => b.IsActive)
                .OrderBy(b => b.Title)
                .ToListAsync();
        }

        public async Task<IEnumerable<Book>> GetPagedAsync(int page, int pageSize)
        {
            return await _context.Books
                .Where(b => b.IsActive)
                .OrderBy(b => b.Title)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> GetCountAsync()
        {
            return await _context.Books
                .CountAsync(b => b.IsActive);
        }

        public async Task<Book?> GetByIdAsync(int id)
        {
            return await _context.Books.FindAsync(id);
        }

        public async Task<Book?> GetByISBNAsync(string isbn)
        {
            return await _context.Books
                .FirstOrDefaultAsync(b => b.ISBN == isbn);
        }

        public async Task<IEnumerable<Book>> SearchAsync(string keyword, int page, int pageSize)
        {
            var lower = keyword.ToLower();
            return await _context.Books
                .Where(b => b.IsActive &&
                           (b.Title.ToLower().Contains(lower) ||
                            b.Author.ToLower().Contains(lower) ||
                            (b.ISBN != null && b.ISBN.Contains(lower))))
                .OrderBy(b => b.Title)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> GetSearchCountAsync(string keyword)
        {
            var lower = keyword.ToLower();
            return await _context.Books
                .CountAsync(b => b.IsActive &&
                                (b.Title.ToLower().Contains(lower) ||
                                 b.Author.ToLower().Contains(lower) ||
                                 (b.ISBN != null && b.ISBN.Contains(lower))));
        }

        public async Task<Book> CreateAsync(Book book)
        {
            _context.Books.Add(book);
            await _context.SaveChangesAsync();
            return book;
        }

        public async Task<Book> UpdateAsync(Book book)
        {
            book.UpdatedAt = DateTime.UtcNow;
            _context.Books.Update(book);
            await _context.SaveChangesAsync();
            return book;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null) return false;

            // Soft delete
            book.IsActive = false;
            book.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Books.AnyAsync(b => b.Id == id);
        }
    }
}
