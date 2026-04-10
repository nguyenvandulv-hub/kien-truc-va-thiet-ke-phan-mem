namespace LibraryManager.Models.DTOs
{
    public class BorrowerDto
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public string? DateOfBirth { get; set; }   // "yyyy-MM-dd" string để tránh lỗi serialize DateOnly trong .NET 6
        public DateTime MembershipDate { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class BorrowerCreateDto
    {
        public string FullName { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public string? DateOfBirth { get; set; }   // "yyyy-MM-dd"
    }

    public class BorrowerUpdateDto
    {
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public string? DateOfBirth { get; set; }   // "yyyy-MM-dd"
        public bool? IsActive { get; set; }
    }
}
