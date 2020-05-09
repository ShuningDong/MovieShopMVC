using MovieShop.Core.ApiModels.Response;
using MovieShop.Core.Entities;
using MovieShop.Core.Helpers;
using MovieShop.Core.RepositoryInterfaces;
using MovieShop.Core.ServiceInterfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using System.Linq.Expressions;

namespace MovieShop.Infrastructure.Services
{
    public class MovieService : IMovieService
    {
        private readonly IMovieRepository _movieRepository;
        //private readonly IAsyncRepository<Favorite> _favoriteRepository;

        public MovieService(IMovieRepository movieRepository)
        {
            _movieRepository = movieRepository;
        }

        //private readonly IAsyncRepository<Movie> _movieRepository;

        //public MovieService(IAsyncRepository<Movie> movieRepository)
        //{
        //    _movieRepository = movieRepository;
        //}

        public async Task<IEnumerable<Movie>> GetHighestGrossingMoviesMovies()
        {
            var movies = await _movieRepository.GetHighestGrossingMovies();
            //var response = _mapper.Map<IEnumerable<MovieResponseModel>>(movies);
            //var movies = await _movieRepository.OrderByDescending(m => m.Revenue).Take(50).ToListAsync();

            return movies;
        }

        public async Task<MovieDetailsResponseModel> GetMovieByIdAsync(int id)
        {

            var movie = await _movieRepository.GetByIdAsync(id);

            var casts = new List<CastResponseModel>();
            foreach (MovieCast ca in movie.MovieCasts)
            {
                casts.Add(new CastResponseModel
                {
                    Id = ca.CastId,
                    Name = ca.Cast.Name,
                    Gender = ca.Cast.Gender,
                    TmdbUrl = ca.Cast.TmdbUrl,
                    ProfilePath = ca.Cast.ProfilePath,
                    Character = ca.Character
                });
            }

            var movieDetails = new MovieDetailsResponseModel
            {
                Id = movie.Id,
                Title = movie.Title,
                Budget = movie.Budget,
                Overview = movie.Overview,
                Tagline = movie.Tagline,
                ImdbUrl = movie.ImdbUrl,
                Revenue = movie.Revenue,
                TmdbUrl = movie.TmdbUrl,
                PosterUrl = movie.PosterUrl,
                BackdropUrl = movie.BackdropUrl,
                OriginalLanguage = movie.OriginalLanguage,
                ReleaseDate = movie.ReleaseDate,
                RunTime = movie.RunTime,
                Price = movie.Price,
                Casts = casts,

            };


            // 
            // loop thru this object and add this movie.MovieCasts info to movieDetails.Casts
            //if (movie == null) throw new NotFoundException("Movie", id);
            //var favoritesCount = await _favoriteRepository.GetCountAsync(f => f.MovieId == id);
            //var response = _mapper.Map<MovieDetailsResponseModel>(movie);
            //response.FavoritesCount = favoritesCount;
            return movieDetails;
        }

        public async Task<IEnumerable<Movie>> GetMoviesByGenre(int genreId)
        {
            var movies = await _movieRepository.GetMoviesByGenre(genreId);
            return movies;
        }

        public async Task<PagedResultSet<MovieResponseModel>> GetMoviesByPagination(int pageSize = 20, int page = 0, string title = "")
        {
            // check if title parameter is null or empty, if not then construct a Expression with Contains method
            // contains method will transalate to SQL like 
                       
            Expression<Func<Movie, bool>> filterExpression = null;
            if (!string.IsNullOrEmpty(title))
            {
                filterExpression = movie => title != null && movie.Title.Contains(title);
            }

            
            //  // we are gonna call GetPagedData method from repository;
            // pass the order by column, here we are ordering our result by movie title
            // pass the above filter expression
            var pagedMovies = await _movieRepository.GetPagedData(page, pageSize, movies => movies.OrderBy(m => m.Title), filterExpression);

            // once you get movies from repository , convert them in to MovieResponseModel List
            var pagedMovieResponseModel = new List<MovieResponseModel>();

            foreach (var movie in pagedMovies)
            {
                pagedMovieResponseModel.Add(new MovieResponseModel
                {
                    Id = movie.Id,
                    Title = movie.Title,
                    PosterUrl = movie.PosterUrl,
                    ReleaseDate = movie.ReleaseDate.Value
                });
            }

            // Pass the List of MovieResponseModel to our PagedResultSet class so that it can display the data along with page numbers
            var movies = new PagedResultSet<MovieResponseModel>(pagedMovieResponseModel, page, pageSize, pagedMovies.TotalCount);
            return movies;
        }
    }
}
