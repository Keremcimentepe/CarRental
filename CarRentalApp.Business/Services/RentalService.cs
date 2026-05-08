using CarRentalApp.DataAccess.Context;
using CarRentalApp.DataAccess.Repositories;
using CarRentalApp.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CarRentalApp.Business.Services;

public class RentalService : IRentalService
{
    private readonly IRepository<Rental> _rentalRepository;
    private readonly IRepository<Car> _carRepository;
    private readonly AppDbContext _context;

    public RentalService(IRepository<Rental> rentalRepository, IRepository<Car> carRepository, AppDbContext context)
    {
        _rentalRepository = rentalRepository;
        _carRepository = carRepository;
        _context = context;
    }

    public async Task RentCarAsync(Rental rental)
    {
        await _rentalRepository.AddAsync(rental);

        var car = await _carRepository.GetByIdAsync(rental.CarId);
        if (car != null)
        {
            car.IsAvailable = false;
            await _carRepository.UpdateAsync(car);
        }
    }

    public async Task<IEnumerable<Rental>> GetUserRentalsAsync(int userId)
    {
        return await _context.Rentals
            .Include(r => r.Car)
            .Where(r => r.UserId == userId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Rental>> GetAllRentalsAsync()
    {
        return await _context.Rentals
            .Include(r => r.Car)
                .ThenInclude(c => c.Owner)
            .Include(r => r.User)
            .ToListAsync();
    }
}