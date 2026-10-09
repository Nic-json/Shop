using Shop.API.Compartilhado.Infraestrutura.Dados;
using Shop.API.Modulos.Clientes.Aplicacao.DTOs;
using Shop.API.Modulos.Clientes.Dominio.Entidades;
using Microsoft.EntityFrameworkCore;

namespace Shop.API.Modulos.Clientes.Aplicacao.Servicos;

public class ClienteService
{
    private readonly ShopSysDbContext _context;

    public ClienteService(ShopSysDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> CriarAsync(
        CriarClienteRequest request,
        Guid empresaId)
    {
        if (empresaId == Guid.Empty)
            throw new ArgumentException("Empresa inválida.");

        if (string.IsNullOrWhiteSpace(request.Nome))
            throw new ArgumentException("O nome é obrigatório.");

        var cliente = new Cliente(
            empresaId,
            request.Nome.Trim(),
            request.CpfCnpj,
            request.Email,
            request.Telefone
        );

        _context.Clientes.Add(cliente);

        await _context.SaveChangesAsync();

        return cliente.Id;
    }
    public async Task<List<ClienteResponse>> ListarAsync(Guid empresaId)
    {
        return await _context.Clientes
            .AsNoTracking()
            .Where(cliente =>
                cliente.EmpresaId == empresaId &&
                cliente.Ativo)
            .Select(cliente => new ClienteResponse
            {
                Id = cliente.Id,
                Nome = cliente.Nome,
                CpfCnpj = cliente.CpfCnpj,
                Email = cliente.Email,
                Telefone = cliente.Telefone
            })
            .ToListAsync();
    }

    public async Task<ClienteResponse?> BuscarPorIdAsync(
    Guid id,
    Guid empresaId)
    {
        return await _context.Clientes
            .AsNoTracking()
            .Where(cliente =>
                cliente.Ativo &&
                cliente.Id == id &&
                cliente.EmpresaId == empresaId)
            .Select(cliente => new ClienteResponse
            {
                Id = cliente.Id,
                Nome = cliente.Nome,
                CpfCnpj = cliente.CpfCnpj,
                Email = cliente.Email,
                Telefone = cliente.Telefone
            })
            .FirstOrDefaultAsync();
    }

    public async Task<bool> AtualizarAsync(
    Guid id,
    Guid empresaId,
    AtualizarClienteRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Nome))
            throw new ArgumentException("O nome é obrigatório.");

        var cliente = await _context.Clientes
            .FirstOrDefaultAsync(c =>
                c.Id == id &&
                c.EmpresaId == empresaId);

        if (cliente == null)
            return false;

        cliente.Nome = request.Nome.Trim();
        cliente.CpfCnpj = request.CpfCnpj;
        cliente.Email = request.Email;
        cliente.Telefone = request.Telefone;

        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DesativarAsync(Guid id, Guid empresaId)
    {
        var cliente = await _context.Clientes
            .FirstOrDefaultAsync(c =>
                c.Id == id &&
                c.EmpresaId == empresaId &&
                c.Ativo);

        if (cliente == null)
            return false;

        cliente.Ativo = false;

        await _context.SaveChangesAsync();

        return true;
    }
}