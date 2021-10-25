// IBoardGameRepository.cs
// Author: František Nečas

using System.Collections.Generic;
using KachnaOnline.Data.Entities.BoardGames;

namespace KachnaOnline.Business.Data.Repositories.Abstractions
{
    public interface IBoardGameRepository : IGenericRepository<BoardGame, int>
    {
        IAsyncEnumerable<BoardGame> GetByCategory(int categoryId);
    }

}
