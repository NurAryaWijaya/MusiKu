using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using CsvHelper.Configuration.Attributes;

namespace MusiKu_API.Models
{
    public class Song
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("title")]
        public string Title { get; set; }

        [Column("artist")]
        public string Artist { get; set; }

        [Column("play_count")]
        public int PlayCount { get; set; }

        [Column("likes")]
        public int Likes { get; set; }

        [Column("mp3_url")]
        public string Mp3Url { get; set; }

        [Column("cover_url")]
        public string CoverUrl { get; set; }
    }
}
