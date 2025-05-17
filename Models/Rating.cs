using DeliveryAppSystem.Models;


public class Rating
{
    public int RatingID { get; set; }

    public int DriverID { get; set; }
    public Driver Driver { get; set; }

    public int CustomerID { get; set; }  // ✅ Changed to int
    public User Customer { get; set; }   // ✅ Navigation to User

    public int Score { get; set; }
    public string Comment { get; set; }
}
