using LibraryManager.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace LibraryManager.Data
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(LibraryDbContext context)
        {
            // Chỉ seed nếu chưa có dữ liệu
            if (await context.Books.AnyAsync()) return;

            // ── SÁCH MẪU ──────────────────────────────────────────
            var books = new List<Book>
            {
                new() { Title = "Lập Trình C# Từ Cơ Bản Đến Nâng Cao", Author = "Nguyễn Văn Hiếu", ISBN = "978-604-1-00001-1", Quantity = 5, AvailableQuantity = 4, Description = "Sách học lập trình C# toàn diện cho người mới bắt đầu", PublishedYear = 2022, IsActive = true, CreatedAt = DateTime.UtcNow },
                new() { Title = "Cơ Sở Dữ Liệu - Database Management", Author = "Lê Thị Hoa", ISBN = "978-604-1-00002-2", Quantity = 8, AvailableQuantity = 6, Description = "Giáo trình cơ sở dữ liệu quan hệ, SQL và thiết kế CSDL", PublishedYear = 2021, IsActive = true, CreatedAt = DateTime.UtcNow },
                new() { Title = "Lập Trình Web ASP.NET Core", Author = "Trần Minh Tuấn", ISBN = "978-604-1-00003-3", Quantity = 6, AvailableQuantity = 5, Description = "Xây dựng ứng dụng web với ASP.NET Core và EF Core", PublishedYear = 2023, IsActive = true, CreatedAt = DateTime.UtcNow },
                new() { Title = "Cấu Trúc Dữ Liệu Và Giải Thuật", Author = "Phạm Quang Dũng", ISBN = "978-604-1-00004-4", Quantity = 10, AvailableQuantity = 8, Description = "Giải thuật và cấu trúc dữ liệu cổ điển", PublishedYear = 2020, IsActive = true, CreatedAt = DateTime.UtcNow },
                new() { Title = "Design Patterns - GOF", Author = "Erich Gamma", ISBN = "978-0-201-63361-0", Quantity = 4, AvailableQuantity = 3, Description = "23 mẫu thiết kế phần mềm kinh điển", PublishedYear = 1994, IsActive = true, CreatedAt = DateTime.UtcNow },
                new() { Title = "Clean Code", Author = "Robert C. Martin", ISBN = "978-0-13-235088-4", Quantity = 5, AvailableQuantity = 4, Description = "Viết code sạch, dễ bảo trì và mở rộng", PublishedYear = 2008, IsActive = true, CreatedAt = DateTime.UtcNow },
                new() { Title = "Nhà Giả Kim", Author = "Paulo Coelho", ISBN = "978-604-1-00007-7", Quantity = 12, AvailableQuantity = 10, Description = "Tiểu thuyết văn học nổi tiếng về hành trình tìm kiếm bản thân", PublishedYear = 1988, IsActive = true, CreatedAt = DateTime.UtcNow },
                new() { Title = "Đắc Nhân Tâm", Author = "Dale Carnegie", ISBN = "978-604-1-00008-8", Quantity = 15, AvailableQuantity = 12, Description = "Nghệ thuật giao tiếp và thu phục lòng người", PublishedYear = 1936, IsActive = true, CreatedAt = DateTime.UtcNow },
                new() { Title = "Tư Duy Phản Biện", Author = "Nguyễn Khắc Giang", ISBN = "978-604-1-00009-9", Quantity = 7, AvailableQuantity = 6, Description = "Rèn luyện tư duy logic và phân tích vấn đề", PublishedYear = 2023, IsActive = true, CreatedAt = DateTime.UtcNow },
                new() { Title = "Trí Tuệ Nhân Tạo - AI Fundamentals", Author = "Stuart Russell", ISBN = "978-0-13-461099-3", Quantity = 3, AvailableQuantity = 2, Description = "Nền tảng trí tuệ nhân tạo và machine learning", PublishedYear = 2020, IsActive = true, CreatedAt = DateTime.UtcNow },
            };

            await context.Books.AddRangeAsync(books);
            await context.SaveChangesAsync();

            // ── NGƯỜI MƯỢN MẪU ───────────────────────────────────
            var borrowers = new List<Borrower>
            {
                new() { FullName = "Nguyễn Thị Lan", Email = "lan.nguyen@gmail.com", Phone = "0901234567", Address = "123 Nguyễn Huệ, Q.1, TP.HCM", DateOfBirth = "2000-05-15", MembershipDate = DateTime.UtcNow.AddMonths(-6), IsActive = true, CreatedAt = DateTime.UtcNow.AddMonths(-6) },
                new() { FullName = "Trần Văn Minh", Email = "minh.tran@gmail.com", Phone = "0912345678", Address = "456 Lê Lợi, Q.3, TP.HCM", DateOfBirth = "2001-08-20", MembershipDate = DateTime.UtcNow.AddMonths(-4), IsActive = true, CreatedAt = DateTime.UtcNow.AddMonths(-4) },
                new() { FullName = "Lê Thị Hồng", Email = "hong.le@gmail.com", Phone = "0923456789", Address = "789 Trần Hưng Đạo, Q.5, TP.HCM", DateOfBirth = "1999-03-10", MembershipDate = DateTime.UtcNow.AddMonths(-8), IsActive = true, CreatedAt = DateTime.UtcNow.AddMonths(-8) },
                new() { FullName = "Phạm Quốc Bảo", Email = "bao.pham@gmail.com", Phone = "0934567890", Address = "321 Điện Biên Phủ, Q.Bình Thạnh", DateOfBirth = "2002-12-25", MembershipDate = DateTime.UtcNow.AddMonths(-2), IsActive = true, CreatedAt = DateTime.UtcNow.AddMonths(-2) },
                new() { FullName = "Võ Thị Thuý", Email = "thuy.vo@gmail.com", Phone = "0945678901", Address = "654 Cách Mạng Tháng 8, Q.Tân Bình", DateOfBirth = "2000-07-30", MembershipDate = DateTime.UtcNow.AddMonths(-3), IsActive = true, CreatedAt = DateTime.UtcNow.AddMonths(-3) },
                new() { FullName = "Đặng Văn Khoa", Email = "khoa.dang@gmail.com", Phone = "0956789012", Address = "987 Hoàng Văn Thụ, Q.Phú Nhuận", DateOfBirth = "2001-01-05", MembershipDate = DateTime.UtcNow.AddMonths(-1), IsActive = true, CreatedAt = DateTime.UtcNow.AddMonths(-1) },
            };

            await context.Borrowers.AddRangeAsync(borrowers);
            await context.SaveChangesAsync();

            // ── LỊCH SỬ MƯỢN MẪU ────────────────────────────────
            var booksInDb   = await context.Books.ToListAsync();
            var borrowersInDb = await context.Borrowers.ToListAsync();

            var records = new List<BorrowRecord>
            {
                // Đã trả
                new() { BookId = booksInDb[0].Id, BorrowerId = borrowersInDb[0].Id, BorrowDate = DateTime.UtcNow.AddDays(-30), DueDate = DateTime.UtcNow.AddDays(-16), ReturnDate = DateTime.UtcNow.AddDays(-18), IsReturned = true, Notes = "Trả đúng hạn", CreatedAt = DateTime.UtcNow.AddDays(-30) },
                new() { BookId = booksInDb[1].Id, BorrowerId = borrowersInDb[1].Id, BorrowDate = DateTime.UtcNow.AddDays(-25), DueDate = DateTime.UtcNow.AddDays(-11), ReturnDate = DateTime.UtcNow.AddDays(-12), IsReturned = true, Notes = "Sách còn tốt", CreatedAt = DateTime.UtcNow.AddDays(-25) },
                new() { BookId = booksInDb[6].Id, BorrowerId = borrowersInDb[2].Id, BorrowDate = DateTime.UtcNow.AddDays(-20), DueDate = DateTime.UtcNow.AddDays(-6), ReturnDate = DateTime.UtcNow.AddDays(-7), IsReturned = true, Notes = "", CreatedAt = DateTime.UtcNow.AddDays(-20) },

                // Đang mượn (chưa quá hạn)
                new() { BookId = booksInDb[0].Id, BorrowerId = borrowersInDb[1].Id, BorrowDate = DateTime.UtcNow.AddDays(-5), DueDate = DateTime.UtcNow.AddDays(9), IsReturned = false, Notes = "Mượn để học môn CNPM", CreatedAt = DateTime.UtcNow.AddDays(-5) },
                new() { BookId = booksInDb[2].Id, BorrowerId = borrowersInDb[0].Id, BorrowDate = DateTime.UtcNow.AddDays(-3), DueDate = DateTime.UtcNow.AddDays(11), IsReturned = false, Notes = "", CreatedAt = DateTime.UtcNow.AddDays(-3) },
                new() { BookId = booksInDb[4].Id, BorrowerId = borrowersInDb[3].Id, BorrowDate = DateTime.UtcNow.AddDays(-7), DueDate = DateTime.UtcNow.AddDays(7), IsReturned = false, Notes = "Nghiên cứu design patterns", CreatedAt = DateTime.UtcNow.AddDays(-7) },
                new() { BookId = booksInDb[7].Id, BorrowerId = borrowersInDb[4].Id, BorrowDate = DateTime.UtcNow.AddDays(-2), DueDate = DateTime.UtcNow.AddDays(12), IsReturned = false, Notes = "", CreatedAt = DateTime.UtcNow.AddDays(-2) },

                // Quá hạn
                new() { BookId = booksInDb[3].Id, BorrowerId = borrowersInDb[2].Id, BorrowDate = DateTime.UtcNow.AddDays(-20), DueDate = DateTime.UtcNow.AddDays(-6), IsReturned = false, Notes = "Chưa liên lạc được", CreatedAt = DateTime.UtcNow.AddDays(-20) },
                new() { BookId = booksInDb[5].Id, BorrowerId = borrowersInDb[5].Id, BorrowDate = DateTime.UtcNow.AddDays(-18), DueDate = DateTime.UtcNow.AddDays(-4), IsReturned = false, Notes = "Đã gửi nhắc nhở", CreatedAt = DateTime.UtcNow.AddDays(-18) },
            };

            // Cập nhật AvailableQuantity cho các sách đang mượn/quá hạn
            var activeRecords = records.Where(r => !r.IsReturned).ToList();
            foreach (var record in activeRecords)
            {
                var book = booksInDb.First(b => b.Id == record.BookId);
                if (book.AvailableQuantity > 0)
                    book.AvailableQuantity--;
            }

            await context.BorrowRecords.AddRangeAsync(records);
            await context.SaveChangesAsync();
        }
    }
}
