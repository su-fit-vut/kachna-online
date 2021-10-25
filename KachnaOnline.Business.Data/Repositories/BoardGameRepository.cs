// BoardGameRepository.cs
// Author: František Nečas

using System.Collections.Generic;
using System.Linq;
using KachnaOnline.Business.Data.Repositories.Abstractions;
using KachnaOnline.Data;
using KachnaOnline.Data.Entities.BoardGames;
using Microsoft.EntityFrameworkCore;

namespace KachnaOnline.Business.Data.Repositories
{
    public class BoardGameRepository : GenericRepository<BoardGame, int>, IBoardGameRepository
    {
        public BoardGameRepository(AppDbContext dbContext) : base(dbContext)
        {
        }

        public IAsyncEnumerable<BoardGame> GetByCategory(int categoryId)
        {
            return Set
                .Where(b => b.CategoryId == categoryId)
                .Include(b => b.Category)
                .AsAsyncEnumerable();
        }
    }
}
