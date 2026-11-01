using Microsoft.AspNetCore.Mvc;
using WebNgheNhac.Models;
using Microsoft.Data.SqlClient;

namespace WebNgheNhac.Controllers;

public class HomeController : Controller
{
    private readonly DbHelper _db;
    public HomeController(DbHelper db) { _db = db; }

    public IActionResult Index()
    {
        var songs = new List<Song>();
        using (var conn = _db.GetConnection())
        {
            conn.Open();
            var cmd = new SqlCommand("SELECT * FROM Songs ORDER BY CreatedAt DESC", conn);
            var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                songs.Add(new Song
                {
                    Id = (int)reader["Id"],
                    Title = reader["Title"].ToString()!,
                    Artist = reader["Artist"].ToString()!,
                    FilePath = reader["FilePath"].ToString()!,
                    ImagePath = reader["ImagePath"]?.ToString(),
                    SpotifyTrackId = reader["SpotifyTrackId"]?.ToString()
                });
            }
        }
        return View(songs);
    }
}