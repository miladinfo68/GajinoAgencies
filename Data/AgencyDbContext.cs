using GajinoAgencies.Models;
using GajinoAgencies.Services;
using Microsoft.EntityFrameworkCore;

namespace GajinoAgencies.Data;

public class AgencyDbContext : DbContext
{
    private readonly IPasswordManagerService _passwordManager;
    public AgencyDbContext(
        DbContextOptions<AgencyDbContext> options, 
        IPasswordManagerService passwordManager) : base(options)
    {
        _passwordManager = passwordManager;
    }

    public DbSet<Agency> Agencies { get; set; }
    public DbSet<Voucher> Vouchers { get; set; }
    public DbSet<Location> Locations { get; set; }
    public DbSet<Payment> Payments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.AddAgencyConfiguration(_passwordManager);
        modelBuilder.AddVoucherConfiguration();
        modelBuilder.AddLocationConfiguration();
        modelBuilder.AddPaymentConfiguration();
    }


}
