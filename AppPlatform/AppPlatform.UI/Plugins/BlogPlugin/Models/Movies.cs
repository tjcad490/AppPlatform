using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BlogPlugin.Models
{
    public class Movies
    {
        private static List<Movie> _movies;
        static Movies()
        {
            _movies = new List<Movie>();
            _movies.Add(new Movie() { Name = "M1", Rating = 5 });
            _movies.Add(new Movie() { Name = "M2", Rating = 5 });
        }

        public List<Movie> GetMovies()
        {
            return _movies;
        }
    }

    public class Movie
    {
        public string Name { get; set; }
        public int Rating { get; set; }
    }
}