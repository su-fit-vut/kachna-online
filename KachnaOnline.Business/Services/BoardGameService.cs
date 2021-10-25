// BoardGameService.cs
// Author: František Nečas

using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using KachnaOnline.Business.Data.Repositories.Abstractions;
using KachnaOnline.Business.Models.BoardGames;
using KachnaOnline.Business.Services.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace KachnaOnline.Business.Services
{
    public class BoardGameService : IBoardGameService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<BoardGameService> _logger;
        private readonly IBoardGameRepository _boardGameRepository;
        private readonly IBoardGameCategoryRepository _boardGameCategoryRepository;

        public BoardGameService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<BoardGameService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _boardGameRepository = _unitOfWork.BoardGames;
            _boardGameCategoryRepository = _unitOfWork.BoardGameCategories;
        }

        /// <inheritdoc />
        public async Task<ICollection<BoardGame>> GetBoardGamesInCategory(int categoryId)
        {
            var boardGames = _boardGameRepository.GetByCategory(categoryId);
            var result = new List<BoardGame>();
            await foreach (var game in boardGames)
            {
                result.Add(_mapper.Map<BoardGame>(game));
            }
            return result;
        }

        /// <inheritdoc />
        public async Task<ICollection<BoardGame>> GetBoardGames()
        {
            var boardGames = await _boardGameRepository.All().Include(b => b.Category).ToListAsync();
            return _mapper.Map<List<BoardGame>>(boardGames);
        }

        /// <inheritdoc />
        public async Task<ICollection<Category>> GetBoardGameCategories()
        {
            var categories = await _boardGameCategoryRepository.All().ToListAsync();
            return _mapper.Map<List<Category>>(categories);
        }
    }
}
