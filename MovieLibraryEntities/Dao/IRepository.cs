using MovieLibraryEntities.Models;

namespace MovieLibraryEntities.Dao
{
    public interface IRepository
    {
        long Add(Movie movie);
        IEnumerable<Movie> GetAll();
        IEnumerable<Movie> Search(string searchString);

        Movie GetById(int id);
        void Update(Movie movieToUpdate);
        void Delete(Movie movieToDelete);
    }
}
