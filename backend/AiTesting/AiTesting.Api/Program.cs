using AiTesting.Application;
using AiTesting.Domain;
using AiTesting.Infrastructure;
using AiTesting.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddHttpContextAccessor();

builder.Services.AddDomain();
builder.Services.AddInfrastructure();
builder.Services.AddApplication();

builder.Services.AddDbContext<DbContext, AiTestingContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("ASPNETCORE_CONNECTION_STRING");
    options.UseNpgsql(connectionString, o =>
    {
        o.EnableRetryOnFailure(
            maxRetryCount: 10,
            maxRetryDelay: TimeSpan.FromSeconds(5),
            errorCodesToAdd: null);
    });
});

builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ClockSkew = TimeSpan.Zero,
            ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
            ValidAudience = builder.Configuration["JwtSettings:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Key"]!))
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy =>
        {
            policy.WithOrigins(builder.Configuration["AllowedHosts"]!, builder.Configuration["FRONTEND_EXTERNAL_URL"]!)
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();
        });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

using(var scope = app.Services.CreateScope())
{
    await scope.ServiceProvider.GetRequiredService<DbContext>().Database.EnsureCreatedAsync();
}

var uploadsPath = Path.Combine(app.Environment.ContentRootPath, "uploads");

if (!Directory.Exists(uploadsPath))
{
    try
    {
        Directory.CreateDirectory(uploadsPath);
        Console.WriteLine($"Created missing directory: {uploadsPath}");
    }
    catch (Exception ex)
    {
        app.Logger.LogError(ex, "Failed to create uploads directory at {Path}", uploadsPath);
    }
}

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(uploadsPath),
    RequestPath = "/uploads"
});

// app.UseHttpsRedirection();
app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

await app.RunAsync();
