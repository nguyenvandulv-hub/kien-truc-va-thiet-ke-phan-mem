using LibraryManager.Models.DTOs;
using LibraryManager.Models.Entities;
using LibraryManager.Repositories.Interfaces;
using LibraryManager.Services.Interfaces;

namespace LibraryManager.Services.Implementations
{
    public class BorrowRecordService : IBorrowRecordService
    {
        private readonly IBorrowRecordRepository _recordRepo;
        private readonly IBookRepository         _bookRepo;
        private readonly IBorrowerRepository     _borrowerRepo;
        private readonly LibraryManager.Data.LibraryDbContext _context;

        public BorrowRecordService(
            IBorrowRecordRepository recordRepo,
            IBookRepository         bookRepo,
            IBorrowerRepository     borrowerRepo,
            LibraryManager.Data.LibraryDbContext context)
        {
            _recordRepo   = recordRepo;
            _bookRepo     = bookRepo;
            _borrowerRepo = borrowerRepo;
            _context      = context;
        }

        public async Task<IEnumerable<BorrowRecordDto>> GetAllAsync()
        {
            var records = await _recordRepo.GetAllAsync();
            return records.Select(ToDto);
        }

        public async Task<PagedResultDto<BorrowRecordDto>> GetPagedAsync(int page, int pageSize)
        {
            var totalCount = await _recordRepo.GetCountAsync();
            var records = await _recordRepo.GetPagedAsync(page, pageSize);

            return new PagedResultDto<BorrowRecordDto>
            {
                Items = records.Select(ToDto),
                TotalCount = totalCount,
                PageNumber = page,
                PageSize = pageSize
            };
        }

        public async Task<BorrowRecordDto?> GetByIdAsync(int id)
        {
            var record = await _recordRepo.GetByIdAsync(id);
            return record == null ? null : ToDto(record);
        }

        public async Task<IEnumerable<BorrowRecordDto>> GetByBorrowerIdAsync(int borrowerId)
        {
            var records = await _recordRepo.GetByBorrowerIdAsync(borrowerId);
            return records.Select(ToDto);
        }

        public async Task<IEnumerable<BorrowRecordDto>> GetByBookIdAsync(int bookId)
        {
            var records = await _recordRepo.GetByBookIdAsync(bookId);
            return records.Select(ToDto);
        }

        public async Task<IEnumerable<BorrowRecordDto>> GetOverdueAsync()
        {
            var records = await _recordRepo.GetOverdueAsync();
            return records.Select(ToDto);
        }

        public async Task<PagedResultDto<BorrowRecordDto>> GetOverduePagedAsync(int page, int pageSize)
        {
            var totalCount = await _recordRepo.GetOverdueCountAsync();
            var records = await _recordRepo.GetOverduePagedAsync(page, pageSize);

            return new PagedResultDto<BorrowRecordDto>
            {
                Items = records.Select(ToDto),
                TotalCount = totalCount,
                PageNumber = page,
                PageSize = pageSize
            };
        }

        public async Task<IEnumerable<BorrowRecordDto>> GetActiveAsync()
        {
            var records = await _recordRepo.GetActiveAsync();
            return records.Select(ToDto);
        }

        // ── Borrow Book (Business Logic with Transaction) ────────
        public async Task<(bool Success, string Message, BorrowRecordDto? Data)> BorrowBookAsync(BorrowCreateDto dto)
        {
            // Kiểm tra sách tồn tại
            var book = await _bookRepo.GetByIdAsync(dto.BookId);
            if (book == null || !book.IsActive)
                return (false, "Sách không tồn tại hoặc đã ngừng hoạt động.", null);

            // Kiểm tra còn sách để mượn không
            if (book.AvailableQuantity <= 0)
                return (false, $"Sách '{book.Title}' hiện không còn bản nào để mượn.", null);

            // Kiểm tra người mượn tồn tại
            var borrower = await _borrowerRepo.GetByIdAsync(dto.BorrowerId);
            if (borrower == null || !borrower.IsActive)
                return (false, "Người mượn không tồn tại hoặc đã bị vô hiệu hóa.", null);

            // Kiểm tra hạn trả hợp lệ
            if (dto.DueDate <= DateTime.UtcNow)
                return (false, "Ngày trả phải lớn hơn ngày hiện tại.", null);

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Giảm số lượng sách available
                book.AvailableQuantity--;
                book.UpdatedAt = DateTime.UtcNow;
                await _bookRepo.UpdateAsync(book);

                // Tạo bản ghi mượn
                var record = new BorrowRecord
                {
                    BookId     = dto.BookId,
                    BorrowerId = dto.BorrowerId,
                    BorrowDate = DateTime.UtcNow,
                    DueDate    = dto.DueDate,
                    IsReturned = false,
                    Notes      = dto.Notes,
                    CreatedAt  = DateTime.UtcNow
                };
                var created = await _recordRepo.CreateAsync(record);

                await transaction.CommitAsync();

                // Reload để có navigation properties
                var full = await _recordRepo.GetByIdAsync(created.Id);
                return (true, "Mượn sách thành công.", ToDto(full!));
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return (false, $"Lỗi hệ thống khi mượn sách: {ex.Message}", null);
            }
        }

        // ── Return Book (Business Logic with Transaction) ────────
        public async Task<(bool Success, string Message, BorrowRecordDto? Data)> ReturnBookAsync(int recordId, ReturnBookDto dto)
        {
            var record = await _recordRepo.GetByIdAsync(recordId);
            if (record == null)
                return (false, "Không tìm thấy bản ghi mượn sách.", null);

            if (record.IsReturned)
                return (false, "Sách này đã được trả rồi.", null);

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Cập nhật bản ghi
                record.IsReturned  = true;
                record.ReturnDate  = DateTime.UtcNow;
                if (dto.Notes != null) record.Notes = dto.Notes;
                await _recordRepo.UpdateAsync(record);

                // Tăng lại số lượng sách available
                var book = await _bookRepo.GetByIdAsync(record.BookId);
                if (book != null)
                {
                    book.AvailableQuantity = Math.Min(book.AvailableQuantity + 1, book.Quantity);
                    book.UpdatedAt = DateTime.UtcNow;
                    await _bookRepo.UpdateAsync(book);
                }

                await transaction.CommitAsync();
                return (true, "Trả sách thành công.", ToDto(record));
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return (false, $"Lỗi hệ thống khi trả sách: {ex.Message}", null);
            }
        }

        // ── Mapper ────────────────────────────────────────────────
        private static BorrowRecordDto ToDto(BorrowRecord r) => new()
        {
            Id           = r.Id,
            BookId       = r.BookId,
            BookTitle    = r.Book?.Title ?? string.Empty,
            BorrowerId   = r.BorrowerId,
            BorrowerName = r.Borrower?.FullName ?? string.Empty,
            BorrowDate   = r.BorrowDate,
            DueDate      = r.DueDate,
            ReturnDate   = r.ReturnDate,
            IsReturned   = r.IsReturned,
            Notes        = r.Notes,
            CreatedAt    = r.CreatedAt
        };
    }
}
