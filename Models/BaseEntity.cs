namespace GajinoAgencies.Models;

public class BaseEntity
{
    public int Id { get; set; }
    public DateTime CreationDate { get; set; } = DateTime.Now;
    public bool IsActive { get; set; } = true;
}