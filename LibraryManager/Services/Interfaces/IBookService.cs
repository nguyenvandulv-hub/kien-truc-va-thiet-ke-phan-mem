using LibraryManager.Models.DTOs;

namespace LibraryManager.Services.Interfaces
{
    public interface IBookService
    {
        Task<IEnumerable<BookDto>> GetAllAsync();
        Task<PagedResultDto<BookDto>> GetPagedAsync(int page, int pageSize);
        Task<BookDto?> GetByIdAsync(int id);
        Task<PagedResultDto<BookDto>> SearchAsync(string keyword, int page = 1, int pageSize = 10);
        Task<BookDto> CreateAsync(BookCreateDto dto);
        Task<BookDto?> UpdateAsync(int id, BookUpdateDto dto);
        Task<(bool Success, string Message)> DeleteAsync(int id);
    }
}
