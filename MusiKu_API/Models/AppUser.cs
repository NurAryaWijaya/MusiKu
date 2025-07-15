using Microsoft.AspNetCore.Identity;
namespace MusiKu_API.Models
{
    public class AppUser : IdentityUser
    {
        public ICollection<Playlist> Playlists { get; set; }
    }
}
