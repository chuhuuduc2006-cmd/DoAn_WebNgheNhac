using Microsoft.AspNetCore.Mvc;
using WebNgheNhac.Models;
using Microsoft.Data.SqlClient;

namespace WebNgheNhac.Controllers;

public class AccountController : Controller
{
    private readonly DbHelper _db;
    public AccountController(DbHelper db) { _db = db; }

    [HttpGet]
    public IActionResult Login() => View();

    [HttpPost]
    public IActionResult Login(string username, string password)
    {
        using var conn = _db.GetConnection();
        conn.Open();
        var cmd = new SqlCommand("SELECT * FROM Users WHERE Username=@u", conn);
        cmd.Parameters.AddWithValue("@u", username);
        var reader = cmd.ExecuteReader();
        if (reader.Read())
        {
            var hash = reader["Password"].ToString()!;
            if (BCrypt.Net.BCrypt.Verify(password, hash))
            {
                HttpContext.Session.SetString("Username", username);
                HttpContext.Session.SetInt32("UserId", (int)reader["Id"]);
                return RedirectToAction("Index", "Home");
            }
        }
        ViewBag.Error = "Sai tên đăng nhập hoặc mật khẩu!";
        return View();
    }

    [HttpGet]
    public IActionResult Register() => View();

    [HttpPost]
    public IActionResult Register(string username, string email, string password)
    {
        try
        {
            using var conn = _db.GetConnection();
            conn.Open();
            var cmd = new SqlCommand(
                "INSERT INTO Users (Username, Email, Password) VALUES (@u, @e, @p)", conn);
            cmd.Parameters.AddWithValue("@u", username);
            cmd.Parameters.AddWithValue("@e", email);
            cmd.Parameters.AddWithValue("@p", BCrypt.Net.BCrypt.HashPassword(password));
            cmd.ExecuteNonQuery();
            return RedirectToAction("Login");
        }
        catch
        {
            ViewBag.Error = "Tên đăng nhập đã tồn tại!";
            return View();
        }
    }

    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Index", "Home");
    }
}