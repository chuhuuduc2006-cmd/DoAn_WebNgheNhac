using Microsoft.AspNetCore.Mvc;
using WebNgheNhac.Models;
using Microsoft.Data.SqlClient;

namespace WebNgheNhac.Controllers;

public class PlaylistController : Controller
{
    private readonly DbHelper _db;
    public PlaylistController(DbHelper db) { _db = db; }

    public IActionResult Index()
    {
        var playlists = new List<Playlist>();
        using (var conn = _db.GetConnection())
        {
            conn.Open();
            var cmd = new SqlCommand("SELECT * FROM Playlists ORDER BY CreatedAt DESC", conn);
            var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                playlists.Add(new Playlist
                {
                    Id = (int)reader["Id"],
                    Name = reader["Name"].ToString()!,
                    Description = reader["Description"]?.ToString(),
                    SpotifyPlaylistId = reader["SpotifyPlaylistId"].ToString()!,
                    CoverImage = reader["CoverImage"]?.ToString()
                });
            }
        }
        return View(playlists);
    }

    public IActionResult Detail(int id)
    {
        Playlist? playlist = null;
        using (var conn = _db.GetConnection())
        {
            conn.Open();
            var cmd = new SqlCommand("SELECT * FROM Playlists WHERE Id=@id", conn);
            cmd.Parameters.AddWithValue("@id", id);
            var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                playlist = new Playlist
                {
                    Id = (int)reader["Id"],
                    Name = reader["Name"].ToString()!,
                    Description = reader["Description"]?.ToString(),
                    SpotifyPlaylistId = reader["SpotifyPlaylistId"].ToString()!,
                    CoverImage = reader["CoverImage"]?.ToString()
                };
            }
        }
        if (playlist == null) return NotFound();
        return View(playlist);
    }
}