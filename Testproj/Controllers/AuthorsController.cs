using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Testproj.Data;
using Testproj.Models;

namespace Testproj.Controllers
{
    public class AuthorsController : Controller
    {
        private readonly BookContext _context;

        public AuthorsController(BookContext context)
        {
            _context = context;
        }

        // GET: Authors
        public async Task<IActionResult> Index(string sortOrder, string currentFilter, string searchString, int? pageNumber)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["AIDSortParm"] = sortOrder == "ID" ? "id_desc" : "";
            ViewData["LastSortParm"] = String.IsNullOrEmpty(sortOrder) ? "last_desc" : "Last";
            ViewData["FirstSortParm"] = String.IsNullOrEmpty(sortOrder) ? "first_desc" : "First";

            if (searchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewData["CurrentFilter"] = searchString;


            var authors = from a in _context.Author
                        select a;

            if (!String.IsNullOrEmpty(searchString))
            {
                authors = authors.Where(b => b.LastName.Contains(searchString));
            }
            switch (sortOrder)
            {
                case "id_desc":
                    authors = authors.OrderByDescending(b => b.AuthorID);
                    break;
                case "first_desc":
                    authors = authors.OrderByDescending(b => b.FirstName);
                    break;
                case "First":
                    authors = authors.OrderBy(b => b.FirstName);
                    break;
                case "Last":
                    authors = authors.OrderBy(b => b.LastName);
                    break;
                case "last_desc":
                    authors = authors.OrderByDescending(b => b.LastName);
                    break;
                default:
                    authors = authors.OrderBy(b => b.AuthorID);
                    break;
            }

            int pageSize = 5;
            return View(await PaginatedList<Author>.CreateAsync(authors.AsNoTracking(), pageNumber ?? 1, pageSize));
        }

        // GET: Authors/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Author == null)
            {
                return NotFound();
            }

            var author = await _context.Author
                .FirstOrDefaultAsync(m => m.AuthorID == id);
            if (author == null)
            {
                return NotFound();
            }

            return View(author);
        }

        // GET: Authors/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Authors/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AuthorID,FirstName,LastName")] Author author)
        {
            if (ModelState.IsValid)
            {
                _context.Add(author);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(author);
        }

        // GET: Authors/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Author == null)
            {
                return NotFound();
            }

            var author = await _context.Author.FindAsync(id);
            if (author == null)
            {
                return NotFound();
            }
            return View(author);
        }

        // POST: Authors/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AuthorID,FirstName,LastName")] Author author)
        {
            if (id != author.AuthorID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(author);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AuthorExists(author.AuthorID))
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
            return View(author);
        }

        // GET: Authors/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Author == null)
            {
                return NotFound();
            }

            var author = await _context.Author
                .FirstOrDefaultAsync(m => m.AuthorID == id);
            if (author == null)
            {
                return NotFound();
            }

            return View(author);
        }

        // POST: Authors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Author == null)
            {
                return Problem("Entity set 'BookContext.Author'  is null.");
            }
            var author = await _context.Author.FindAsync(id);
            if (author != null)
            {
                _context.Author.Remove(author);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AuthorExists(int id)
        {
          return _context.Author.Any(e => e.AuthorID == id);
        }
    }
}
