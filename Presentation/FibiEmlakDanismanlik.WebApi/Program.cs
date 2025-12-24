using FibiEmlakDanismanlik.Application.Interfaces;
using FibiEmlakDanismanlik.Application.Interfaces.BlogInterfaces;
using FibiEmlakDanismanlik.Application.Interfaces.PropertyInterfaces;
using FibiEmlakDanismanlik.Application.Services;
using FibiEmlakDanismanlik.Persistence.Context;
using FibiEmlakDanismanlik.Persistence.Repositories;
using FibiEmlakDanismanlik.Persistence.Repositories.BlogRepositories;
using FibiEmlakDanismanlik.Persistence.Repositories.PropertyRepositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<FibiEmlakDanismanlikContext>(options=>options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Allow Frontend Request
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhost",
        policy =>
        {
            policy.WithOrigins("https://localhost:7012") // frontend Localaddress
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

// Add services to the container.

builder.Services.AddScoped<FibiEmlakDanismanlikContext>();


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


//Configure IRepositories 
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped(typeof(IPropertyRepository), typeof(PropertyRepository));
builder.Services.AddScoped(typeof(IBlogRepository), typeof(BlogRepository));

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

app.UseAuthorization();

app.MapControllers();

app.Run();
