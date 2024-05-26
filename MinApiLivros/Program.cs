using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using MinApiLivros.Context;
using MinApiLivros.Endpoints;
using MinApiLivros.Entities;
using MinApiLivros.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddTransient<ILivroService, LivroService>();

string mysqlConnection =
    builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new
    Exception("A string de conexão 'DefaultConnection' não foi configurada");

builder.Services.AddDbContext<AppDbContext>(options =>
                    options.UseMySql(mysqlConnection,
                    ServerVersion.AutoDetect(mysqlConnection)));

builder.Services.AddIdentityApiEndpoints<IdentityUser>()
    .AddEntityFrameworkStores<AppDbContext>();

builder.Services.AddAuthorization();

var app = builder.Build();

// Configura o middleware de exceção
app.UseStatusCodePages(async statusCodeContext
    => await Results.Problem(statusCode: statusCodeContext.HttpContext.Response.StatusCode)
        .ExecuteAsync(statusCodeContext.HttpContext));

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGroup("/identity/").MapIdentityApi<IdentityUser>();

app.RegisterLivrosEndpoints();

app.Run();
