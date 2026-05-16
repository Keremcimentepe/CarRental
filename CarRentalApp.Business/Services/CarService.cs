using CarRentalApp.DataAccess.Context;
using CarRentalApp.DataAccess.Repositories;
using CarRentalApp.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CarRentalApp.Business.Services;

public class CarService : ICarService
{
    private readonly IRepository<Car> _carRepository;
    private readonly AppDbContext _context;

    public CarService(IRepository<Car> carRepository, AppDbContext context)
    {
        _carRepository = carRepository;
        _context = context;
    }

    public async Task<IEnumerable<Car>> GetAllCarsAsync() => await _carRepository.GetAllAsync();
    
    public async Task<Car> GetCarByIdAsync(int id) => await _carRepository.GetByIdAsync(id);
    
    public async Task AddCarAsync(Car car) => await _carRepository.AddAsync(car);
    
    public async Task UpdateCarAsync(Car car) => await _carRepository.UpdateAsync(car);
    
    public async Task DeleteCarAsync(int id) => await _carRepository.DeleteAsync(id);

    // --- ARAMA VE FİLTRELEME (Sadece Onaylı Araçlar) ---
    public async Task<(IEnumerable<Car> Cars, int TotalCount)> SearchCarsAsync(string brand, string model, decimal? minPrice, decimal? maxPrice, bool? isAvailable, int page, int pageSize)
    {
        var query = _context.Cars.Include(c => c.Owner).Include(c => c.Rentals).Where(c => c.IsApproved).AsQueryable();
        // Veritabanından araçları, sahiplerini (Owner) ve kiralamalarını (Rentals) da dahil ederek çekiyoruz
        // 1. KURAL: Herkesin gördüğü ana sayfada SADECE onaylanmış araçlar listelensin
        query = query.Where(c => c.IsApproved == true); 
        int totalCount = await query.CountAsync();
        // 2. KURAL: Diğer filtrelemeler (Marka, model, fiyat vs.)
        if (!string.IsNullOrWhiteSpace(brand))
            query = query.Where(c => c.Brand.ToLower().Contains(brand.ToLower()));

        if (!string.IsNullOrWhiteSpace(model))
            query = query.Where(c => c.Model.ToLower().Contains(model.ToLower()));

        if (minPrice.HasValue)
            query = query.Where(c => c.DailyPrice >= minPrice.Value);

        if (maxPrice.HasValue)
            query = query.Where(c => c.DailyPrice <= maxPrice.Value);

        if (isAvailable.HasValue)
            query = query.Where(c => c.IsAvailable == isAvailable.Value);

        var cars = await query
        .OrderByDescending(c => c.Id) // Yeniler en üstte
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync();

        return (cars, totalCount);
    }

    // --- ADMİN ONAY SİSTEMİ METOTLARI ---
    
    // Onay bekleyen araçları listeler
    public async Task<IEnumerable<Car>> GetPendingCarsAsync()
    {
        return await _context.Cars
            .Include(c => c.Owner)
            .Where(c => c.IsApproved == false)
            .ToListAsync();
    }

    // Seçilen aracı onaylar ve yayına alır
    public async Task ApproveCarAsync(int id)
    {
        var car = await _context.Cars.FindAsync(id);
        if (car != null)
        {
            car.IsApproved = true;
            await _context.SaveChangesAsync();
        }
        
    }
    
}