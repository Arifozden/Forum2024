using Forum.Data;
using Forum.Models;
using Microsoft.EntityFrameworkCore;

namespace Forum.DAL
{
    public class QuestionRepository : Repository<Question>
    {
        private readonly ApplicationDbContext _context;

        public QuestionRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Question>> GetAll()
        {
            return await _context.Questions.ToListAsync();
        }

        public async Task<Question> GetById(int id)
        {
            return await _context.Questions.FirstOrDefaultAsync(q => q.Id == id);
        }

        public async Task Add(Question entity)
        {
            _context.Questions.Add(entity);
            await _context.SaveChangesAsync();
        }

        public async Task Update(Question entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task Delete(Question entity)
        {
            _context.Questions.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }
}
