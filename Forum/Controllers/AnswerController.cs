using Forum.DAL;
using Forum.Models;
using Microsoft.AspNetCore.Mvc;

public class AnswerController : Controller
{
    private readonly Repository<Answer> _answerRepository; // Answer tipine sahip Repository'i enjekte edin.

    public AnswerController(Repository<Answer> answerRepository)
    {
        _answerRepository = answerRepository;
    }

    // GET: Answer/Details/5
    public async Task<IActionResult> Details(int id)
    {
        var answer = await _answerRepository.GetById(id);
        if (answer == null)
        {
            return NotFound();
        }
        return View(answer);
    }

    // GET: Answer/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        var answer = await _answerRepository.GetById(id);
        if (answer == null)
        {
            return NotFound();
        }
        return View(answer);
    }

    // POST: Answer/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Content")] Answer answer)
    {
        if (id != answer.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                await _answerRepository.Update(answer);
            }
            catch
            {
                // Handle exception
            }
            return RedirectToAction("Details", new { id = answer.Id });
        }
        return View(answer);
    }
}
