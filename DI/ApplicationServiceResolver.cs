using GajinoAgencies.Data;
using GajinoAgencies.Services;
using GajinoAgencies.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using FluentValidation;
using FluentValidation.AspNetCore;
using GajinoAgencies.Dtos;
using GajinoAgencies.Mapping;
using Microsoft.Data.SqlClient;
using System.Data;
using StackExchange.Redis;
using GajinoAgencies.Utilities;
using Microsoft.AspNetCore.Authorization;


namespace GajinoAgencies.DI;

public static class ApplicationServiceResolver
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        //Binding Settings
        //services.Configure<AppSettings>(configuration.GetSection(nameof(AppSettings)));
        services.Configure<JwtSettings>(configuration.GetSection(nameof(JwtSettings)));
        services.Configure<CryptographySettings>(configuration.GetSection(nameof(CryptographySettings)));
        services.Configure<SmsProvider>(configuration.GetSection(nameof(SmsProvider)));
        services.Configure<OtpSettings>(configuration.GetSection(nameof(OtpSettings)));
        services.Configure<PackageCacheSettings>(configuration.GetSection(nameof(PackageCacheSettings)));
        services.Configure<AppConnectionStrings>(configuration.GetSection("ConnectionStrings"));


        //Adding Swagger configurations
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("V1", new OpenApiInfo
            {
                Version = "V1",
                Title = "Gajino Celler Agency Pannel",
                Description = "Gajino Celler Agency Pannel"
            });
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Name = "Authorization",
                Description = "Bearer Authentication with JWT Token",
                Type = SecuritySchemeType.Http
            });
            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Id = "Bearer",
                            Type = ReferenceType.SecurityScheme
                        }
                    },
                    new List<string>()
                }
            });
        });


        //Adding Jwt Authentication Settings
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = configuration["JwtSettings:Issuer"],
                ValidAudience = configuration["JwtSettings:Audience"],
                ClockSkew = TimeSpan.Zero,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JwtSettings:SecretKey"]))
            };
        });





        //Register Third party Services
        services.AddFluentValidationAutoValidation();
        services.AddValidatorsFromAssemblyContaining<AddAgentRequestDtoValidator>(); //method 1
        //services.AddValidatorsFromAssembly(typeof(AddAgentRequestDtoValidator).Assembly); //method 2
        //services.AddScoped<IValidator<AddAgentRequestDto>, AddAgentRequestDtoValidator>();
        //services.AddScoped<IValidator<AddNewVoucherDto>, AddNewVoucherDtoValidator>(); 

        //Register Other Services
        services.AddSingleton<IPasswordManagerService, PasswordManagerService>();
        services.AddScoped<ITokenManagerService, TokenManagerService>();
        services.AddScoped<IUserAccountService, UserAccountService>();
        services.AddScoped<IGaginoService, GaginoService>();
        services.AddScoped<IVoucherManagerService, VoucherManagerService>();
        services.AddScoped<ILocationService, LocationService>();
        services.AddScoped<IPaymentDocumentService, PaymentDocumentService>();



        services.AddAuthorization(options =>
        {
            options.AddPolicy("AdminPolicy", policy => policy.Requirements.Add(new AdminRequirement()));
        });

        services.AddScoped<IAuthorizationHandler, AdminAuthorizationHandler>();



        services.AddHttpClient("OtpClient", client =>
        {
            client.BaseAddress = new Uri(configuration["SmsProvider:BaseUrl"]!);
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            client.DefaultRequestHeaders.Add("Accept", "application/x-www-form-urlencoded");
        });
        services.AddScoped<IOtpService, OtpService>();

        //Register AutoMapper
        services.AddAutoMapper(typeof(AutoMapperProfile));


        //Register DbContexts
        services.AddDbContextPool<AgencyDbContext>(options =>
        {
            options.UseSqlServer(
                configuration.GetConnectionString("AgencyDb")!
                , sqlOptions => sqlOptions.EnableRetryOnFailure());
        });

        services.AddScoped<AgencyDbContext>(provider =>
        {
            var options = provider.GetRequiredService<DbContextOptions<AgencyDbContext>>();
            var passwordManager = provider.GetRequiredService<IPasswordManagerService>();
            return new AgencyDbContext(options, passwordManager);
        });


        services.AddSingleton<IDbConnection>(sp =>
            new SqlConnection(configuration.GetConnectionString("GajinoDb")));


        services.AddSingleton<IConnectionMultiplexer>(sp =>
        {
            var redisConnectionString = configuration.GetConnectionString("Redis")!;
            var config = ConfigurationOptions.Parse(redisConnectionString);
            //config.AbortOnConnectFail = false;  // Allow retrying
            return ConnectionMultiplexer.Connect(config);
        });

        services.AddScoped<ICacheManagerService>(sp =>
        {
            //var configuration = sp.GetRequiredService<IConfiguration>();
            var connectionMultiplexer = sp.GetRequiredService<IConnectionMultiplexer>();
            var cacheExpirationTime = TimeSpan.FromMinutes(int.Parse(configuration["JwtSettings:MinuteExpirationTime"]!));
            return new CacheManagerService(connectionMultiplexer, cacheExpirationTime);
        });

        return services;

    }
}
