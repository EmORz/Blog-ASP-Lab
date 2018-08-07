using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BlogV3.Data;
using BlogV3.Models;
using Microsoft.AspNetCore.Authorization;

namespace BlogV3.Controllers
{
    public class ArticleController : Controller
    {
        private readonly ApplicationDbContext _context;


        private bool IsAuthorizeToEdit(Article article)
        {
            bool isAuthor = article.IsAuthor(this.User.Identity.Name);
            return isAuthor;
        }

        public ArticleController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Article
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Articles.Include(a => a.Author);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Article/Details/5
        [Authorize]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var article = await _context.Articles
                .Include(a => a.Author)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (article == null)
            {
                return NotFound();
            }

            return View(article);
        }

        // GET: Article/Create
        [Authorize]
        public IActionResult Create()
        {
            ViewData["AuthorId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: Article/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Content,AuthorId")] Article article)
        {
            if (ModelState.IsValid)
            {
                _context.Add(article);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AuthorId"] = new SelectList(_context.Users, "Id", "Id", article.AuthorId);
            return View(article);
        }

        // GET: Article/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
       

            var article = await _context.Articles.SingleOrDefaultAsync(m => m.Id == id);
            if (article == null)
            {
                return NotFound();
            }
          
            ViewData["AuthorId"] = new SelectList(_context.Users, "Id", "Id", article.AuthorId);
            var model = new ArticleViewModel();
            model.Id = article.Id;
            model.Title = article.Title;
            model.Content = article.Content;

     


            return View(model);
        }

        // POST: Article/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize]
        public ActionResult Edit( ArticleViewModel model)
        {            

            if (ModelState.IsValid)
            {
                var article = _context.Articles.FirstOrDefault(a => a.Id == model.Id);
                article.Title = model.Title;
                article.Content = model.Content;
                _context.Update(article);
                _context.SaveChanges();

                return RedirectToAction("Index");
            }
          
            return View(model);
        }

        // GET: Article/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var article = await _context.Articles
                .Include(a => a.Author)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (article == null)
            {
                return NotFound();
            }
            if (IsAuthorizeToEdit(article) == false)
            {
                return Forbid();
            }
            return View(article);
        }

        // POST: Article/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var article = await _context.Articles.SingleOrDefaultAsync(m => m.Id == id);
            _context.Articles.Remove(article);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ArticleExists(int id)
        {
            return _context.Articles.Any(e => e.Id == id);
        }
    }
}
