using DropBeatAPI.Core.DTOs.Beat;
using DropBeatAPI.Core.DTOs.Tag;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DropBeatAPI.Core.Interfaces
{
    public interface IBeatService
    {
        Task<Guid> AddBeatAsync(CreateBeatDto dto, Guid sellerId);
        Task<bool> DeleteBeatAsync(Guid beatId, Guid sellerId);
        Task<List<ShortBeatDto>> GetAllBeatsAsync();
        Task<ShortBeatDto?> GetShortBeatByIdAsync(Guid beatId);
        Task<BeatDto?> GetBeatByIdAsync(Guid beatId);

        Task<List<TagDto>> GetRandomTagsAsync();
        Task<List<TagDto>> SearchTagsAsync(string searchString);
        Task<List<FilteredBeatDto>> GetFilteredBeatsAsync(FilterBeatsDto filterDto);
    }
}
