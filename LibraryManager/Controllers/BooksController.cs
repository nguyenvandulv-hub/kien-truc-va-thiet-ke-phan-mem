using LibraryManager.Models.DTOs;
using LibraryManager.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManager.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BooksController : ControllerBase
    {
        private readonly IBookService _bookService;

        public BooksController(IBookService bookService)
        {
            _bookService = bookService;
        }

        // GET api/books?page=1&pageSize=10
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 10;

            var result = await _bookService.GetPagedAsync(page, pageSize);
            return Ok(result);
        }

        // GET api/books/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var book = await _bookService.GetByIdAsync(id);
            if (book == null) return NotFound(new { message = $"Không tìm thấy sách với id={id}" });
            return Ok(book);
        }

        // GET api/books/search?keyword=harry&page=1&pageSize=10
        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string keyword, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return BadRequest(new { message = "Vui lòng nhập từ khóa tìm kiếm." });

            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 10;

            var result = await _bookService.SearchAsync(keyword, page, pageSize);
            return Ok(result);
        }

        // POST api/books
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] BookCreateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var created = await _bookService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        // PUT api/books/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] BookUpdateDto dto)
        {
            var updated = await _bookService.UpdateAsync(id, dto);
            if (updated == null) return NotFound(new { message = $"Không tìm thấy sách với id={id}" });
            return Ok(updated);
        }

        // DELETE api/books/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var (success, message) = await _bookService.DeleteAsync(id);
            if (!success)
            {
                if (message == "Không tìm thấy sách.") return NotFound(new { message });
                return BadRequest(new { message });
            }
            return Ok(new { message });
        }
    }
}
