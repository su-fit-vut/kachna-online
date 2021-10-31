// BoardGameCategoryRepository.cs
// Author: František Nečas

using System.Threading.Tasks;
using KachnaOnline.Business.Data.Repositories.Abstractions;
using KachnaOnline.Data;
using KachnaOnline.Data.Entities.BoardGames;
using Microsoft.EntityFrameworkCore;

namespace KachnaOnline.Business.Data.Repositories
{
    public class BoardGameCategoryRepository : GenericRepository<Category, int>, IBoardGameCategoryRepository
    {
        public BoardGameCategoryRepository(AppDbContext dbContext) : base(dbContext)
        {
        }

        public Task<Category> GetWithBoardGames(int categoryId)
        {
            return Set.Include(c => c.Games).FirstOrDefaultAsync(c => c.Id == categoryId);
        }
    }
}
