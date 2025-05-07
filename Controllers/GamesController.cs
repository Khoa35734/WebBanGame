using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WEB_GAME_STORE.Models;
using System.Linq;

namespace WEB_GAME_STORE.Controllers
{
    public class GamesController : Controller
    {
        private readonly Pbl3Context _context;

        public GamesController(Pbl3Context context)
        {
            _context = context;
        }

        // GET: /Games
        public IActionResult Index(string searchString)
        {
            var games = from g in _context.TbGames.Include(g => g.IdCategoriesNavigation)
                        select g;

            if (!string.IsNullOrEmpty(searchString))
            {
                games = games.Where(g => g.TenGame.Contains(searchString));
            }

            return View(games.ToList());
        }

        // GET: /Games/Details/5
        public IActionResult Details(int id)
        {
            var game = _context.TbGames
                .Include(g => g.IdCategoriesNavigation)
                .FirstOrDefault(g => g.IdGame == id);

            if (game == null) return NotFound();
            return View(game);
        }

        // GET: /Games/Create
        public IActionResult Create()
        {
            ViewBag.Categories = _context.Categories.ToList();
            return View();
        }

        // POST: /Games/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(TbGame game)
        {
            if (ModelState.IsValid)
            {
                _context.TbGames.Add(game);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(game);
        }

        // GET: /Games/Edit/5
        public IActionResult Edit(int id)
        {
            var game = _context.TbGames.Find(id);
            if (game == null) return NotFound();
            ViewBag.Categories = _context.Categories.ToList();
            return View(game);
        }

        // POST: /Games/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, TbGame game)
        {
            if (id != game.IdGame) return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(game);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(game);
        }

        // GET: /Games/Delete/5
        public IActionResult Delete(int id)
        {
            var game = _context.TbGames.Find(id);
            if (game == null) return NotFound();
            return View(game);
        }

        // POST: /Games/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var game = _context.TbGames.Find(id);
            if (game != null)
            {
                _context.TbGames.Remove(game);
                _context.SaveChanges();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
