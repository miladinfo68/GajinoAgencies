using GajinoAgencies.DI;
using GajinoAgencies.Utilities;
using Microsoft.AspNetCore.Mvc.Authorization;



var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
var appSettingsFile = env switch
{
    "Development" => "appsettings.Development.json",
    "Staging" => "appsettings.Staging.json",
    _ => "appsettings.json"
};

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile(appSettingsFile, optional: false, reloadOnChange: true).AddEnvironmentVariables();


builder.Services.AddRouting(options => { options.LowercaseUrls = true; });

builder.Services.AddCors();

builder.Services.AddControllers(options =>
{
    options.Filters.Add(new AuthorizeFilter());
});

builder.Services.AddApplicationServices(builder.Configuration);

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/V1/swagger.json", "Gajino Celler Agency Pannel");
});

app.UseHttpsRedirection();

app.UseCors(x => x
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader());


app.UseRouting();

app.UseMiddleware<JwtMiddleware>();
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

//app.UseMiddleware<JwtMiddleware>();

app.MapControllers();

//app.UseMiddleware<ExceptionHandlingMiddleware>();

app.Run();


