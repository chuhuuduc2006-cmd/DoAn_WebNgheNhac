namespace WebNgheNhac.Models;

public class Playlist
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string? Description { get; set; }
    public string SpotifyPlaylistId { get; set; } = "";
    public string? CoverImage { get; set; }
}