using Microsoft.EntityFrameworkCore;
using Shop.API.Compartilhado.Infraestrutura.Dados;
using Shop.API.Modulos.Clientes.Aplicacao.Servicos;
using Shop.API.Modulos.Empresas.Aplicacao.Servicos;
using Shop.API.Modulos.Lojas.Aplicacao.Servicos;
using Shop.API.Compartilhado.Infraestrutura;

var builder = WebApplication.CreateBuilder(args);

// Controllers
builder.Services.AddControllers();

// Services
builder.Services.AddScoped<ClienteService>();
builder.Services.AddScoped<EmpresaService>();
builder.Services.AddScoped<LojaService>();
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<TratadorExcecaoValidacao>();

// OpenAPI
builder.Services.AddOpenApi();

// Banco de dados
builder.Services.AddDbContext<ShopSysDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection")
    ));

builder.Services.AddCors(options =>
{
    options.AddPolicy("AngularDev", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseCors("AngularDev");

app.MapControllers();

app.UseExceptionHandler();

app.Run();

public partial class Program { }