using CarRentalApp.Business.Services;
using CarRentalApp.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace CarRentalApp.Web.Controllers;

[Authorize]
public class ReviewController : Controller
{
    private readonly IReviewService _reviewService;

    public ReviewController(IReviewService reviewService)
    {
        _reviewService = reviewService;
    }

    [HttpPost]
    public async Task<IActionResult> Create(int carId, int rating, string comment)
    {
        var userId = int.Parse(User.FindFirst("UserId").Value);

        if (rating < 1 || rating > 5 || string.IsNullOrWhiteSpace(comment))
        {
            return RedirectToAction("Index", "Home"); // Basit doğrulama hatası koruması
        }

        var review = new Review
        {
            CarId = carId,
            UserId = userId,
            Rating = rating,
            Comment = comment
        };

        await _reviewService.AddReviewAsync(review);
        
        // Yorum yapıldıktan sonra aracın kiralama sayfasına veya ana sayfaya dönebiliriz
        return RedirectToAction("Index", "Home");
    }
}