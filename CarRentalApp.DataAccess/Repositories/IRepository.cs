using System.Collections.Generic;
using System.Threading.Tasks;

namespace CarRentalApp.DataAccess.Repositories;

// T, burada User, Car veya Rental olabilir. Generic (Genel) bir yapı kuruyoruz.
public interface IRepository<T> where T : class
{
    Task<IEnumerable<T>> GetAllAsync();
    Task<T> GetByIdAsync(int id);
    Task AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(int id);
}