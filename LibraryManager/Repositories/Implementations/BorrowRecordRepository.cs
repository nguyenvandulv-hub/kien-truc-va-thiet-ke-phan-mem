using LibraryManager.Data;
using LibraryManager.Models.Entities;
using LibraryManager.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LibraryManager.Repositories.Implementations
{
    public class BorrowRecordRepository : IBorrowRecordRepository
    {
        private readonly LibraryDbContext _context;

        public BorrowRecordRepository(LibraryDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<BorrowRecord>> GetAllAsync()
        {
            return await _context.BorrowRecords
                .Include(r => r.Book)
                .Include(r => r.Borrower)
                .OrderByDescending(r => r.BorrowDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<BorrowRecord>> GetPagedAsync(int page, int pageSize)
        {
            return await _context.BorrowRecords
                .Include(r => r.Book)
                .Include(r => r.Borrower)
                .OrderByDescending(r => r.BorrowDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> GetCountAsync()
        {
            return await _context.BorrowRecords.CountAsync();
        }

        public async Task<BorrowRecord?> GetByIdAsync(int id)
        {
            return await _context.BorrowRecords
                .Include(r => r.Book)
                .Include(r => r.Borrower)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<IEnumerable<BorrowRecord>> GetByBorrowerIdAsync(int borrowerId)
        {
            return await _context.BorrowRecords
                .Include(r => r.Book)
                .Include(r => r.Borrower)
                .Where(r => r.BorrowerId == borrowerId)
                .OrderByDescending(r => r.BorrowDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<BorrowRecord>> GetByBookIdAsync(int bookId)
        {
            return await _context.BorrowRecords
                .Include(r => r.Book)
                .Include(r => r.Borrower)
                .Where(r => r.BookId == bookId)
                .OrderByDescending(r => r.BorrowDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<BorrowRecord>> GetOverdueAsync()
        {
            return await _context.BorrowRecords
                .Include(r => r.Book)
                .Include(r => r.Borrower)
                .Where(r => !r.IsReturned && r.DueDate < DateTime.UtcNow)
                .OrderBy(r => r.DueDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<BorrowRecord>> GetOverduePagedAsync(int page, int pageSize)
        {
            return await _context.BorrowRecords
                .Include(r => r.Book)
                .Include(r => r.Borrower)
                .Where(r => !r.IsReturned && r.DueDate < DateTime.UtcNow)
                .OrderBy(r => r.DueDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> GetOverdueCountAsync()
        {
            return await _context.BorrowRecords
                .CountAsync(r => !r.IsReturned && r.DueDate < DateTime.UtcNow);
        }

        public async Task<IEnumerable<BorrowRecord>> GetActiveAsync()
        {
            return await _context.BorrowRecords
                .Include(r => r.Book)
                .Include(r => r.Borrower)
                .Where(r => !r.IsReturned)
                .OrderBy(r => r.DueDate)
                .ToListAsync();
        }

        public async Task<bool> AnyUnreturnedByBookIdAsync(int bookId)
        {
            return await _context.BorrowRecords
                .AnyAsync(r => r.BookId == bookId && !r.IsReturned);
        }

        public async Task<bool> AnyUnreturnedByBorrowerIdAsync(int borrowerId)
        {
            return await _context.BorrowRecords
                .AnyAsync(r => r.BorrowerId == borrowerId && !r.IsReturned);
        }

        public async Task<BorrowRecord> CreateAsync(BorrowRecord record)
        {
            _context.BorrowRecords.Add(record);
            await _context.SaveChangesAsync();
            return record;
        }

        public async Task<BorrowRecord> UpdateAsync(BorrowRecord record)
        {
            _context.BorrowRecords.Update(record);
            await _context.SaveChangesAsync();
            return record;
        }
    }
}
