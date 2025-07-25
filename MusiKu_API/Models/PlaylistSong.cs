﻿namespace MusiKu_API.Models
{
    public class PlaylistSong
    {
        public int PlaylistId { get; set; }
        public Playlist Playlist { get; set; }

        public int SongId { get; set; }
        public Song Song { get; set; }
        public DateTime AddedAt { get; set; } = DateTime.UtcNow;
    }
}
