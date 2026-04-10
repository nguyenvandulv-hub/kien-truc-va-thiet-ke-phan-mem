using LibraryManager.Models.DTOs;
using LibraryManager.Models.Entities;
using LibraryManager.Repositories.Interfaces;
using LibraryManager.Services.Interfaces;

namespace LibraryManager.Services.Implementations
{
    public class BookService : IBookService
    {
        private readonly IBookRepository _bookRepo;
        private readonly IBorrowRecordRepository _recordRepo;

        public BookService(IBookRepository bookRepo, IBorrowRecordRepository recordRepo)
        {
            _bookRepo = bookRepo;
            _recordRepo = recordRepo;
        }

        public async Task<IEnumerable<BookDto>> GetAllAsync()
        {
            var books = await _bookRepo.GetAllAsync();
            return books.Select(ToDto);
        }

        public async Task<PagedResultDto<BookDto>> GetPagedAsync(int page, int pageSize)
        {
            var totalCount = await _bookRepo.GetCountAsync();
            var books = await _bookRepo.GetPagedAsync(page, pageSize);

            return new PagedResultDto<BookDto>
            {
                Items = books.Select(ToDto),
                TotalCount = totalCount,
                PageNumber = page,
                PageSize = pageSize
            };
        }

        public async Task<BookDto?> GetByIdAsync(int id)
        {
            var book = await _bookRepo.GetByIdAsync(id);
            return book == null ? null : ToDto(book);
        }

        public async Task<PagedResultDto<BookDto>> SearchAsync(string keyword, int page = 1, int pageSize = 10)
        {
            var totalCount = await _bookRepo.GetSearchCountAsync(keyword);
            var books = await _bookRepo.SearchAsync(keyword, page, pageSize);

            return new PagedResultDto<BookDto>
            {
                Items = books.Select(ToDto),
                TotalCount = totalCount,
                PageNumber = page,
                PageSize = pageSize
            };
        }

        public async Task<BookDto> CreateAsync(BookCreateDto dto)
        {
            var book = new Book
            {
                Title           = dto.Title,
                Author          = dto.Author,
                ISBN            = dto.ISBN,
                Quantity        = dto.Quantity,
                AvailableQuantity = dto.Quantity,   // Khi tạo, available = total
                Description     = dto.Description,
                PublishedYear   = dto.PublishedYear,
                IsActive        = true,
                CreatedAt       = DateTime.UtcNow
            };

            var created = await _bookRepo.CreateAsync(book);
            return ToDto(created);
        }

        public async Task<BookDto?> UpdateAsync(int id, BookUpdateDto dto)
        {
            var book = await _bookRepo.GetByIdAsync(id);
            if (book == null) return null;

            if (dto.Title       != null) book.Title         = dto.Title;
            if (dto.Author      != null) book.Author        = dto.Author;
            if (dto.ISBN        != null) book.ISBN          = dto.ISBN;
            if (dto.Description != null) book.Description   = dto.Description;
            if (dto.PublishedYear.HasValue) book.PublishedYear = dto.PublishedYear;
            if (dto.IsActive.HasValue) book.IsActive         = dto.IsActive.Value;

            if (dto.Quantity.HasValue)
            {
                int diff = dto.Quantity.Value - book.Quantity;
                book.Quantity          = dto.Quantity.Value;
                book.AvailableQuantity = Math.Max(0, book.AvailableQuantity + diff);
            }

            var updated = await _bookRepo.UpdateAsync(book);
            return ToDto(updated);
        }

        public async Task<(bool Success, string Message)> DeleteAsync(int id)
        {
            var hasActive = await _recordRepo.AnyUnreturnedByBookIdAsync(id);
            if (hasActive)
                return (false, "Không thể xóa sách này vì vẫn còn phiếu mượn chưa được trả.");

            var deleted = await _bookRepo.DeleteAsync(id);
            if (!deleted) return (false, "Không tìm thấy sách.");

            return (true, "Xóa sách thành công.");
        }

        // ── Mapper ────────────────────────────────────────────────
        private static BookDto ToDto(Book b) => new()
        {
            Id                = b.Id,
            Title             = b.Title,
            Author            = b.Author,
            ISBN              = b.ISBN,
            Quantity          = b.Quantity,
            AvailableQuantity = b.AvailableQuantity,
            Description       = b.Description,
            PublishedYear     = b.PublishedYear,
            IsActive          = b.IsActive,
            CreatedAt         = b.CreatedAt,
            UpdatedAt         = b.UpdatedAt
        };
    }
}
