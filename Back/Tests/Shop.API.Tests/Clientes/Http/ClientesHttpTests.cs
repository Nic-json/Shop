
using System.Net;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using Shop.API.Modulos.Clientes.Aplicacao.DTOs;
using Shop.API.Modulos.Empresas.Dominio.Entidades;
using Shop.API.Tests.Infraestrutura;
using Xunit;
using Shop.API.Modulos.Clientes.Dominio.Entidades;

namespace Shop.API.Tests.Clientes.Http;

public class ClientesHttpTests : IClassFixture<PostgreSqlFixture>
{
    private readonly PostgreSqlFixture _fixture;

    private static readonly Guid EmpresaId =
        Guid.Parse("11111111-1111-1111-1111-111111111111");

    public ClientesHttpTests(PostgreSqlFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task Post_ClienteValido_DeveRetornar201()
    {

        await GarantirEmpresaDeTesteAsync();

        using var factory = new ShopApiFactory(
            _fixture.ConnectionString
        );

        using var client = factory.CreateClient();

        var request = new CriarClienteRequest
        {
            Nome = "Cliente HTTP Teste",
            CpfCnpj = null,
            Email = "cliente.http@teste.com"
        };

        // Act: envia uma requisição HTTP real
        var response = await client.PostAsJsonAsync(
            "/api/clientes",
            request
        );

        // Assert: verifica o código HTTP
        Assert.Equal(
            HttpStatusCode.Created,
            response.StatusCode
        );

        // Confirma que o cliente foi persistido
        await using var verificacao = _fixture.CriarContexto();

        var clienteExiste = await verificacao.Clientes
            .AsNoTracking()
            .AnyAsync(c =>
                c.EmpresaId == EmpresaId &&
                c.Email == "cliente.http@teste.com");

        Assert.True(clienteExiste);
    }


    [Fact]
    public async Task Get_ClienteExistente_DeveRetornar200()
    {
        await GarantirEmpresaDeTesteAsync();

        using var factory = new ShopApiFactory(
            _fixture.ConnectionString
        );

        using var client = factory.CreateClient();

        var clienteId = Guid.NewGuid();

        await using (var contexto = _fixture.CriarContexto())
        {
            var cliente = new Cliente(
                EmpresaId,
                "Cliente Busca HTTP",
                null,
                "busca@teste.com",
                null
            );

            clienteId = cliente.Id;

            contexto.Clientes.Add(cliente);
            await contexto.SaveChangesAsync();
        }

        var response = await client.GetAsync(
            $"/api/clientes/{clienteId}"
        );

        Assert.Equal(
            HttpStatusCode.OK,
            response.StatusCode
        );
    }

    [Fact]
    public async Task Get_ClienteInexistente_DeveRetornar404()
    {
        using var factory = new ShopApiFactory(
            _fixture.ConnectionString
        );

        using var client = factory.CreateClient();

        var clienteId = Guid.NewGuid();

        var response = await client.GetAsync(
            $"/api/clientes/{clienteId}"
        );

        Assert.Equal(
            HttpStatusCode.NotFound,
            response.StatusCode
        );
    }

    private async Task GarantirEmpresaDeTesteAsync()
    {
        await using var contexto = _fixture.CriarContexto();

        var empresaExiste = await contexto.Empresas
            .AnyAsync(e => e.Id == EmpresaId);

        if (empresaExiste)
            return;

        await contexto.Database.ExecuteSqlInterpolatedAsync(
            $"""
        INSERT INTO "Empresas"
        ("Id", "RazaoSocial", "NomeFantasia", "Cnpj", "CriadoEm", "Ativo")
        VALUES (
            {EmpresaId},
            {"Empresa HTTP Teste"},
            {"Loja HTTP Teste"},
            {"99887766000155"},
            {DateTime.UtcNow},
            {true}
        )
        """
        );
    }
}
