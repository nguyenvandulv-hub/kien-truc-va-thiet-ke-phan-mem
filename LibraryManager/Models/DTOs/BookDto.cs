namespace LibraryManager.Models.DTOs
{
    // GET response
    public class BookDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string? ISBN { get; set; }
        public int Quantity { get; set; }
        public int AvailableQuantity { get; set; }
        public string? Description { get; set; }
        public int? PublishedYear { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    // CREATE / UPDATE request
    public class BookCreateDto
    {
        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string? ISBN { get; set; }
        public int Quantity { get; set; }
        public string? Description { get; set; }
        public int? PublishedYear { get; set; }
    }

    public class BookUpdateDto
    {
        public string? Title { get; set; }
        public string? Author { get; set; }
        public string? ISBN { get; set; }
        public int? Quantity { get; set; }
        public string? Description { get; set; }
        public int? PublishedYear { get; set; }
        public bool? IsActive { get; set; }
    }
}
