using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MusiKu_API.Data;
using MusiKu_API.Models;
using System.Runtime.CompilerServices;
using System.Security.Claims;

namespace MusiKu_API.Controllers
{
    [Route("api/songs")]
    [ApiController]
    public class SongsController : ControllerBase
    {
        private readonly MusiKuContext _context;

        // Constructor untuk inject MusiKuContext
        public SongsController(MusiKuContext context)
        {
            _context = context;
        }

        // endpoint untuk mengakses via id
        [HttpGet("search-by-id")]
        public async Task<ActionResult<Song>> GetSongById(int id)
        {
            var song = await _context.Songs.FindAsync(id);
            if (song == null)
            {
                return NotFound();
            }
            return song;
        }

        // menampilkan semua lagu
        [HttpGet("view-all-songs")]
        public async Task<ActionResult<IEnumerable<Song>>> GetSongs()
        {
            var songs = await _context.Songs.ToListAsync();
            return Ok(songs);
        }

        // menambahkan lagu
        [HttpPost("add-song")]
        public async Task<ActionResult<Song>> PostSong(SongDto songDto)
        {
            var song = new Song
            {
                Title = songDto.Title,
                Artist = songDto.Artist,
                Mp3Url = songDto.Mp3Url,
                CoverUrl = songDto.CoverUrl,
                PlayCount = 0,
                Likes = 0
            };

            _context.Songs.Add(song);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetSongs), new { id = song.Id }, song);
        }


        [HttpDelete("delete-song/{id}")]
        public async Task<IActionResult> DeleteSong(int id)
        {
            try
            {
                var song = await _context.Songs.FindAsync(id);
                if (song == null)
                {
                    return NotFound($"Song with ID {id} not found.");
                }

                _context.Songs.Remove(song);
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest($"Exception: {ex.Message}");
            }
        }



        // update PlayCount
        [HttpPut("update-PlayCount")]
        public async Task<IActionResult> UpdateSong(int id, Song updateSong)
        {
            if (id != updateSong.Id)
            {
                return BadRequest();
            }

            var song = await _context.Songs.FindAsync(id);
            if (song == null)
            {
                return NotFound();
            }

            song.PlayCount = updateSong.PlayCount;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Songs.Any(e => e.Id == id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return NoContent();
        }
        // Update likes
        [HttpPut("update-Like")]
        public async Task<IActionResult> UpdateLike(int id, Song updateSong)
        {
            if (id != updateSong.Id)
            {
                return BadRequest();
            }

            var song = await _context.Songs.FindAsync(id);
            if (song == null)
            {
                return NotFound();
            }

            song.Likes = updateSong.Likes;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Songs.Any(e => e.Id == id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return NoContent();
        }
        [Authorize]
        [HttpPost("like")]
        public async Task<IActionResult> LikeSong([FromQuery] int songId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var liked = await _context.LikedSongs
                .AnyAsync(x => x.SongId == songId && x.UserId == userId);

            if (liked)
                return BadRequest("Already liked.");

            // Tambahkan like user
            _context.LikedSongs.Add(new LikedSong
            {
                SongId = songId,
                UserId = userId
            });

            // Tambahkan +1 ke jumlah likes
            var song = await _context.Songs.FindAsync(songId);
            if (song != null)
                song.Likes += 1;

            await _context.SaveChangesAsync();
            return Ok();
        }
        [Authorize]
        [HttpDelete("unlike")]
        public async Task<IActionResult> UnlikeSong([FromQuery] int songId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var like = await _context.LikedSongs
                .FirstOrDefaultAsync(x => x.SongId == songId && x.UserId == userId);

            if (like == null)
                return NotFound("Like tidak ditemukan");

            _context.LikedSongs.Remove(like);

            // ➖ Kurangi jumlah likes di tabel Songs
            var song = await _context.Songs.FindAsync(songId);
            if (song != null && song.Likes > 0)
                song.Likes -= 1;

            await _context.SaveChangesAsync();
            return Ok();
        }

        [Authorize]
        [HttpGet("is-liked")]
        public async Task<IActionResult> IsSongLiked([FromQuery] int songId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var liked = await _context.LikedSongs
                .AnyAsync(x => x.SongId == songId && x.UserId == userId);

            return Ok(liked);
        }
        [Authorize]
        [HttpGet("liked")]
        public async Task<ActionResult<IEnumerable<Song>>> GetLikedSongs()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User ID tidak valid");

            var likedSongIds = await _context.LikedSongs
                .Where(x => x.UserId == userId)
                .Select(x => x.SongId)
                .ToListAsync();

            var likedSongs = await _context.Songs
                .Where(song => likedSongIds.Contains(song.Id))
                .ToListAsync();

            return Ok(likedSongs);
        }
    }
}
