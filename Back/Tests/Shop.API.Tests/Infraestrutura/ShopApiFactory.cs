
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shop.API.Compartilhado.Infraestrutura.Dados;

namespace Shop.API.Tests.Infraestrutura;

public class ShopApiFactory : WebApplicationFactory<Program>
{
    private readonly string _connectionString;

    public ShopApiFactory(string connectionString)
    {
        _connectionString = connectionString;
    }

    protected override void ConfigureWebHost(
        IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");

        builder.ConfigureTestServices(services =>
        {
            services.AddDbContext<ShopSysDbContext>(options =>
                options.UseNpgsql(_connectionString));
        });
    }
}
