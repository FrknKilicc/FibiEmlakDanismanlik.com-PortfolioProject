using FibiEmlakDanismanlik.Application.Interfaces;
using FibiEmlakDanismanlik.Application.Services;
using FibiEmlakDanismanlik.Persistence.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


//Configure IRepositories 
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

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

app.UseAuthorization();

app.MapControllers();

app.Run();
