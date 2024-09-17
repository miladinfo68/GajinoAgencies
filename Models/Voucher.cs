namespace GajinoAgencies.Models;

public class Voucher : BaseEntity
{
  
    public string Title { get; set; }
    public string Code { get; set; }
    public byte Discount { get; set; }
    public int Count { get; set; }
    public int UnUsed { get; set; }
    public DateTime Expiration { get; set; }
    public string PackageDetailIds { get; set; }
    public int AgencyId { get; set; }
    public Agency Agency { get; set; }

}