using Microsoft.AspNetCore.Mvc;
using WebNgheNhac.Models;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace WebNgheNhac.Controllers;

public class AdminController : Controller
{
    private readonly DbHelper _db;
    private readonly IWebHostEnvironment _env;

    // Inject DbHelper và IWebHostEnvironment (để lấy đường dẫn thư mục wwwroot)
    public AdminController(DbHelper db, IWebHostEnvironment env)
    {
        _db = db;
        _env = env;
    }

    // Kiểm tra quyền Admin (Bạn có thể sửa lại logic này theo ý muốn)
    private bool IsAdmin()
    {
        // Tạm thời cho phép nếu đã đăng nhập (để dễ test)
        // Thực tế nên check: HttpContext.Session.GetString("Role") == "admin"
        return HttpContext.Session.GetString("Username") != null;
    }

    // 1. TRANG DANH SÁCH BÀI HÁT (READ)
    public IActionResult Index()
    {
        if (!IsAdmin()) return RedirectToAction("Login", "Account");

        var songs = new List<Song>();
        using (var conn = _db.GetConnection())
        {
            conn.Open();
            var cmd = new SqlCommand("SELECT * FROM Songs ORDER BY Id DESC", conn);
            var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                songs.Add(new Song
                {
                    Id = (int)reader["Id"],
                    Title = reader["Title"].ToString()!,
                    Artist = reader["Artist"].ToString()!,
                    FilePath = reader["FilePath"].ToString()!
                });
            }
        }
        return View(songs);
    }

    // 2. THÊM BÀI HÁT MỚI (CREATE - GET)
    [HttpGet]
    public IActionResult Create()
    {
        if (!IsAdmin()) return RedirectToAction("Login", "Account");
        return View();
    }

    // 2. THÊM BÀI HÁT MỚI (CREATE - POST)
    [HttpPost]
    public async Task<IActionResult> Create(string title, string artist, IFormFile musicFile)
    {
        if (!IsAdmin()) return RedirectToAction("Login", "Account");

        if (musicFile != null && musicFile.Length > 0)
        {
            // Tạo tên file ngẫu nhiên để không bị trùng
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(musicFile.FileName);
            var uploadPath = Path.Combine(_env.WebRootPath, "uploads", fileName);

            // Lưu file vào thư mục wwwroot/uploads
            using (var stream = new FileStream(uploadPath, FileMode.Create))
            {
                await musicFile.CopyToAsync(stream);
            }

            // Lưu thông tin vào Database
            using (var conn = _db.GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand("INSERT INTO Songs (Title, Artist, FilePath, ImagePath) VALUES (@t, @a, @f, 'default.jpg')", conn);
                cmd.Parameters.AddWithValue("@t", title);
                cmd.Parameters.AddWithValue("@a", artist);
                cmd.Parameters.AddWithValue("@f", fileName); // Lưu tên file vào DB
                cmd.ExecuteNonQuery();
            }
        }
        return RedirectToAction("Index");
    }

    // 3. SỬA BÀI HÁT (UPDATE - GET)
    [HttpGet]
    public IActionResult Edit(int id)
    {
        if (!IsAdmin()) return RedirectToAction("Login", "Account");

        Song? song = null;
        using (var conn = _db.GetConnection())
        {
            conn.Open();
            var cmd = new SqlCommand("SELECT * FROM Songs WHERE Id = @id", conn);
            cmd.Parameters.AddWithValue("@id", id);
            var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                song = new Song
                {
                    Id = (int)reader["Id"],
                    Title = reader["Title"].ToString()!,
                    Artist = reader["Artist"].ToString()!,
                    FilePath = reader["FilePath"].ToString()!
                };
            }
        }
        if (song == null) return NotFound();
        return View(song);
    }

    // 3. SỬA BÀI HÁT (UPDATE - POST)
    [HttpPost]
    public async Task<IActionResult> Edit(int id, string title, string artist, IFormFile? musicFile)
    {
        if (!IsAdmin()) return RedirectToAction("Login", "Account");

        string? newFileName = null;

        // Nếu có upload file mới thì lưu file mới
        if (musicFile != null && musicFile.Length > 0)
        {
            newFileName = Guid.NewGuid().ToString() + Path.GetExtension(musicFile.FileName);
            var uploadPath = Path.Combine(_env.WebRootPath, "uploads", newFileName);
            using (var stream = new FileStream(uploadPath, FileMode.Create))
            {
                await musicFile.CopyToAsync(stream);
            }
        }

        using (var conn = _db.GetConnection())
        {
            conn.Open();
            string sql = newFileName != null 
                ? "UPDATE Songs SET Title = @t, Artist = @a, FilePath = @f WHERE Id = @id"
                : "UPDATE Songs SET Title = @t, Artist = @a WHERE Id = @id";

            var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@t", title);
            cmd.Parameters.AddWithValue("@a", artist);
            cmd.Parameters.AddWithValue("@id", id);
            
            if (newFileName != null) cmd.Parameters.AddWithValue("@f", newFileName);
            
            cmd.ExecuteNonQuery();
        }
        return RedirectToAction("Index");
    }

    // 4. XÓA BÀI HÁT (DELETE)
    public IActionResult Delete(int id)
    {
        if (!IsAdmin()) return RedirectToAction("Login", "Account");

        using (var conn = _db.GetConnection())
        {
            conn.Open();
            var cmd = new SqlCommand("DELETE FROM Songs WHERE Id = @id", conn);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();
        }
        return RedirectToAction("Index");
    }
}