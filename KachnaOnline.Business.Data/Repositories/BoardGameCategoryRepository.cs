// BoardGameCategoryRepository.cs
// Author: František Nečas

using KachnaOnline.Business.Data.Repositories.Abstractions;
using KachnaOnline.Data;
using KachnaOnline.Data.Entities.BoardGames;

namespace KachnaOnline.Business.Data.Repositories
{
    public class BoardGameCategoryRepository : GenericRepository<Category, int>, IBoardGameCategoryRepository
    {
        public BoardGameCategoryRepository(AppDbContext dbContext) : base(dbContext)
        {
        }
    }
}
