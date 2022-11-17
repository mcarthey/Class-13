using Microsoft.EntityFrameworkCore;
using MovieLibraryEntities.Context;
using MovieLibraryEntities.Models;

namespace MovieLibraryEntities.Dao
{
    public class Repository : IRepository, IDisposable
    {
        private readonly IDbContextFactory<MovieContext> _contextFactory;
        private readonly MovieContext _context;

        public Repository(IDbContextFactory<MovieContext> contextFactory)
        {
            _contextFactory = contextFactory;
            _context = _contextFactory.CreateDbContext();
        }

        public long Add(Movie movie)
        {
            _context.Movies.Add(movie);
            _context.SaveChanges();

            return movie.Id;
        }

        public void Delete(Movie movieToDelete)
        {
            _context.Movies.Remove(movieToDelete);
            _context.SaveChanges();
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        public IEnumerable<Movie> GetAll()
        {
            return _context.Movies.ToList();
        }

        public Movie GetById(int id)
        {
            return _context.Movies.FirstOrDefault(x => x.Id == id);
        }

        public IEnumerable<Movie> Search(string searchString)
        {
            // ToList() is only necessary in the case of using Lazy Loading in your context
            //var allMovies = _context.Movies;
            //var listOfMovies = allMovies.ToList();
            //return _context.Movies.Where(x => x.Title.Contains(searchString, StringComparison.CurrentCultureIgnoreCase));

            var lowerSearchString = searchString.ToLower();
            var temp = _context.Movies
                .Include(x => x.MovieGenres)
                .ThenInclude(x => x.Genre)
                .Where(x =>  x.Title.ToLower().Contains(lowerSearchString));

            return temp;
        }

        public void Update(Movie movieToUpdate)
        {
            _context.Movies.Update(movieToUpdate);
            _context.SaveChanges();
        }

    }
}
