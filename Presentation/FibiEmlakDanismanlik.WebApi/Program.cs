using FibiEmlakDanismanlik.Application.Interfaces;
using FibiEmlakDanismanlik.Application.Interfaces.PropertyInterfaces;
using FibiEmlakDanismanlik.Application.Services;
using FibiEmlakDanismanlik.Persistence.Context;
using FibiEmlakDanismanlik.Persistence.Repositories;
using FibiEmlakDanismanlik.Persistence.Repositories.PropertyRepositories;

var builder = WebApplication.CreateBuilder(args);

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
