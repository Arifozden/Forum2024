﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Forum.Data;
using Forum.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Forum.Controllers
{
    public class QuestionsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public QuestionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Questions
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Questions.Include(q => q.User).Include(a=>a.Answers);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Questions
        public async Task<IActionResult> List(int? id,string l)
        {
            var questions = await _context.Questions
                .Include(q => q.User)
                .Include(c => c.Answers)
                .ThenInclude(q => q.User)
                .Where(q => id == null || q.Id == id)
                .ToListAsync(); // İlgili soruları bir liste olarak alın;

            if (!string.IsNullOrEmpty(l))
            {
                // Arama terimini küçük harfe çevirin ve filtreleme işlemini gerçekleştirin
                l = l.ToLower();
                questions = questions.Where(q =>
                        q.Title.ToLower().Contains(l) ||
                        q.Description.ToLower().Contains(l))
                    .ToList();
            }

            return View("Index", questions);
        }

        public async Task<IActionResult> Filter(int? id, string d, string category)
        {
            var questions = _context.Questions
                .Include(q => q.User)
                .Include(c => c.Answers)
                .ThenInclude(q => q.User)
                .Where(q => id == null || q.Id == id)
                ; // İlgili soruları bir liste olarak alın; 

            if (!string.IsNullOrEmpty(d))
            {
                d = d.ToLower();
                questions = questions.Where(q =>
                    q.Title.ToLower().Contains(d) ||
                    q.Description.ToLower().Contains(d));
            }

            if (!string.IsNullOrEmpty(category))
            {
                questions = questions.Where(q => q.Category == category);
            }
            var questionList = questions.ToList();
            return View("Index", questionList);
        }

        // GET: Questions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            
            if (id == null || _context.Questions == null)
            {
                return NotFound();
            }

            var question = await _context.Questions
                .Include(q => q.User)
                .Include(c => c.Answers)
                .ThenInclude(q => q.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (question == null)
            {
                return NotFound();
            }

            return View(question);
        }

        // GET: Questions/Create
        [Authorize]
        public IActionResult Create()
        {
            
            return View();
        }

        // POST: Questions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Description,Category,IdentityUserId")] Question question)
        {
            if (ModelState.IsValid)
            {
                _context.Add(question);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            
            return View(question);
        }
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddAnswer([Bind("Id,Content,QuestionId, IdentityUserId")] Answer answer) //string returnUrl
        {
            if (ModelState.IsValid)
            {
                _context.Add(answer);
                await _context.SaveChangesAsync();
            }
            var question =await _context.Questions
                .Include(q => q.User)
                .Include(a => a.Answers)
                .ThenInclude(q => q.User)
                .FirstOrDefaultAsync(q => q.Id == answer.QuestionId);

             return View("Details", question);
            //return LocalRedirect(returnUrl);
        }

        // GET: Questions/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Questions == null)
            {
                return NotFound();
            }

            var question = await _context.Questions.FindAsync(id);

            if (question.IdentityUserId != User.FindFirstValue(ClaimTypes.NameIdentifier))
            {
                return Forbid();
            }
            ViewData["IdentityUserId"] = new SelectList(_context.Users, "Id", "Id", question.IdentityUserId);
            return View(question);
        }

        // POST: Questions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,Category,IdentityUserId")] Question question)
        {
            if (id != question.Id)
            {
                return NotFound();
            }
            if (question.IdentityUserId != User.FindFirstValue(ClaimTypes.NameIdentifier))
            {
                return Forbid();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(question);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!QuestionExists(question.Id))
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
            
            ViewData["IdentityUserId"] = new SelectList(_context.Users, "Id", "Id", question.IdentityUserId);
            return View(question);
        }

        // GET: Questions/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Questions == null)
            {
                return NotFound();
            }

            var question = await _context.Questions
                .Include(q => q.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (question == null)
            {
                return NotFound();
            }
            if (question.IdentityUserId != User.FindFirstValue(ClaimTypes.NameIdentifier))
            {
                return Forbid();
            }

            return View(question);
        }

        // POST: Questions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Questions == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Questions'  is null.");
            }

            var question = await _context.Questions.FindAsync(id);

            if (question == null)
            {
                return NotFound();
            }

           
            if (question.IdentityUserId != User.FindFirstValue(ClaimTypes.NameIdentifier))
            {
                return Unauthorized(); 
            }

            
            var answersToDelete = await _context.Answers.Where(a => a.QuestionId == question.Id).ToListAsync();
            _context.Answers.RemoveRange(answersToDelete);

            _context.Questions.Remove(question);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool QuestionExists(int id)
        {
            return (_context.Questions?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
