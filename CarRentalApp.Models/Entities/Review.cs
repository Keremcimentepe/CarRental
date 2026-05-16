namespace CarRentalApp.Models.Entities;

public class Review
{
    public int Id { get; set; }
    public int CarId { get; set; }
    public Car Car { get; set; }
    public int UserId { get; set; }
    public User User { get; set; }
    public int Rating { get; set; } // 1-5 arası yıldız
    public string Comment { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);
}