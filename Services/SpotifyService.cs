using SpotifyAPI.Web;

namespace WebNgheNhac.Services;

public class SpotifyService
{
    private readonly SpotifyClient _spotify;

    public SpotifyService(IConfiguration config)
    {
        var clientId = config["Spotify:ClientId"];
        var clientSecret = config["Spotify:ClientSecret"];

        // Sử dụng Client Credentials flow (chỉ đọc dữ liệu công khai)
        var configSpotify = SpotifyClientConfig
            .CreateDefault()
            .WithAuthenticator(new ClientCredentialsAuthenticator(clientId, clientSecret));

        _spotify = new SpotifyClient(configSpotify);
    }

    public async Task<List<FullTrack>> SearchTracksAsync(string query, int limit = 10)
    {
        if (string.IsNullOrWhiteSpace(query))
            return new List<FullTrack>();

        // Gọi API tìm kiếm bài hát
        var searchRequest = new SearchRequest(SearchRequest.Types.Track, query)
        {
            Limit = limit
        };

        var searchResponse = await _spotify.Search.Item(searchRequest);
        
        // Trả về danh sách bài hát
        return searchResponse.Tracks.Items?.ToList() ?? new List<FullTrack>();
    }
}