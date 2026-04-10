using LibraryManager.Models.Entities;

namespace LibraryManager.Repositories.Interfaces
{
    public interface IBorrowerRepository
    {
        Task<IEnumerable<Borrower>> GetAllAsync();
        Task<IEnumerable<Borrower>> GetPagedAsync(int page, int pageSize);
        Task<int> GetCountAsync();
        Task<Borrower?> GetByIdAsync(int id);
        Task<Borrower?> GetByEmailAsync(string email);
        Task<IEnumerable<Borrower>> SearchAsync(string keyword, int page, int pageSize);
        Task<int> GetSearchCountAsync(string keyword);
        Task<Borrower> CreateAsync(Borrower borrower);
        Task<Borrower> UpdateAsync(Borrower borrower);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}
