
using Microsoft.EntityFrameworkCore;
using Shop.API.Compartilhado.Infraestrutura.Dados;
using Shop.API.Modulos.Lojas.Aplicacao.DTOs;
using Shop.API.Modulos.Lojas.Dominio.Entidades;

namespace Shop.API.Modulos.Lojas.Aplicacao.Servicos;

public class LojaService
{
    private readonly ShopSysDbContext _context;

    public LojaService(ShopSysDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> CriarAsync(
        Guid empresaId,
        CriarLojaRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Nome))
            throw new ArgumentException("O nome da loja é obrigatório.");

        var empresaExiste = await _context.Empresas
            .AnyAsync(e => e.Id == empresaId && e.Ativo);

        if (!empresaExiste)
            throw new KeyNotFoundException(
                "Empresa não encontrada ou inativa.");

        var loja = new Loja(
            empresaId,
            request.Nome.Trim(),
            request.Telefone?.Trim(),
            request.Endereco?.Trim()
        );

        _context.Lojas.Add(loja);
        await _context.SaveChangesAsync();

        return loja.Id;
    }

    public async Task<List<LojaResponse>> ListarAsync(Guid empresaId)
    {
        return await _context.Lojas
            .AsNoTracking()
            .Where(l => l.EmpresaId == empresaId && l.Ativo)
            .Select(l => new LojaResponse
            {
                Id = l.Id,
                EmpresaId = l.EmpresaId,
                Nome = l.Nome,
                Telefone = l.Telefone,
                Endereco = l.Endereco,
                Ativo = l.Ativo
            })
            .ToListAsync();
    }

    public async Task<LojaResponse?> BuscarPorIdAsync(
        Guid id,
        Guid empresaId)
    {
        return await _context.Lojas
            .AsNoTracking()
            .Where(l =>
                l.Id == id &&
                l.EmpresaId == empresaId &&
                l.Ativo)
            .Select(l => new LojaResponse
            {
                Id = l.Id,
                EmpresaId = l.EmpresaId,
                Nome = l.Nome,
                Telefone = l.Telefone,
                Endereco = l.Endereco,
                Ativo = l.Ativo
            })
            .FirstOrDefaultAsync();
    }

    public async Task<bool> AtualizarAsync(
        Guid id,
        Guid empresaId,
        AtualizarLojaRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Nome))
            throw new ArgumentException("O nome da loja é obrigatório.");

        var loja = await _context.Lojas
            .FirstOrDefaultAsync(l =>
                l.Id == id &&
                l.EmpresaId == empresaId &&
                l.Ativo);

        if (loja == null)
            return false;

        loja.Nome = request.Nome.Trim();
        loja.Telefone = request.Telefone?.Trim();
        loja.Endereco = request.Endereco?.Trim();

        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DesativarAsync(Guid id, Guid empresaId)
    {
        var loja = await _context.Lojas
            .FirstOrDefaultAsync(l =>
                l.Id == id &&
                l.EmpresaId == empresaId &&
                l.Ativo);

        if (loja == null)
            return false;

        loja.Ativo = false;

        await _context.SaveChangesAsync();

        return true;
    }
}
