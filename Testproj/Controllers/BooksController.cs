using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Testproj.Data;
using Testproj.Models;

namespace Testproj.Controllers
{
    public class BooksController : Controller
    {
        private readonly BookContext _context;

        public BooksController(BookContext context)
        {
            _context = context;
        }

        // GET: Books
        public async Task<IActionResult> Index(string sortOrder, string currentFilter, string searchString, int? pageNumber)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["ISBNSortParm"] = sortOrder == "ISBN" ? "isbn_desc" : "";
            ViewData["TitleSortParm"] = String.IsNullOrEmpty(sortOrder) ? "title_desc" : "Title";
            ViewData["PriceSortParm"] = sortOrder == "Price" ? "price_desc" : "Price";

            if (searchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewData["CurrentFilter"] = searchString;


            var books = from b in _context.Book
                        select b;

            if (!String.IsNullOrEmpty(searchString))
            {
                books = books.Where(b => b.Title.Contains(searchString)) ;
            }
            switch (sortOrder)
            {
                case "isbn_desc":
                    books = books.OrderByDescending(b => b.ISBN);
                    break;
                case "title_desc":
                    books = books.OrderByDescending(b => b.Title);
                    break;
                case "Title":
                    books = books.OrderBy(b => b.Title);
                    break;
                case "Price":
                    books = books.OrderBy(b => b.Price);
                    break;
                case "price_desc":
                    books = books.OrderByDescending(b => b.Price);
                    break;
                default:
                    books = books.OrderBy(b => b.ISBN);
                    break;
            }

            int pageSize = 5;
            return View(await PaginatedList<Book>.CreateAsync(books.AsNoTracking(), pageNumber ?? 1, pageSize));
        }

        // GET: Books/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Book == null)
            {
                return NotFound();
            }

            var book = await _context.Book
                .Include(b => b.Author)
                .Include(b => b.Publisher)
                .FirstOrDefaultAsync(m => m.ISBN == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // GET: Books/Create
        public IActionResult Create()
        {
            ViewData["AuthorID"] = new SelectList(_context.Set<Author>(), "AuthorID", "FirstName");
            ViewData["PublisherID"] = new SelectList(_context.Set<Publisher>(), "PublisherID", "City");
            return View();
        }

        // POST: Books/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ISBN,Title,Price,Genre,AuthorID,PublisherID")] Book book)
        {
            if (ModelState.IsValid)
            {
                _context.Add(book);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AuthorID"] = new SelectList(_context.Set<Author>(), "AuthorID", "FirstName", book.AuthorID);
            ViewData["PublisherID"] = new SelectList(_context.Set<Publisher>(), "PublisherID", "City", book.PublisherID);
            return View(book);
        }

        // GET: Books/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Book == null)
            {
                return NotFound();
            }

            var book = await _context.Book.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }
            ViewData["AuthorID"] = new SelectList(_context.Set<Author>(), "AuthorID", "FirstName", book.AuthorID);
            ViewData["PublisherID"] = new SelectList(_context.Set<Publisher>(), "PublisherID", "City", book.PublisherID);
            return View(book);
        }

        // POST: Books/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ISBN,Title,Price,Genre,AuthorID,PublisherID")] Book book)
        {
            if (id != book.ISBN)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(book);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookExists(book.ISBN))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["AuthorID"] = new SelectList(_context.Set<Author>(), "AuthorID", "FirstName", book.AuthorID);
            ViewData["PublisherID"] = new SelectList(_context.Set<Publisher>(), "PublisherID", "City", book.PublisherID);
            return View(book);
        }

        // GET: Books/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Book == null)
            {
                return NotFound();
            }

            var book = await _context.Book
                .Include(b => b.Author)
                .Include(b => b.Publisher)
                .FirstOrDefaultAsync(m => m.ISBN == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // POST: Books/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Book == null)
            {
                return Problem("Entity set 'BookContext.Book'  is null.");
            }
            var book = await _context.Book.FindAsync(id);
            if (book != null)
            {
                _context.Book.Remove(book);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BookExists(int id)
        {
          return _context.Book.Any(e => e.ISBN == id);
        }

        public IActionResult InitializeDatabase()
        {
            _context.Database.EnsureCreated();
            if (_context.Book.Any())
            {
                // return;
            }

            var authors = new Author[]
            {
                new Author{AuthorID=1, FirstName="James", LastName="Paterson" },
                new Author{AuthorID=2, FirstName="David", LastName="Baldacci" },
                new Author{AuthorID=3, FirstName="Colleen", LastName="Hoover" },
                new Author{AuthorID=4, FirstName="Nora", LastName="Roberts" },
                new Author{AuthorID=5, FirstName="C J", LastName="Box" }
            };

            foreach (Author a in authors)
            {
                _context.Author.Add(a);
            }
            //_context.SaveChanges();

            var publishers = new Publisher[]
            {
                new Publisher{PublisherID=1, Name="Penguin", City="London"},
                new Publisher{PublisherID=2, Name="Random House", City="New York"},
                new Publisher{PublisherID=3, Name="Pearson", City="London"},
                new Publisher{PublisherID=4, Name="Wiley", City="Hoboken"},
                new Publisher{PublisherID=5, Name="HarperCollins", City="New York"}
            };

            foreach (Publisher p in publishers)
            {
                _context.Publisher.Add(p);
            }
            //_context.SaveChanges();

            var books = new Book[]
                {
                    new Book{ISBN=123456789, Title="Diana, William, and Harry", Price=17.99, Genre="Non-Fiction", AuthorID=1, PublisherID=1},
                    new Book{ISBN=234567890, Title="Long Shadows", Price=16.49, Genre="Sci-Fi", AuthorID=2, PublisherID=2},
                    new Book{ISBN=345678901, Title="It Starts with Us", Price=37.99, Genre="Romance", AuthorID=3, PublisherID=3},
                    new Book{ISBN=456789012, Title="Imagine Us", Price=7.83, Genre="Romance", AuthorID=4, PublisherID=4},
                    new Book{ISBN=567890123, Title="Treasure State", Price=7.83, Genre="Other", AuthorID=5, PublisherID=5}
                };

            foreach (Book s in books)
            {
                _context.Book.Add(s);
            }
            _context.SaveChanges();
            
            return RedirectToAction(nameof(Index));
        }

        public IActionResult ClearDatabase()
        {
            _context.Database.ExecuteSqlRaw("DELETE FROM Book");
            _context.SaveChanges();

            _context.Database.ExecuteSqlRaw("DELETE FROM Author");
            _context.SaveChanges();

            _context.Database.ExecuteSqlRaw("DELETE FROM Publisher");
            _context.SaveChanges();

            //            conn = Configuration.GetConnectionString("DefaultConnection")

            return RedirectToAction(nameof(Index));
        }
    }
}
