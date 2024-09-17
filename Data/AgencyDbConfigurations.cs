using GajinoAgencies.Models;
using GajinoAgencies.Services;
using Microsoft.EntityFrameworkCore;

namespace GajinoAgencies.Data;

public static class AgencyDbConfigurations
{
    public static void AddAgencyConfiguration(this ModelBuilder modelBuilder, IPasswordManagerService passwordManager)
    {
        modelBuilder.Entity<Agency>(e =>
        {
            //these line not ganna work to change seed from 1000 instead we add empty migration 
            //and this line to up method migrationBuilder.Sql("DBCC CHECKIDENT ('Agencies', RESEED, 1000)");

            //----- 1 dotnet ef migrations add SetAgencyIdentitySeed -c AgencyDbContext
            //----- 2 add this line to up method in migration name SetAgencyIdentitySeed migrationBuilder.Sql("DBCC CHECKIDENT ('Agencies', RESEED, 1000)");


            //e.HasKey(p => p.Id).HasAnnotation("SqlServer:Identity", "1, 1000");
            //e.Property(p => p.Id).ValueGeneratedOnAdd().HasAnnotation("SqlServer:Identity", "1, 1000");

            e.HasKey(p => p.Id);
            e.Property(p => p.Mobile).HasColumnType("varchar(15)").IsRequired();
            //e.Property(p => p.Salt).HasColumnType("varchar(100)").IsRequired().HasDefaultValue(passwordManager.GetSalt);
            //e.Property(p => p.Password)
            //    .HasConversion(toSql => passwordManager.Encrypt(toSql),
            //        fromSql => passwordManager.Decrypt(fromSql));
            
            e.Property(p => p.Password).HasColumnType("varchar(256)").IsRequired();
            e.Property(p => p.LocationId).HasColumnType("int").IsRequired();
            e.Property(p => p.Salt).HasColumnType("varchar(256)").IsRequired();

            e.Property(p => p.Email).HasColumnType("varchar(100)").IsRequired(false);
            e.Property(p => p.FirstName).HasColumnType("nvarchar(100)").IsRequired(false);
            e.Property(p => p.LastName).HasColumnType("nvarchar(100)").IsRequired(false);
            e.Property(p => p.Institute).HasColumnType("nvarchar(100)").IsRequired(false);
            e.Property(p => p.IsActive).HasColumnType("bit").HasDefaultValue(true);
            e.Property(p => p.IsAdmin).HasColumnType("bit").HasDefaultValue(false);
            e.Property(p => p.CreationDate).HasColumnType("datetime2").HasDefaultValueSql("GetDate()");

            e.HasOne(nav => nav.Location)
            .WithOne(nav => nav.Agency)
            .HasForeignKey<Agency>(f => f.LocationId);


        });
    }


    public static void AddVoucherConfiguration(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Voucher>(e =>
        {
            e.HasKey(p => p.Id);
            e.Property(p => p.Title).HasColumnType("varchar(100)").IsRequired();
            e.Property(p => p.Code).HasColumnType("varchar(10)").IsRequired();
            e.Property(p => p.Discount).HasColumnType("tinyint").IsRequired();
            e.Property(p => p.Count).HasColumnType("int").IsRequired();
            e.Property(p => p.UnUsed).HasColumnType("int").HasDefaultValue(0);
            e.Property(p => p.Expiration).HasColumnType("datetime2");
            e.Property(p => p.PackageDetailIds).HasColumnType("varchar(200)").IsRequired();
            e.Property(p => p.IsActive).HasColumnType("bit").HasDefaultValue(true);
            e.Property(p => p.CreationDate).HasColumnType("datetime2").HasDefaultValueSql("GetDate()");

            e.HasOne(nav => nav.Agency)
                .WithMany(nav => nav.Vouchers)
                .HasForeignKey(f => f.AgencyId);
        });
    }

    public static void AddLocationConfiguration(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Location>(e =>
        {
            e.Property(p => p.CityCode).HasColumnType("char(3)")
                .HasConversion(
                    t => t.ToUpper(),
                    f => f.ToUpper()).IsRequired();

            e.Property(p => p.Province).HasColumnType("nvarchar(100)").IsRequired(false);
            e.Property(p => p.City).HasColumnType("nvarchar(100)").IsRequired(false);
            e.Property(p => p.IsActive).HasColumnType("bit").HasDefaultValue(true);
            e.Property(p => p.CreationDate).HasColumnType("datetime2").HasDefaultValueSql("GetDate()");

            e.HasData(new List<Location>
            {
                new (){Id = 1, CityCode = "THR",City = "تهران" ,Province = "تهران"},
                new (){Id = 2,CityCode = "HMD",City = "همدان" ,Province = "همدان"},
                new (){Id = 3,CityCode = "SHZ",City = "شیراز" ,Province = "شیراز"},
                new (){Id = 4,CityCode = "ESF",City = "اصفهان" ,Province = "اصفهان"},
                new (){Id = 5,CityCode = "YZD",City = "یزد" ,Province = "یزد"},
                new (){Id = 6,CityCode = "LOR",City = "لرستان" ,Province = "لرستان"},
                new (){Id = 7,CityCode = "KRJ",City = "کرج" ,Province = "کرج"},
            });
        });
    }

    public static void AddPaymentConfiguration(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Payment>(e =>
        {
            e.HasKey(p => p.Id);
            e.Property(p => p.FirstName).HasColumnType("nvarchar(100)").IsRequired();
            e.Property(p => p.LastName).HasColumnType("nvarchar(100)").IsRequired();
            e.Property(p => p.AccountNo).HasColumnType("varchar(20)").IsRequired();
            e.Property(p => p.Deposit).HasColumnType("decimal(18,2)").IsRequired();
            e.Property(p => p.TraceNo).HasColumnType("varchar(20)").IsRequired();
            e.Property(p => p.PaymentDate).HasColumnType("datetime2").IsRequired();
            e.Property(p => p.IsActive).HasColumnType("bit").HasDefaultValue(true);
            e.Property(p => p.CreationDate).HasColumnType("datetime2").HasDefaultValueSql("GetDate()");

            e.HasOne(a => a.Agency)
                .WithMany(p => p.Payments)
                .HasForeignKey(f => f.AgencyId);
        });
    }
}

