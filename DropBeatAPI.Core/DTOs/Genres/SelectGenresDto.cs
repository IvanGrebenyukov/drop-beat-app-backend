using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DropBeatAPI.Core.DTOs.Genres
{
    public class SelectGenresDto
    {
        [MinLength(1, ErrorMessage = "Выберите хотя бы один жанр")]
        public List<Guid> GenreIds { get; set; }
    }
}
