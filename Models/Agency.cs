namespace GajinoAgencies.Models;

public class Agency : BaseEntity
{
    public string Mobile { get; set; }
    public string Password { get; set; }
    public string Email { get; set; }
    public string Salt { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Institute { get; set; }
    public bool IsAdmin { get; set; } = false;
    public int LocationId { get; set; }
    public Location Location{ get; set; }
    public ICollection<Voucher> Vouchers { get; set; } = new List<Voucher>();
    public ICollection<Payment> Payments { get; set; } = new List<Payment>();
}