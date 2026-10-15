
using Microsoft.EntityFrameworkCore;
using Shop.API.Compartilhado.Infraestrutura.Dados;
using Testcontainers.PostgreSql;
using Xunit;

namespace Shop.API.Tests.Infraestrutura;

public class PostgreSqlFixture : IAsyncLifetime
{
    public string ConnectionString => _container.GetConnectionString();
    private readonly PostgreSqlContainer _container =
        new PostgreSqlBuilder("postgres:17")
            .WithDatabase("shopsys_test")
            .WithUsername("test")
            .WithPassword("test")
            .Build();

    public async Task InitializeAsync()
    {
        await _container.StartAsync();

        await using var contexto = CriarContexto();

        await contexto.Database.MigrateAsync();
    }

    public async Task DisposeAsync()
    {
        await _container.DisposeAsync();
    }

    public ShopSysDbContext CriarContexto()
    {
        var options = new DbContextOptionsBuilder<ShopSysDbContext>()
            .UseNpgsql(_container.GetConnectionString())
            .Options;

        return new ShopSysDbContext(options);
    }
}
