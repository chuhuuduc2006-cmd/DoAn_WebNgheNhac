using Microsoft.AspNetCore.Mvc;
using WebNgheNhac.Services;

namespace WebNgheNhac.Controllers;

public class SearchController : Controller
{
    private readonly SpotifyService _spotifyService;

    public SearchController(SpotifyService spotifyService)
    {
        _spotifyService = spotifyService;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> Search(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
            return Json(new { success = false, message = "Vui lòng nhập từ khóa" });

        try
        {
            var tracks = await _spotifyService.SearchTracksAsync(query);
            var results = tracks.Select(t => new
            {
                id = t.Id,
                title = t.Name,
                artist = string.Join(", ", t.Artists.Select(a => a.Name)),
                image = t.Album.Images.FirstOrDefault()?.Url ?? "/uploads/default.jpg"
            });
            return Json(new { success = true, data = results });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }
}