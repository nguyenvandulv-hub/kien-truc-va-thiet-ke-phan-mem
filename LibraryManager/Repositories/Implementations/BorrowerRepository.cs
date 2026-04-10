using LibraryManager.Data;
using LibraryManager.Models.Entities;
using LibraryManager.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LibraryManager.Repositories.Implementations
{
    public class BorrowerRepository : IBorrowerRepository
    {
        private readonly LibraryDbContext _context;

        public BorrowerRepository(LibraryDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Borrower>> GetAllAsync()
        {
            return await _context.Borrowers
                .Where(b => b.IsActive)
                .OrderBy(b => b.FullName)
                .ToListAsync();
        }

        public async Task<IEnumerable<Borrower>> GetPagedAsync(int page, int pageSize)
        {
            return await _context.Borrowers
                .Where(b => b.IsActive)
                .OrderBy(b => b.FullName)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> GetCountAsync()
        {
            return await _context.Borrowers
                .CountAsync(b => b.IsActive);
        }

        public async Task<Borrower?> GetByIdAsync(int id)
        {
            return await _context.Borrowers.FindAsync(id);
        }

        public async Task<Borrower?> GetByEmailAsync(string email)
        {
            return await _context.Borrowers
                .FirstOrDefaultAsync(b => b.Email == email);
        }

        public async Task<IEnumerable<Borrower>> SearchAsync(string keyword, int page, int pageSize)
        {
            var lower = keyword.ToLower();
            return await _context.Borrowers
                .Where(b => b.IsActive &&
                           (b.FullName.ToLower().Contains(lower) ||
                            (b.Email != null && b.Email.ToLower().Contains(lower)) ||
                            (b.Phone != null && b.Phone.Contains(lower))))
                .OrderBy(b => b.FullName)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> GetSearchCountAsync(string keyword)
        {
            var lower = keyword.ToLower();
            return await _context.Borrowers
                .CountAsync(b => b.IsActive &&
                                (b.FullName.ToLower().Contains(lower) ||
                                 (b.Email != null && b.Email.ToLower().Contains(lower)) ||
                                 (b.Phone != null && b.Phone.Contains(lower))));
        }

        public async Task<Borrower> CreateAsync(Borrower borrower)
        {
            _context.Borrowers.Add(borrower);
            await _context.SaveChangesAsync();
            return borrower;
        }

        public async Task<Borrower> UpdateAsync(Borrower borrower)
        {
            _context.Borrowers.Update(borrower);
            await _context.SaveChangesAsync();
            return borrower;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var borrower = await _context.Borrowers.FindAsync(id);
            if (borrower == null) return false;

            // Soft delete
            borrower.IsActive = false;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Borrowers.AnyAsync(b => b.Id == id);
        }
    }
}
