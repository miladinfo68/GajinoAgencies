namespace GajinoAgencies.Models;

public class Location : BaseEntity
{
    public string Province { get; set; }
    public string City { get; set; }
    public string CityCode { get; set; }
    public Agency Agency { get; set; }
}