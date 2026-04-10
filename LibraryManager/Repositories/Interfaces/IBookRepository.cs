using LibraryManager.Models.Entities;

namespace LibraryManager.Repositories.Interfaces
{
    public interface IBookRepository
    {
        Task<IEnumerable<Book>> GetAllAsync();
        Task<IEnumerable<Book>> GetPagedAsync(int page, int pageSize);
        Task<int> GetCountAsync();
        Task<Book?> GetByIdAsync(int id);
        Task<Book?> GetByISBNAsync(string isbn);
        Task<IEnumerable<Book>> SearchAsync(string keyword, int page, int pageSize);
        Task<int> GetSearchCountAsync(string keyword);
        Task<Book> CreateAsync(Book book);
        Task<Book> UpdateAsync(Book book);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}
