namespace LibraryManager.Models.DTOs
{
    public class BorrowRecordDto
    {
        public int Id { get; set; }
        public int BookId { get; set; }
        public string BookTitle { get; set; } = string.Empty;
        public int BorrowerId { get; set; }
        public string BorrowerName { get; set; } = string.Empty;
        public DateTime BorrowDate { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public bool IsReturned { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsOverdue => !IsReturned && DateTime.UtcNow > DueDate;
    }

    public class BorrowCreateDto
    {
        public int BookId { get; set; }
        public int BorrowerId { get; set; }
        public DateTime DueDate { get; set; }
        public string? Notes { get; set; }
    }

    public class ReturnBookDto
    {
        public string? Notes { get; set; }
    }
}
