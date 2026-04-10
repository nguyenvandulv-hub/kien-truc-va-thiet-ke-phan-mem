using LibraryManager.Models.Entities;

namespace LibraryManager.Repositories.Interfaces
{
    public interface IBorrowRecordRepository
    {
        Task<IEnumerable<BorrowRecord>> GetAllAsync();
        Task<IEnumerable<BorrowRecord>> GetPagedAsync(int page, int pageSize);
        Task<int> GetCountAsync();
        Task<BorrowRecord?> GetByIdAsync(int id);
        Task<IEnumerable<BorrowRecord>> GetByBorrowerIdAsync(int borrowerId);
        Task<IEnumerable<BorrowRecord>> GetByBookIdAsync(int bookId);
        Task<IEnumerable<BorrowRecord>> GetOverdueAsync();
        Task<IEnumerable<BorrowRecord>> GetOverduePagedAsync(int page, int pageSize);
        Task<int> GetOverdueCountAsync();
        Task<IEnumerable<BorrowRecord>> GetActiveAsync();
        Task<bool> AnyUnreturnedByBookIdAsync(int bookId);
        Task<bool> AnyUnreturnedByBorrowerIdAsync(int borrowerId);
        Task<BorrowRecord> CreateAsync(BorrowRecord record);
        Task<BorrowRecord> UpdateAsync(BorrowRecord record);
    }
}
