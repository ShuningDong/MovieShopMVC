using MovieShop.Core.ApiModels.Response;
using MovieShop.Core.Entities;
using MovieShop.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MovieShop.Core.ServiceInterfaces
{
    public interface IMovieService
    {
        Task<IEnumerable<Movie>> GetHighestGrossingMoviesMovies();
        Task<MovieDetailsResponseModel> GetMovieByIdAsync(int id);
        Task<IEnumerable<Movie>> GetMoviesByGenre(int genreId);
        Task<PagedResultSet<MovieResponseModel>> GetMoviesByPagination(int pageSize = 20, int page = 0, string title = "");
    }
}
