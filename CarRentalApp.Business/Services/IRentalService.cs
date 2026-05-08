using CarRentalApp.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CarRentalApp.Business.Services;

public interface IRentalService
{
    Task RentCarAsync(Rental rental);
    Task<IEnumerable<Rental>> GetUserRentalsAsync(int userId);
    Task<IEnumerable<Rental>> GetAllRentalsAsync(); // Eksik olan buydu
}