namespace WebNgheNhac.Models;

public class Song
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public string Artist { get; set; } = "";
    public string FilePath { get; set; } = "";
    public string? ImagePath { get; set; }
    public string? SpotifyTrackId { get; set; }  // <-- Dòng quan trọng đang bị thiếu
    public int? CategoryId { get; set; }
}