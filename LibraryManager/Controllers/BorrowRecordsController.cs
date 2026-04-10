using LibraryManager.Models.DTOs;
using LibraryManager.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManager.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BorrowRecordsController : ControllerBase
    {
        private readonly IBorrowRecordService _borrowRecordService;

        public BorrowRecordsController(IBorrowRecordService borrowRecordService)
        {
            _borrowRecordService = borrowRecordService;
        }

        // GET api/borrowrecords?page=1&pageSize=10
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 10;

            var result = await _borrowRecordService.GetPagedAsync(page, pageSize);
            return Ok(result);
        }

        // GET api/borrowrecords/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var record = await _borrowRecordService.GetByIdAsync(id);
            if (record == null) return NotFound(new { message = $"Không tìm thấy bản ghi mượn sách id={id}" });
            return Ok(record);
        }

        // GET api/borrowrecords/borrower/3
        [HttpGet("borrower/{borrowerId}")]
        public async Task<IActionResult> GetByBorrower(int borrowerId)
        {
            var records = await _borrowRecordService.GetByBorrowerIdAsync(borrowerId);
            return Ok(records);
        }

        // GET api/borrowrecords/book/5
        [HttpGet("book/{bookId}")]
        public async Task<IActionResult> GetByBook(int bookId)
        {
            var records = await _borrowRecordService.GetByBookIdAsync(bookId);
            return Ok(records);
        }

        // GET api/borrowrecords/overdue?page=1&pageSize=10
        [HttpGet("overdue")]
        public async Task<IActionResult> GetOverdue([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 10;

            var result = await _borrowRecordService.GetOverduePagedAsync(page, pageSize);
            return Ok(result);
        }

        // GET api/borrowrecords/active
        [HttpGet("active")]
        public async Task<IActionResult> GetActive()
        {
            var records = await _borrowRecordService.GetActiveAsync();
            return Ok(records);
        }

        // POST api/borrowrecords/borrow
        [HttpPost("borrow")]
        public async Task<IActionResult> Borrow([FromBody] BorrowCreateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var (success, message, data) = await _borrowRecordService.BorrowBookAsync(dto);
            if (!success) return BadRequest(new { message });

            return CreatedAtAction(nameof(GetById), new { id = data!.Id },
                new { message, data });
        }

        // PUT api/borrowrecords/5/return
        [HttpPut("{id}/return")]
        public async Task<IActionResult> Return(int id, [FromBody] ReturnBookDto dto)
        {
            var (success, message, data) = await _borrowRecordService.ReturnBookAsync(id, dto);
            if (!success) return BadRequest(new { message });
            return Ok(new { message, data });
        }
    }
}
