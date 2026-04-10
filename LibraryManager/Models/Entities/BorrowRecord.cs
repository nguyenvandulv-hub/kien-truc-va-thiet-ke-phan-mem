using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryManager.Models.Entities
{
    [Table("borrow_records")]
    public class BorrowRecord
    {
        [Key]
        [Column("Id")]
        public int Id { get; set; }

        [Required]
        public int BookId { get; set; }

        [Required]
        public int BorrowerId { get; set; }

        public DateTime BorrowDate { get; set; } = DateTime.UtcNow;

        [Required]
        public DateTime DueDate { get; set; }

        public DateTime? ReturnDate { get; set; }

        public bool IsReturned { get; set; } = false;

        public string? Notes { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        [ForeignKey("BookId")]
        public Book Book { get; set; } = null!;

        [ForeignKey("BorrowerId")]
        public Borrower Borrower { get; set; } = null!;
    }
}
