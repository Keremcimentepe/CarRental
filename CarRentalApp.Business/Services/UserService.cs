using CarRentalApp.DataAccess.Repositories;
using CarRentalApp.Models.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CarRentalApp.Business.Services;

public class UserService : IUserService
{
    private readonly IRepository<User> _userRepository;

    public UserService(IRepository<User> userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<IEnumerable<User>> GetAllUsersAsync()
    {
        return await _userRepository.GetAllAsync();
    }

    public async Task<User> GetUserByIdAsync(int id)
    {
        return await _userRepository.GetByIdAsync(id);
    }

    public async Task AddUserAsync(User user)
    {
        await _userRepository.AddAsync(user);
    }

    public async Task UpdateUserAsync(User user)
    {
        await _userRepository.UpdateAsync(user);
    }

    public async Task DeleteUserAsync(int id)
    {
        await _userRepository.DeleteAsync(id);
    }

    // Kullanıcı adı ve şifre doğrulaması yapan metod
    public async Task<User> LoginAsync(string username, string password)
    {
        var users = await _userRepository.GetAllAsync();
        // Eşleşen kullanıcıyı bul, bulamazsa null döner
        return users.FirstOrDefault(u => u.Username == username && u.Password == password);
    }
}