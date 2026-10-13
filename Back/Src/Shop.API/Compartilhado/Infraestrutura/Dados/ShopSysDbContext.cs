
using Microsoft.EntityFrameworkCore;
using Shop.API.Modulos.Produtos.Dominio.Entidades;
using Shop.API.Modulos.Clientes.Dominio.Entidades;
using Shop.API.Modulos.Empresas.Dominio.Entidades;
using Shop.API.Modulos.Lojas.Dominio.Entidades;

namespace Shop.API.Compartilhado.Infraestrutura.Dados;

public class ShopSysDbContext : DbContext
{
    public ShopSysDbContext(DbContextOptions<ShopSysDbContext> options)
        : base(options)
    {
    }

    public DbSet<Empresa> Empresas { get; set; }
    public DbSet<Produto> Produtos { get; set; }
    public DbSet<Cliente> Clientes { get; set; }
    public DbSet<Loja> Lojas { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Empresa>()
            .HasIndex(empresa => empresa.Cnpj)
            .IsUnique();

        modelBuilder.Entity<Loja>()
            .HasOne<Empresa>()
            .WithMany()
            .HasForeignKey(loja => loja.EmpresaId)
            .OnDelete(DeleteBehavior.Restrict);
    }

}
