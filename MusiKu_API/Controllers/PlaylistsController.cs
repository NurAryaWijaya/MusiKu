using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MusiKu_API.Data;
using MusiKu_API.Models;
using Swashbuckle.AspNetCore.Filters;
using System.CodeDom;
using System.Security.Claims;

namespace MusiKu_API.Controllers
{
    [Route("api/playlist")]
    [ApiController]
    public class PlaylistsController : ControllerBase
    {
        private readonly MusiKuContext _context;
        public PlaylistsController(MusiKuContext context)
        {
            _context = context;
        }

        [HttpGet("popular")]
        public async Task<ActionResult<IEnumerable<PlaylistDto>>> GetPopularPlaylist()
        {
            var songs = await _context.Songs
                .OrderByDescending(e => e.PlayCount)
                .Take(10)
                .ToListAsync();
            if (songs.All(s => s.PlayCount == 0))
            {
                songs = await _context.Songs
                    .OrderBy(e => Guid.NewGuid())
                    .Take(10)
                    .ToListAsync();
            }

            var playlist = new PlaylistDto
            {
                Name = "Popular Playlist",
                Songs = songs
            };
            return Ok(playlist);

        }

        [HttpGet("favorite")]
        public async Task<ActionResult<IEnumerable<PlaylistDto>>> GetFavoritePlaylist()
        {
            var songs = await _context.Songs
                .OrderByDescending(e => e.Likes)
                .Take(10)
                .ToListAsync();
            if (songs.All(s => s.Likes == 0))
            {
                songs = await _context.Songs
                    .OrderBy(e => Guid.NewGuid())
                    .Take(10)
                    .ToListAsync();
            }

            var playlist = new PlaylistDto
            {
                Name = "Most Favorite",
                Songs = songs
            };
            return Ok(playlist);
        }

        [HttpGet("default-theme")]
        public async Task<ActionResult<PlaylistDto>> GetDefaultThemePlaylist(string name)
        {
            var defaultTheme = new Dictionary<string, List<int>>
            {
                {"90 Song Playlist", new List<int> {1,2,3,4,5,6,7,8,9,10} },
                {"aespa", new List<int> {11,12,13,14,15,16,17} },
                {"Ariana Grande", new List<int> {18,19,20,21,22,280} },
                {"BABYMONSTER", new List<int> {23,24,25,26} },
                {"Billie Eilish", new List<int> {27,28,29,30,31} },
                {"BLACKPINK", new List<int> {32,33,34,35,36,37,38,39,40} },
                {"Bruno Mars", new List<int> {41,42,43,44,45,46} },
                {"BTS", new List<int> {47,48,49,50,51,52,53,54,55,56,57} },
                {"Dangdut Playlist", new List<int> {58,59,60,61,62,63,64,65,66,67} },
                {"DJ Remix Playlist", new List<int> {68,69,70,71,72} },
                {"EXO", new List<int> {73,74,75,76,77} },
                {"Isyana Sarasvati", new List<int> {78,79,80,81} },
                {"IVE", new List<int> {82,83,84,85,86,87,88,89,90} },
                {"JKT48", new List<int> {91,92,93,94,95,96,97,98,99} },
                {"Justin Bieber", new List<int> {100,101,102,103,104,105,106,107} },
                {"Kpop", new List<int> { 47,48,49,35,36,37,12,13,14} },
                {"Other", new List<int> { 108,109,110,111,112,113,114,115,116,117,118,119,120,121,122,123,124,125,126,127,128,129,130} },
                {"OST A Business Proposal", new List<int> {131,132,133,134,135,136} },
                {"OST A Love So Beautiful", new List<int> {137,138,139} },
                {"OST A River Runs Through It", new List<int> {140,141,142,143,144,145,146,147} },
                {"OST Descendants of the Sun", new List<int> {148,149,150,151,152} },
                {"OST Extra Ordinary You", new List<int> {153,154,155,156} },
                {"OST Filter", new List<int> {157,158,159,160,161,162,163} },
                {"OST Guardian The Lonely and Great God(Goblin)", new List<int> {164,165,166,167,168} },
                {"OST Hidden Love", new List<int> {169,170,171,172,173,174,175} },
                {"OST Love O2O", new List<int> {176,177,178,179} },
                {"OST Lovely Runner", new List<int> {180,181,182,183,184,185} },
                {"OST Moon Lovers Scarlet Heart Ryeo", new List<int> {186,187,188,189,190,191,192,193,194} },
                {"OST The First Frost", new List<int> {195,196,197,198,199,200,201,202,203,204,205,206,207,208} },
                {"OST Twenty Five Twenty One", new List<int> {209,210,211,212,213,214,215} },
                {"OST TwinkIing Watermelon", new List<int> {216,217,218,219,220,221} },
                {"OST When I Fly Towards You", new List<int> {222,223,224,225,226} },
                {"POP Indonesia", new List<int> {227,228,229,230,231,232,233,234,235,236,237,238,23,240,241,242,243,244,245,246} },
                {"Raisa", new List<int> {247,248,249,250,251} },
                {"Red Velvet", new List<int> {252,253,254,255,256,257} },
                {"Rock Playlist", new List<int> {258,259,260,261,262,263,264,265,266,267} },
                {"Taylor Swift", new List<int> {268,269,270,271,272} },
                {"Twice", new List<int> {273,274,275,276,277,278} }
            };

            var matchedEntry = defaultTheme
                        .FirstOrDefault(x => string.Equals(x.Key, name, StringComparison.OrdinalIgnoreCase));

            if (matchedEntry.Equals(default(KeyValuePair<string, List<int>>)))
            {
                return NotFound($"Playlist {name} Not Found");
            }

            var selectedIds = matchedEntry.Value;


            var songs = await _context.Songs
                .Where(e => selectedIds.Contains(e.Id))
                .ToListAsync();

            var playlist = new PlaylistDto
            {
                Name = $"{name}",
                Songs = songs.Select(s => new Song
                {
                    Id = s.Id,
                    Title = s.Title,
                    Artist = s.Artist,
                    PlayCount = s.PlayCount,
                    Likes = s.Likes,
                    Mp3Url = s.Mp3Url,
                    CoverUrl = s.CoverUrl
                }).ToList()
            };

            return Ok(playlist);
        }

        [HttpPost("user-created")]
        [Authorize]
        [SwaggerRequestExample(typeof(CreatePlaylistDto), typeof(CreatePlaylistDtoExample))]
        public async Task<ActionResult> CreateUserPlaylist([FromBody] CreatePlaylistDto dto)
        {
            // Pastikan DTO tidak null atau kosong
            if (dto == null || dto.SongIds == null || !dto.SongIds.Any())
            {
                return BadRequest("Playlist name and song IDs cannot be null or empty.");
            }

            // Mengambil UserId dari klaim (Claim)
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var playlistCount = await _context.Playlists.CountAsync(p => p.UserId == userId);
            var name = string.IsNullOrEmpty(dto.Name)? $"Playlist {playlistCount + 1}": dto.Name;
            // Jika userId tidak ditemukan, return Unauthorized
            if (userId == null)
            {
                return Unauthorized("User is not authenticated.");
            }

            // Membuat playlist baru dengan informasi dari DTO
            var playlist = new Playlist
            {
                Name = name,
                UserId = userId
            };

            // Validasi SongIds agar semua ID lagu ada di database
            var songs = await _context.Songs
                .Where(s => dto.SongIds.Contains(s.Id))
                .ToListAsync();

            if (songs.Count != dto.SongIds.Count)
            {
                return BadRequest("One or more song IDs are invalid.");
            }

            // Menambahkan Song ke dalam Playlist
            playlist.PlaylistSongs = songs.Select(song => new PlaylistSong
            {
                SongId = song.Id
            }).ToList();

            // Menambahkan playlist ke dalam database
            _context.Playlists.Add(playlist);
            await _context.SaveChangesAsync();

            // Kembalikan response dengan status sukses
            return Ok(new { message = "Playlist berhasil dibuat", playlistId = playlist.Id });
        }

        [HttpGet("user-playlist-created")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<PlaylistDto>>> GetMyPlaylist()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var playlist = await _context.Playlists
                .Where(p => p.UserId == userId)
                .Include(p => p.PlaylistSongs)
                    .ThenInclude(ps => ps.Song)
                .ToListAsync();

            if (playlist.Count == 0)
            {
                return NotFound("No playlist found");
            }

            var playlistDtos = playlist.Select(p => new PlaylistDto
            {
                Id = p.Id,
                Name = p.Name,
                Songs = p.PlaylistSongs
                    .OrderByDescending(ps => ps.AddedAt) // ⬅️ Urut berdasarkan waktu ditambahkan
                    .Select(ps => new Song
                    {
                        Id = ps.Song.Id,
                        Title = ps.Song.Title,
                        Artist = ps.Song.Artist,
                        PlayCount = ps.Song.PlayCount,
                        Likes = ps.Song.Likes,
                        Mp3Url = ps.Song.Mp3Url,
                        CoverUrl = ps.Song.CoverUrl
                    }).ToList()
                }).ToList();


            return Ok(playlistDtos);
        }

        [HttpPut("edit-playlist")]
        [Authorize]
        public async Task<ActionResult> EditPlaylistName(int playlistId, [FromBody] string newName)
        {
            if (string.IsNullOrEmpty(newName))
            {
                return BadRequest("New Playlist Name cannot empty");
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var playlist = await _context.Playlists
                .Include(p => p.PlaylistSongs)
                .ThenInclude(ps => ps.Song)
                .FirstOrDefaultAsync(p => p.Id == playlistId && p.UserId == userId);
            if (playlist == null)
            {
                return NotFound("Playlist not found");
            }

            playlist.Name = newName;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Playlist name updated successfully" });
        }

        [HttpDelete("delete-user-playlist")]
        [Authorize]
        public async Task<ActionResult> DeletePlaylist(int playlistId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
            {
                return Unauthorized("User is not aunthentication");
            }

            var playlist = await _context.Playlists
                .Include(p => p.PlaylistSongs)
                .FirstOrDefaultAsync(p => p.Id == playlistId && p.UserId == userId);

            if (playlist == null)
            {
                return NotFound("Playlist not found");
            }

            _context.Playlists.Remove(playlist);
            await _context.SaveChangesAsync();

            return Ok(new { massage = "Playlist deleted successfully" });
        }

        [HttpPost("add-song")]
        [Authorize]
        public async Task<ActionResult> AddSongToPlaylist(int playlistId, [FromBody] List<int> songIds)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized("User is not authenticated");
            }
            if (songIds == null)
            {
                return BadRequest("SongIds cannot be empty");
            }

            var playlist = await _context.Playlists
                .Include(p => p.PlaylistSongs)
                .FirstOrDefaultAsync(p => p.Id == playlistId && p.UserId == userId);

            if (playlist == null)
            {
                return NotFound("Playlist not found");
            }

            var songs = await _context.Songs
                .Where(s => songIds.Contains(s.Id))
                .ToListAsync();
            if (songs.Count != songIds.Count)
            {
                return BadRequest("One or more song IDs are invalid.");
            }

            foreach (var song in songs)
            {
                if (!playlist.PlaylistSongs.Any(ps => ps.SongId == song.Id))
                {
                    playlist.PlaylistSongs.Add(new PlaylistSong
                    {
                        SongId = song.Id,
                        AddedAt = DateTime.UtcNow
                    });

                }
            }
            await _context.SaveChangesAsync();
            return Ok(new { message = "Song added to playlist" });
        }

        [HttpPost("remove-song")]
        public async Task<IActionResult> RemoveSongsFromPlaylist(int playlistId, [FromBody] List<int> songIds)
        {
            if (songIds == null || !songIds.Any())
                return BadRequest("No song IDs provided.");

            var playlist = await _context.Playlists
                .Include(p => p.PlaylistSongs)
                .FirstOrDefaultAsync(p => p.Id == playlistId);

            if (playlist == null)
                return NotFound();

            playlist.PlaylistSongs.RemoveAll(s => songIds.Contains(s.SongId));

            await _context.SaveChangesAsync();
            return Ok();
        }

    }
}
