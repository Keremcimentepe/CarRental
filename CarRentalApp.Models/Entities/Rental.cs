namespace CarRentalApp.Models.Entities;

public class Rental
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User User { get; set; }
    
    public int CarId { get; set; }
    public Car Car { get; set; }
    
    public System.DateTime StartDate { get; set; }
    public System.DateTime EndDate { get; set; }
    public decimal TotalPrice { get; set; }        // Kullanıcının ödediği brüt tutar
public decimal CommissionAmount { get; set; } // Şirketin aldığı %10 pay
public decimal SellerAmount { get; set; }     // Satıcıya kalan %90 pay
}