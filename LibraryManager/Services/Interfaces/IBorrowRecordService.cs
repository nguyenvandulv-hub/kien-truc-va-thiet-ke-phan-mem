using LibraryManager.Models.DTOs;

namespace LibraryManager.Services.Interfaces
{
    public interface IBorrowRecordService
    {
        Task<IEnumerable<BorrowRecordDto>> GetAllAsync();
        Task<PagedResultDto<BorrowRecordDto>> GetPagedAsync(int page, int pageSize);
        Task<BorrowRecordDto?> GetByIdAsync(int id);
        Task<IEnumerable<BorrowRecordDto>> GetByBorrowerIdAsync(int borrowerId);
        Task<IEnumerable<BorrowRecordDto>> GetByBookIdAsync(int bookId);
        Task<IEnumerable<BorrowRecordDto>> GetOverdueAsync();
        Task<PagedResultDto<BorrowRecordDto>> GetOverduePagedAsync(int page, int pageSize);
        Task<IEnumerable<BorrowRecordDto>> GetActiveAsync();
        Task<(bool Success, string Message, BorrowRecordDto? Data)> BorrowBookAsync(BorrowCreateDto dto);
        Task<(bool Success, string Message, BorrowRecordDto? Data)> ReturnBookAsync(int recordId, ReturnBookDto dto);
    }
}
