using DropBeatAPI.Core.Entities;
using DropBeatAPI.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DropBeatAPI.Infrastructure.Repositories;

public class ChatSeeder
{
    private readonly BeatsDbContext _context;

    public ChatSeeder(BeatsDbContext context)
    {
        _context = context;
    }

    public async Task SeedGenreChatsAsync()
    {
        var genres = await _context.Genres.ToListAsync();

        foreach (var genre in genres)
        {
            var exists = await _context.Chats.AnyAsync(c => c.Type == ChatType.Genre && c.GenreId == genre.Id);
            if (!exists)
            {
                _context.Chats.Add(new Chat
                {
                    Id = Guid.NewGuid(),
                    Type = ChatType.Genre,
                    GenreId = genre.Id
                });
            }
        }

        await _context.SaveChangesAsync();
    }
}
