using FibiEmlakDanismanlik.Application.Interfaces;
using FibiEmlakDanismanlik.Application.Interfaces.BlogInterfaces;
using FibiEmlakDanismanlik.Application.Interfaces.ListingTypeInterfaces;
using FibiEmlakDanismanlik.Application.Interfaces.LocationInterfaces;
using FibiEmlakDanismanlik.Application.Interfaces.MapInterfaces;
using FibiEmlakDanismanlik.Application.Interfaces.NearbyInterfaces;
using FibiEmlakDanismanlik.Application.Interfaces.PropertyInterfaces;
using FibiEmlakDanismanlik.Application.Helpers;
using FibiEmlakDanismanlik.Application.Services;
using FibiEmlakDanismanlik.Persistence.Context;
using FibiEmlakDanismanlik.Persistence.Repositories;
using FibiEmlakDanismanlik.Persistence.Repositories.BlogRepositories;
using FibiEmlakDanismanlik.Persistence.Repositories.ListingTypeRepositories;
using FibiEmlakDanismanlik.Persistence.Repositories.LocationRepositories;
using FibiEmlakDanismanlik.Persistence.Repositories.MapRepositories;
using FibiEmlakDanismanlik.Persistence.Repositories.NearbyRepositories;
using FibiEmlakDanismanlik.Persistence.Repositories.PropertyRepositories;
using FibiEmlakDanismanlik.Application.Interfaces.CityGalleryInterfaces;
using FibiEmlakDanismanlik.Persistence.Repositories.CityGalleryRepositories;
using FibiEmlakDanismanlik.Application.Interfaces.AgentInterfaces;
using FibiEmlakDanismanlik.Persistence.Repositories.AgentRepositories;
using FibiEmlakDanismanlik.Application.Interfaces.BestDealInterfaces;
using FibiEmlakDanismanlik.Persistence.Repositories.BestDealRepositories;
using FibiEmlakDanismanlik.Application.Interfaces.PopularPlaceInterfaces;
using FibiEmlakDanismanlik.Persistence.Repositories.PopularPlaceRepositories;
using FibiEmlakDanismanlik.Application.Interfaces.FibiServiceInterfaces;
using FibiEmlakDanismanlik.Persistence.Repositories.FibiServiceRepositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<FibiEmlakDanismanlikContext>(options=>options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Allow Frontend Request
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhost",
        policy =>
        {
            policy.WithOrigins("https://localhost:7012", "http://localhost:7012", "http://localhost:5146", "http://localhost:5175")
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

// Add services to the container.

builder.Services.AddScoped<FibiEmlakDanismanlikContext>();


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "api/Auth/login ile alınan token'ı girin."
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" } },
            Array.Empty<string>()
        }
    });
});

// JWT: imzalama anahtarı (Jwt:Key) User Secrets'ta, Issuer/Audience appsettings'te
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(JwtTokenGenerator.GetSigningKey(builder.Configuration)),
            ClockSkew = TimeSpan.FromMinutes(1)
        };
    });


//Configure IRepositories 
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped(typeof(IPropertyRepository), typeof(PropertyRepository));
builder.Services.AddScoped(typeof(IBlogRepository), typeof(BlogRepository));
builder.Services.AddScoped(typeof(ILocationRepository), typeof(LocationRepository));
builder.Services.AddScoped(typeof(INearbyRepository), typeof(NearbyRepository));
builder.Services.AddScoped(typeof(IMapRepository), typeof(MapRepository));
builder.Services.AddScoped(typeof(ICityGalleryRepository), typeof(CityGalleryRepository));
builder.Services.AddScoped(typeof(IListingTypeRepository), typeof(ListingTypeRepository));
builder.Services.AddScoped(typeof(IAgentRepository), typeof(AgentRepository));
builder.Services.AddScoped(typeof(IBestDealRepository), typeof(BestDealRepository));
builder.Services.AddScoped(typeof(IPopularPlaceRepository), typeof(PopularPlaceRepository));
builder.Services.AddScoped(typeof(IMarketStatRepository), typeof(MarketStatRepository));
//Configure IRepositories

//MediaTR tried
builder.Services.AddApplicationService(builder.Configuration);
//MediaTR 

var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowLocalhost");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
