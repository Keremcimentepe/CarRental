using System.Collections.Generic;

namespace CarRentalApp.Models.Entities;

public class Car
{
    public int Id { get; set; }
    public string Brand { get; set; }
    public string Model { get; set; }
    public int Year { get; set; }
    public decimal DailyPrice { get; set; }
    public bool IsAvailable { get; set; }
    public byte[]? Image { get; set; } // Sonuna ? eklendi
    
    public int OwnerId { get; set; } 
    public User Owner { get; set; }

    // EKSİK OLAN SATIR BURASI (Aracın kiralama geçmişi)
    public ICollection<Rental> Rentals { get; set; } 
    public List<Review> Reviews { get; set; }
    public bool IsApproved { get; set; } = false; // Varsayılan olarak onay bekliyor
}