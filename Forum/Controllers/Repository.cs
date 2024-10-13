using System.Collections.Generic;
using System.Threading.Tasks;

namespace Forum.DAL
{
    public interface Repository<T> where T : class
    {
        Task<IEnumerable<T>> GetAll();
        Task<T> GetById(int id);
        Task Add(T entity);
        Task Update(T entity);
        Task Delete(T entity);
    }
}
