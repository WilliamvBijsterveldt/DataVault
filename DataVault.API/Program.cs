using DataVault.API.Data;
using DataVault.API.Logic;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<FileHandlerService>();
builder.Services.AddDbContext<UserDBContext>(
    o => o.UseNpgsql(builder.Configuration.GetConnectionString("DbConnection"), 
        o => o.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null )));

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