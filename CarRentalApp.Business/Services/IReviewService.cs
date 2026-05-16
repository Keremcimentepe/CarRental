using CarRentalApp.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CarRentalApp.Business.Services;

public interface IReviewService
{
    Task AddReviewAsync(Review review);
    Task<IEnumerable<Review>> GetReviewsByCarIdAsync(int carId);
    Task<IEnumerable<Review>> GetReviewsByUserIdAsync(int userId);
}