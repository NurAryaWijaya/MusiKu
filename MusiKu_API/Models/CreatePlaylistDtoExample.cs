using Swashbuckle.AspNetCore.Filters;
using System.Xml.Linq;
using MusiKu_API.Models;
namespace MusiKu_API.Models
{
    public class CreatePlaylistDtoExample : IExamplesProvider<CreatePlaylistDto>
    {
        public CreatePlaylistDto GetExamples ()
        {
            return new CreatePlaylistDto
            {
                Name = "",
                SongIds = new List<int> { 1, 2, 3 }
            };
        }
    }
}
