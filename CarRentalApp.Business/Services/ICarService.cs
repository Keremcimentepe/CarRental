using CarRentalApp.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CarRentalApp.Business.Services;

public interface ICarService
{
    Task<IEnumerable<Car>> GetAllCarsAsync();
    Task<Car> GetCarByIdAsync(int id);
    Task AddCarAsync(Car car);
    Task UpdateCarAsync(Car car);
    Task DeleteCarAsync(int id);
    Task<IEnumerable<Car>> GetPendingCarsAsync();
    Task ApproveCarAsync(int id);
    
    // Proje isterindeki "Gelişmiş Arama İşlemi" için güncellenen metodumuz
    Task<(IEnumerable<Car> Cars, int TotalCount)> SearchCarsAsync(string brand, string model, decimal? minPrice, decimal? maxPrice, bool? isAvailable, int page, int pageSize,string sortOrder);
}