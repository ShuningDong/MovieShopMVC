using MovieShop.Core.ApiModels.Request;
using MovieShop.Core.ApiModels.Response;
using MovieShop.Core.Entities;
using MovieShop.Core.RepositoryInterfaces;
using MovieShop.Core.ServiceInterfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MovieShop.Infrastructure.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly ICryptoService _cryptoService;
        private readonly IAsyncRepository<Purchase> _purchaseRepository;
        public UserService(IUserRepository userRepository, ICryptoService cryptoService, IAsyncRepository<Purchase> purchaseRepository)
        {
            _userRepository = userRepository;
            _cryptoService = cryptoService;
            _purchaseRepository = purchaseRepository;
        }
        public async Task<UserRegisterResponseModel> CreateUser(UserRegisterRequestModel requestModel)
        {

            // 1. Call GetUserByEmail  with  requestModel.Email to check if the email exists in the User Table or not
            // if user/email exists return Email already exists and throw an Conflict exception

            // if email does not exists then we can proceed in creating the User record
            // 1. var salt =  Genreate a random salt
            // 2. var hashedPassword =  we take requestModel.Password and add Salt from above step and Hash them to generate Unique Hash
            // 3. Save Email, Salt, hashedPassword along with other details that user sent like FirstName, LastName etc
            // 4. return the UserRegisterResponseModel object with newly craeted Id for the User

            var dbUser = await _userRepository.GetUserByEmail(requestModel.Email);
            if (dbUser != null)
            {
                throw new Exception("Email alreadyy exists");
            }

            var salt = _cryptoService.CreateSalt();
            var hashedPassword = _cryptoService.HashPassword(requestModel.Password, salt);

            var user = new User
            {
                Email = requestModel.Email,
                Salt = salt,
                HashedPassword = hashedPassword,
                FirstName = requestModel.FirstName,
                LastName = requestModel.LastName
            };

            var createdUser = await _userRepository.AddAsync(user);

            var response = new UserRegisterResponseModel
            {
                Id = createdUser.Id,
                Email = requestModel.Email,
                FirstName = requestModel.FirstName,
                LastName = requestModel.LastName
            };
            return response;
        }

        public async Task<PurchaseResponseModel> GetAllPurchasedMoviesByUser(int id)
        {
            var movies = await _purchaseRepository.ListAllWithIncludesAsync(
                p => p.UserId == id,
                p => p.Movie
                );
            return GetAllPurchasedMovies(movies, id);
        }
        
        private PurchaseResponseModel GetAllPurchasedMovies(IEnumerable<Purchase> purchases, int id)
        {

            var movies = new List<PurchasedMovieResponseModel>();
            foreach(var pur in purchases)
            {
                movies.Add(new PurchasedMovieResponseModel
                {
                    Id = pur.MovieId,
                    PosterUrl = pur.Movie.PosterUrl,
                    PurchasedDateTime = pur.PurchaseDateTime,
                    Title = pur.Movie.Title
                });

            }
            return new PurchaseResponseModel { UserId = id, purchasedMovies = movies };

        }

        public async Task<User> GetUserByEmail(string email)
        {
            return await _userRepository.GetUserByEmail(email);
        }

        public async Task<User> ValidateUser(string email, string password)
        {
            // 1 Go to databse and get the whole record for this email, so that the object inlcudes salt and hashedpassword

            var user = await _userRepository.GetUserByEmail(email);
            if (user==null)
            {
                // User did not even registered in our Database
                return null;
            }

            // if  User Registered
            // hash the password with user entered password and database Salt
            // then compare the hashes
            var hashedPassword = _cryptoService.HashPassword(password, user.Salt);
            if (hashedPassword == user.HashedPassword)
            {
                return user;
            }
            else return null;
        }

        
    }
}
