using Microsoft.EntityFrameworkCore;
using Shop.API.Compartilhado.Infraestrutura.Dados;
using Shop.API.Modulos.Clientes.Aplicacao.Servicos;

var builder = WebApplication.CreateBuilder(args);

// Controllers
builder.Services.AddControllers();

// Services
builder.Services.AddScoped<ClienteService>();

// OpenAPI
builder.Services.AddOpenApi();

// Banco de dados
builder.Services.AddDbContext<ShopSysDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection")
    ));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();