using CarRentalApp.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CarRentalApp.Business.Services;

public interface IUserService
{
    Task<IEnumerable<User>> GetAllUsersAsync();
    Task<User> GetUserByIdAsync(int id);
    Task AddUserAsync(User user);
    Task UpdateUserAsync(User user);
    Task DeleteUserAsync(int id);
    
    // Hocanın istediği Login (Giriş) mekanizması için özel metodumuz
    Task<User> LoginAsync(string username, string password);
}