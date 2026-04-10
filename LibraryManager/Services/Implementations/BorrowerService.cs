using LibraryManager.Models.DTOs;
using LibraryManager.Models.Entities;
using LibraryManager.Repositories.Interfaces;
using LibraryManager.Services.Interfaces;

namespace LibraryManager.Services.Implementations
{
    public class BorrowerService : IBorrowerService
    {
        private readonly IBorrowerRepository _borrowerRepo;
        private readonly IBorrowRecordRepository _recordRepo;

        public BorrowerService(IBorrowerRepository borrowerRepo, IBorrowRecordRepository recordRepo)
        {
            _borrowerRepo = borrowerRepo;
            _recordRepo = recordRepo;
        }

        public async Task<IEnumerable<BorrowerDto>> GetAllAsync()
        {
            var borrowers = await _borrowerRepo.GetAllAsync();
            return borrowers.Select(ToDto);
        }

        public async Task<PagedResultDto<BorrowerDto>> GetPagedAsync(int page, int pageSize)
        {
            var totalCount = await _borrowerRepo.GetCountAsync();
            var borrowers = await _borrowerRepo.GetPagedAsync(page, pageSize);

            return new PagedResultDto<BorrowerDto>
            {
                Items = borrowers.Select(ToDto),
                TotalCount = totalCount,
                PageNumber = page,
                PageSize = pageSize
            };
        }

        public async Task<BorrowerDto?> GetByIdAsync(int id)
        {
            var borrower = await _borrowerRepo.GetByIdAsync(id);
            return borrower == null ? null : ToDto(borrower);
        }

        public async Task<PagedResultDto<BorrowerDto>> SearchAsync(string keyword, int page = 1, int pageSize = 10)
        {
            var totalCount = await _borrowerRepo.GetSearchCountAsync(keyword);
            var borrowers = await _borrowerRepo.SearchAsync(keyword, page, pageSize);

            return new PagedResultDto<BorrowerDto>
            {
                Items = borrowers.Select(ToDto),
                TotalCount = totalCount,
                PageNumber = page,
                PageSize = pageSize
            };
        }

        public async Task<BorrowerDto> CreateAsync(BorrowerCreateDto dto)
        {
            var borrower = new Borrower
            {
                FullName       = dto.FullName,
                Email          = dto.Email,
                Phone          = dto.Phone,
                Address        = dto.Address,
                DateOfBirth    = dto.DateOfBirth,   // string "yyyy-MM-dd"
                MembershipDate = DateTime.UtcNow,
                IsActive       = true,
                CreatedAt      = DateTime.UtcNow
            };

            var created = await _borrowerRepo.CreateAsync(borrower);
            return ToDto(created);
        }

        public async Task<BorrowerDto?> UpdateAsync(int id, BorrowerUpdateDto dto)
        {
            var borrower = await _borrowerRepo.GetByIdAsync(id);
            if (borrower == null) return null;

            if (dto.FullName     != null) borrower.FullName    = dto.FullName;
            if (dto.Email        != null) borrower.Email       = dto.Email;
            if (dto.Phone        != null) borrower.Phone       = dto.Phone;
            if (dto.Address      != null) borrower.Address     = dto.Address;
            if (dto.DateOfBirth  != null) borrower.DateOfBirth = dto.DateOfBirth;
            if (dto.IsActive.HasValue)    borrower.IsActive    = dto.IsActive.Value;

            var updated = await _borrowerRepo.UpdateAsync(borrower);
            return ToDto(updated);
        }

        public async Task<(bool Success, string Message)> DeleteAsync(int id)
        {
            var hasActive = await _recordRepo.AnyUnreturnedByBorrowerIdAsync(id);
            if (hasActive)
                return (false, "Không thể xóa thành viên này vì vẫn còn phiếu mượn chưa được trả.");

            var deleted = await _borrowerRepo.DeleteAsync(id);
            if (!deleted) return (false, "Không tìm thấy người mượn.");

            return (true, "Xóa thành viên thành công.");
        }

        // ── Mapper ────────────────────────────────────────────────
        private static BorrowerDto ToDto(Borrower b) => new()
        {
            Id             = b.Id,
            FullName       = b.FullName,
            Email          = b.Email,
            Phone          = b.Phone,
            Address        = b.Address,
            DateOfBirth    = b.DateOfBirth,   // already string
            MembershipDate = b.MembershipDate,
            IsActive       = b.IsActive,
            CreatedAt      = b.CreatedAt
        };
    }
}
