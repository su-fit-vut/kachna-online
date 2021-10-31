// IBoardGamesCategoryRepository.cs
// Author: František Nečas

using System.Threading.Tasks;
using KachnaOnline.Data.Entities.BoardGames;

namespace KachnaOnline.Business.Data.Repositories.Abstractions
{
    public interface IBoardGamesCategoryRepository : IGenericRepository<Category, int>
    {
        Task<Category> GetWithBoardGames(int categoryId);
    }
}
