
using Shop.API.Modulos.Clientes.Aplicacao.DTOs;
using Shop.API.Modulos.Clientes.Aplicacao.Servicos;
using Shop.API.Modulos.Empresas.Dominio.Entidades;
using Shop.API.Tests.Infraestrutura;
using Xunit;
using Microsoft.EntityFrameworkCore;
using Shop.API.Compartilhado.Validacao;

namespace Shop.API.Tests.Clientes;

public class ClienteServiceTests : IClassFixture<PostgreSqlFixture>
{
    private readonly PostgreSqlFixture _fixture;

    public ClienteServiceTests(PostgreSqlFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task CriarAsync_QuandoNomeInvalido_DeveRejeitarCadastro()
    {
        await using var contexto = _fixture.CriarContexto();

        var service = new ClienteService(contexto);

        var request = new CriarClienteRequest
        {
            Nome = "Jo",
            CpfCnpj = null,
            Email = null,
            Telefone = null
        };

        var empresaId = Guid.NewGuid();

        var quantidadeAntes = await contexto.Clientes.CountAsync();

        await Assert.ThrowsAsync<ExcecaoValidacao>(() =>
            service.CriarAsync(request, empresaId)
        );

        var quantidadeDepois = await contexto.Clientes.CountAsync();

        Assert.Equal(quantidadeAntes, quantidadeDepois);
    }

    [Fact]
    public async Task CriarAsync_QuandoCpfDuplicado_DeveRejeitarCadastro()
    {
        await using var contexto = _fixture.CriarContexto();

        var empresa = new Empresa(
            "Empresa Teste",
            "Loja Teste",
            GerarCnpjUnicoParaTeste()
        );

        contexto.Empresas.Add(empresa);
        await contexto.SaveChangesAsync();

        var service = new ClienteService(contexto);

        var primeiroCliente = new CriarClienteRequest
        {
            Nome = "Carlos Silva",
            CpfCnpj = "52998224725"
        };

        await service.CriarAsync(primeiroCliente, empresa.Id);

        var segundoCliente = new CriarClienteRequest
        {
            Nome = "Maria Silva",
            CpfCnpj = "52998224725"
        };

        await Assert.ThrowsAsync<ExcecaoValidacao>(() =>
            service.CriarAsync(segundoCliente, empresa.Id)
        );

        var clientes = await service.ListarAsync(empresa.Id);

        Assert.Single(clientes);
    }


    [Fact]
    public async Task AtualizarAsync_QuandoMantemProprioCpf_DevePermitir()
    {
        await using var contexto = _fixture.CriarContexto();

        var empresa = new Empresa(
            "Empresa Edicao",
            "Loja Edicao",
            GerarCnpjUnicoParaTeste()
        );

        contexto.Empresas.Add(empresa);
        await contexto.SaveChangesAsync();

        var service = new ClienteService(contexto);

        var clienteId = await service.CriarAsync(
            new CriarClienteRequest
            {
                Nome = "Carlos Silva",
                CpfCnpj = "52998224725"
            },
            empresa.Id
        );

        var atualizado = await service.AtualizarAsync(
            clienteId,
            empresa.Id,
            new AtualizarClienteRequest
            {
                Nome = "Carlos Oliveira",
                CpfCnpj = "529.982.247-25"
            }
        );

        var cliente = await service.BuscarPorIdAsync(
            clienteId,
            empresa.Id
        );

        Assert.True(atualizado);
        Assert.NotNull(cliente);
        Assert.Equal("Carlos Oliveira", cliente.Nome);
        Assert.Equal("52998224725", cliente.CpfCnpj);
    }

    [Fact]
    public async Task AtualizarAsync_QuandoCpfPertenceAOutroCliente_DeveRejeitar()
    {
        await using var contexto = _fixture.CriarContexto();

        var empresa = new Empresa(
            "Empresa Duplicidade",
            "Loja Duplicidade",
            GerarCnpjUnicoParaTeste()
        );

        contexto.Empresas.Add(empresa);
        await contexto.SaveChangesAsync();

        var service = new ClienteService(contexto);

        await service.CriarAsync(
            new CriarClienteRequest
            {
                Nome = "Cliente Primeiro",
                CpfCnpj = "52998224725"
            },
            empresa.Id
        );

        var segundoClienteId = await service.CriarAsync(
            new CriarClienteRequest
            {
                Nome = "Cliente Segundo",
                CpfCnpj = "16899535009"
            },
            empresa.Id
        );

        await Assert.ThrowsAsync<ExcecaoValidacao>(() =>
            service.AtualizarAsync(
                segundoClienteId,
                empresa.Id,
                new AtualizarClienteRequest
                {
                    Nome = "Cliente Segundo Alterado",
                    CpfCnpj = "52998224725"
                }
            )
        );

        var cliente = await service.BuscarPorIdAsync(
            segundoClienteId,
            empresa.Id
        );

        Assert.NotNull(cliente);
        Assert.Equal("Cliente Segundo", cliente.Nome);
        Assert.Equal("16899535009", cliente.CpfCnpj);
    }

    [Fact]
    public async Task AtualizarAsync_QuandoClienteDesativado_DeveRetornarFalse()
    {
        await using var contexto = _fixture.CriarContexto();

        var empresa = new Empresa(
            "Empresa Desativacao",
            "Loja Desativacao",
            GerarCnpjUnicoParaTeste()
        );

        contexto.Empresas.Add(empresa);
        await contexto.SaveChangesAsync();

        var service = new ClienteService(contexto);

        var clienteId = await service.CriarAsync(
            new CriarClienteRequest
            {
                Nome = "Cliente Desativado",
                CpfCnpj = "52998224725"
            },
            empresa.Id
        );

        await service.DesativarAsync(clienteId, empresa.Id);

        var atualizado = await service.AtualizarAsync(
            clienteId,
            empresa.Id,
            new AtualizarClienteRequest
            {
                Nome = "Nome Alterado",
                CpfCnpj = "52998224725"
            }
        );

        Assert.False(atualizado);

        var clienteNoBanco = await contexto.Clientes
            .AsNoTracking()
            .SingleAsync(c => c.Id == clienteId);

        Assert.False(clienteNoBanco.Ativo);
        Assert.Equal("Cliente Desativado", clienteNoBanco.Nome);
    }

    [Fact]
    public async Task CriarAsync_QuandoCpfExisteEmOutraEmpresa_DevePermitir()
    {
        await using var contexto = _fixture.CriarContexto();

        var empresaA = new Empresa(
            "Empresa A",
            "Loja A",
            GerarCnpjUnicoParaTeste()
        );

        var empresaB = new Empresa(
            "Empresa B",
            "Loja B",
            GerarCnpjUnicoParaTeste()
        );

        contexto.Empresas.AddRange(empresaA, empresaB);
        await contexto.SaveChangesAsync();

        var service = new ClienteService(contexto);

        await service.CriarAsync(
            new CriarClienteRequest
            {
                Nome = "Cliente Empresa A",
                CpfCnpj = "52998224725"
            },
            empresaA.Id
        );

        await service.CriarAsync(
            new CriarClienteRequest
            {
                Nome = "Cliente Empresa B",
                CpfCnpj = "52998224725"
            },
            empresaB.Id
        );

        var clientesA = await service.ListarAsync(empresaA.Id);
        var clientesB = await service.ListarAsync(empresaB.Id);

        Assert.Single(clientesA);
        Assert.Single(clientesB);
        Assert.Equal(clientesA[0].CpfCnpj, clientesB[0].CpfCnpj);
    }

    private static string GerarCnpjUnicoParaTeste()
    {
        return string.Concat(
            Enumerable.Range(0, 14)
                .Select(_ => Random.Shared.Next(0, 10).ToString())
        );
    }

}
