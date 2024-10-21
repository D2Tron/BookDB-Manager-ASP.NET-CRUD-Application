using System.ComponentModel.DataAnnotations;

namespace Testproj.Models
{
    public class Book
    {
        [Key]
        public int ISBN { get; set; }
        [Required]
        public string? Title { get; set; }
        public double Price { get; set; }
        [Required]
        public string? Genre { get; set; }
        public string[] Genres = new[] { "Non-Fiction", "Fantasy", "Romance", "Sci-Fi", "Other" };

        public int AuthorID { get; set; }
        public virtual Author? Author { get; set; }

        public int PublisherID { get; set; }
        public virtual Publisher? Publisher { get; set; }
    }

    public class Author
    {
        public int AuthorID { get; set; }
        [Required]
        public string? FirstName { get; set; }
        [Required]
        public string? LastName { get; set; }

    }

    public class Publisher
    {
        public int PublisherID { get; set; }
        [Required]
        public string? Name { get; set; }
        [Required]
        public string? City { get; set; }
    }
}
