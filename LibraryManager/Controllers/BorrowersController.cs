using LibraryManager.Models.DTOs;
using LibraryManager.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManager.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BorrowersController : ControllerBase
    {
        private readonly IBorrowerService _borrowerService;

        public BorrowersController(IBorrowerService borrowerService)
        {
            _borrowerService = borrowerService;
        }

        // GET api/borrowers?page=1&pageSize=10
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 10;

            var result = await _borrowerService.GetPagedAsync(page, pageSize);
            return Ok(result);
        }

        // GET api/borrowers/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var borrower = await _borrowerService.GetByIdAsync(id);
            if (borrower == null) return NotFound(new { message = $"Không tìm thấy người mượn với id={id}" });
            return Ok(borrower);
        }

        // GET api/borrowers/search?keyword=nguyen&page=1&pageSize=10
        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string keyword, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return BadRequest(new { message = "Vui lòng nhập từ khóa tìm kiếm." });

            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 10;

            var result = await _borrowerService.SearchAsync(keyword, page, pageSize);
            return Ok(result);
        }

        // POST api/borrowers
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] BorrowerCreateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var created = await _borrowerService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        // PUT api/borrowers/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] BorrowerUpdateDto dto)
        {
            var updated = await _borrowerService.UpdateAsync(id, dto);
            if (updated == null) return NotFound(new { message = $"Không tìm thấy người mượn với id={id}" });
            return Ok(updated);
        }

        // DELETE api/borrowers/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var (success, message) = await _borrowerService.DeleteAsync(id);
            if (!success)
            {
                if (message == "Không tìm thấy người mượn.") return NotFound(new { message });
                return BadRequest(new { message });
            }
            return Ok(new { message });
        }
    }
}
