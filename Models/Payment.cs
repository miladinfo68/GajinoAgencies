namespace GajinoAgencies.Models;

public class Payment : BaseEntity
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime PaymentDate { get; set; }
    public decimal Deposit { get; set; }
    public string AccountNo { get; set; }
    public string TraceNo { get; set; }
    public int AgencyId { get; set; }
    public Agency Agency { get; set; }
}