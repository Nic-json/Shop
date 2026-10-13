using Microsoft.EntityFrameworkCore;
using Shop.API.Compartilhado.Infraestrutura.Dados;
using Shop.API.Modulos.Clientes.Aplicacao.Servicos;
using Shop.API.Modulos.Empresas.Aplicacao.Servicos;
using Shop.API.Modulos.Lojas.Aplicacao.Servicos;

var builder = WebApplication.CreateBuilder(args);

// Controllers
builder.Services.AddControllers();

// Services
builder.Services.AddScoped<ClienteService>();
builder.Services.AddScoped<EmpresaService>();
builder.Services.AddScoped<LojaService>();

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