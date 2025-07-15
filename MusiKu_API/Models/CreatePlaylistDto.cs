using System.ComponentModel.DataAnnotations;

namespace MusiKu_API.Models
{
    public class CreatePlaylistDto
    {
        public string Name { get; set; }
        [Required]
        [MinLength(1, ErrorMessage = "At least one song ID must be provided.")]
        public List<int> SongIds { get; set; } = new List<int>();
    }
}
