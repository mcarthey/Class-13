using ConsoleTables;
using Microsoft.Extensions.Logging;
using MovieLibraryEntities.Context;
using MovieLibraryEntities.Dao;
using MovieLibraryEntities.Models;
using MovieLibraryOO.Dao;
using MovieLibraryOO.Dto;
using MovieLibraryOO.Mappers;
using System;

namespace MovieLibraryOO.Services
{
    public class MainService : IMainService
    {
        private readonly ILogger<MainService> _logger;
        private readonly IMovieMapper _movieMapper;
        private readonly IRepository _repository;
        private readonly IFileService _fileService;

        public MainService(ILogger<MainService> logger, IMovieMapper movieMapper, IRepository repository, IFileService fileService)
        {
            _logger = logger;
            _movieMapper = movieMapper;
            _repository = repository;
            _fileService = fileService;
        }


        public void Invoke()
        {
            var menu = new Menu();

            Menu.MenuOptions menuChoice;
            do
            {
                menuChoice = menu.ChooseAction();

                switch (menuChoice)
                {
                    case Menu.MenuOptions.ListFromDb:
                        _logger.LogInformation("Listing movies from database");
                        var allMovies = _repository.GetAll();
                        var movies = _movieMapper.Map(allMovies);
                        ConsoleTable.From<MovieDto>(movies).Write();
                        break;
                    case Menu.MenuOptions.ListFromFile:
                        _fileService.Read();
                        _fileService.Display();
                        break;
                    case Menu.MenuOptions.Add:
                        _logger.LogInformation("Adding a new movie");

                        Console.WriteLine("Enter the movie title");
                        var movieTitle = Console.ReadLine();

                        Console.WriteLine("Enter the release date");
                        var movieRelease = Console.ReadLine();
                        var movieReleaseDate = DateTime.Parse(movieRelease);

                        var movie = new Movie()
                        {
                            Title = movieTitle,
                            ReleaseDate = movieReleaseDate
                        };

                        var addedId = _repository.Add(movie);

                        Console.WriteLine($"Your movie {movie.Title} was added with Id {addedId}");

                        break;
                    case Menu.MenuOptions.Update:
                        _logger.LogInformation("Updating an existing movie");

                        // note that we are assuming the user knows the id - probably not a good choice
                        Console.WriteLine("Enter the MovieId to update");
                        var movieIdToUpdate = Convert.ToInt32(Console.ReadLine());
                        var movieToUpdate = _repository.GetById(movieIdToUpdate);

                        // note that we are assuming the user wants to update the title - again not good
                        Console.WriteLine($"Here is your movie {movieToUpdate.Title}");
                        Console.WriteLine("What do you want to change title to?");
                        var newTitle = Console.ReadLine();

                        movieToUpdate.Title = newTitle;

                        _repository.Update(movieToUpdate);

                        break;
                    case Menu.MenuOptions.Delete:
                        _logger.LogInformation("Deleting a movie");

                        Console.WriteLine("Enter the MovieId to delete");
                        var movieIdToDelete = Convert.ToInt32(Console.ReadLine());
                        var movieToDelete = _repository.GetById(movieIdToDelete);

                        _repository.Delete(movieToDelete);

                        break;
                    case Menu.MenuOptions.Search:
                        _logger.LogInformation("Searching for a movie");
                        var userSearchTerm = menu.GetUserResponse("Enter your", "search string:", "green");
                        var searchedMovies = _repository.Search(userSearchTerm);
                        // movies = _movieMapper.Map(searchedMovies);
                        //ConsoleTable.From<Movie>(searchedMovies).Write();

                        foreach (var mov in searchedMovies)
                        {
                            Console.Write($"{mov.Title}: ");

                            foreach (var gen in mov.MovieGenres)
                            {
                                Console.Write($"{gen.Genre.Name} ");
                            }
                            Console.WriteLine();
                        }
                        break;
                }
            }
            while (menuChoice != Menu.MenuOptions.Exit);

            menu.Exit();


            Console.WriteLine("\nThanks for using the Movie Library!");

        }
    }
}