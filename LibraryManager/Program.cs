using LibraryManager.Data;
using LibraryManager.Repositories.Implementations;
using LibraryManager.Repositories.Interfaces;
using LibraryManager.Services.Implementations;
using LibraryManager.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ── Database ─────────────────────────────────────────────────────
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<LibraryDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// ── Dependency Injection (N-Layer) ───────────────────────────────
builder.Services.AddScoped<IBookRepository,         BookRepository>();
builder.Services.AddScoped<IBorrowerRepository,      BorrowerRepository>();
builder.Services.AddScoped<IBorrowRecordRepository,  BorrowRecordRepository>();

builder.Services.AddScoped<IBookService,         BookService>();
builder.Services.AddScoped<IBorrowerService,      BorrowerService>();
builder.Services.AddScoped<IBorrowRecordService,  BorrowRecordService>();

// ── Controllers + JSON ───────────────────────────────────────────
builder.Services.AddControllers()
    .AddJsonOptions(opts =>
    {
        opts.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
        opts.JsonSerializerOptions.ReferenceHandler =
            System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });

// ── Swagger ──────────────────────────────────────────────────────
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new()
    {
        Title       = "Library Manager API",
        Version     = "v1",
        Description = "Hệ thống Quản lý Thư viện - Module Mượn/Trả Sách"
    });
});

// ── CORS ─────────────────────────────────────────────────────────
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
});

var app = builder.Build();

// ── Middleware ───────────────────────────────────────────────────
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Library Manager API v1");
    c.RoutePrefix = "swagger";
});

// Phục vụ file tĩnh (giao diện HTML)
app.UseDefaultFiles();
app.UseStaticFiles();

app.UseCors();
app.UseAuthorization();
app.MapControllers();

// ── Auto migrate + Seed Data ─────────────────────────────────────
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<LibraryDbContext>();
    db.Database.Migrate();
    await DbSeeder.SeedAsync(db);
}

app.Run();
