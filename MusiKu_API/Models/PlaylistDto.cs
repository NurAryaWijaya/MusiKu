namespace MusiKu_API.Models
{
    public class PlaylistDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Song> Songs { get; set; }
    }
}
