using Microsoft.EntityFrameworkCore;
using Shop.API.Modulos.Produtos.Dominio.Entidades;
using Shop.API.Modulos.Clientes.Dominio.Entidades;

namespace Shop.API.Compartilhado.Infraestrutura.Dados;

public class ShopSysDbContext : DbContext
{
    public ShopSysDbContext(DbContextOptions<ShopSysDbContext> options)
        : base(options)
    {
    }

    public DbSet<Produto> Produtos { get; set; }
    public DbSet<Cliente> Clientes { get; set; }
}