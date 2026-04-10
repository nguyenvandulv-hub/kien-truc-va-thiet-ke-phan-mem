using LibraryManager.Models.DTOs;

namespace LibraryManager.Services.Interfaces
{
    public interface IBorrowerService
    {
        Task<IEnumerable<BorrowerDto>> GetAllAsync();
        Task<PagedResultDto<BorrowerDto>> GetPagedAsync(int page, int pageSize);
        Task<BorrowerDto?> GetByIdAsync(int id);
        Task<PagedResultDto<BorrowerDto>> SearchAsync(string keyword, int page = 1, int pageSize = 10);
        Task<BorrowerDto> CreateAsync(BorrowerCreateDto dto);
        Task<BorrowerDto?> UpdateAsync(int id, BorrowerUpdateDto dto);
        Task<(bool Success, string Message)> DeleteAsync(int id);
    }
}
