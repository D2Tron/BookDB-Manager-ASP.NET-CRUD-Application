using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Testproj.Models;

namespace Testproj.Data
{
    public class BookContext : DbContext
    {
        public BookContext (DbContextOptions<BookContext> options)
            : base(options)
        {
        }

        public DbSet<Testproj.Models.Book> Book { get; set; } = default!;
        public DbSet<Testproj.Models.Author> Author { get; set; }
        public DbSet<Publisher> Publisher { get; set; }
    }
}
