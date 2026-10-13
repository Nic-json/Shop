using Microsoft.EntityFrameworkCore;
using Shop.API.Compartilhado.Infraestrutura.Dados;
using Shop.API.Modulos.Empresas.Aplicacao.DTOs;
using Shop.API.Modulos.Empresas.Dominio.Entidades;
using Shop.API.Compartilhado.Validadores;
using Npgsql;

namespace Shop.API.Modulos.Empresas.Aplicacao.Servicos;

public class EmpresaService
{
    private readonly ShopSysDbContext _context;

    public EmpresaService(ShopSysDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> CriarAsync(CriarEmpresaRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.RazaoSocial))
            throw new ArgumentException("A razão social é obrigatória.");

        if (string.IsNullOrWhiteSpace(request.NomeFantasia))
            throw new ArgumentException("O nome fantasia é obrigatório.");

        if (string.IsNullOrWhiteSpace(request.Cnpj))
            throw new ArgumentException("O CNPJ é obrigatório.");

        var cnpj = CnpjValidador.Normalizar(request.Cnpj);

        if (!CnpjValidador.EhValido(cnpj))
            throw new ArgumentException("O CNPJ informado é inválido.");

        var cnpjExistente = await _context.Empresas
            .AnyAsync(empresa => empresa.Cnpj == cnpj);

        if (cnpjExistente)
            throw new InvalidOperationException("Já existe uma empresa com esse CNPJ.");

        var empresa = new Empresa(
            request.RazaoSocial.Trim(),
            request.NomeFantasia.Trim(),
            cnpj
        );

        _context.Empresas.Add(empresa);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
            when (ex.InnerException is PostgresException pgEx &&
                  pgEx.SqlState == PostgresErrorCodes.UniqueViolation &&
                  pgEx.ConstraintName == "IX_Empresas_Cnpj")
        {
            throw new InvalidOperationException(
                "Já existe uma empresa com esse CNPJ.", ex);
        }

        return empresa.Id;
    }
    public async Task<List<EmpresaResponse>> ListarAsync()
    {
        return await _context.Empresas
            .AsNoTracking()
            .Where(empresa => empresa.Ativo)
            .Select(empresa => new EmpresaResponse
            {
                Id = empresa.Id,
                RazaoSocial = empresa.RazaoSocial,
                NomeFantasia = empresa.NomeFantasia,
                Cnpj = empresa.Cnpj,
                Ativo = empresa.Ativo
            })
            .ToListAsync();
    }
    public async Task<EmpresaResponse?> BuscarPorIdAsync(Guid id)
    {
        return await _context.Empresas
            .AsNoTracking()
            .Where(empresa => empresa.Id == id && empresa.Ativo)
            .Select(empresa => new EmpresaResponse
            {
                Id = empresa.Id,
                RazaoSocial = empresa.RazaoSocial,
                NomeFantasia = empresa.NomeFantasia,
                Cnpj = empresa.Cnpj,
                Ativo = empresa.Ativo
            })
            .FirstOrDefaultAsync();
    }

    public async Task<bool> AtualizarAsync(
    Guid id,
    AtualizarEmpresaRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.RazaoSocial))
            throw new ArgumentException("A razão social é obrigatória.");

        if (string.IsNullOrWhiteSpace(request.NomeFantasia))
            throw new ArgumentException("O nome fantasia é obrigatório.");

        if (string.IsNullOrWhiteSpace(request.Cnpj))
            throw new ArgumentException("O CNPJ é obrigatório.");

        var cnpj = CnpjValidador.Normalizar(request.Cnpj);

        if (!CnpjValidador.EhValido(cnpj))
            throw new ArgumentException("O CNPJ informado é inválido.");

        var empresa = await _context.Empresas
            .FirstOrDefaultAsync(e => e.Id == id && e.Ativo);

        if (empresa == null)
            return false;

        var cnpjExistente = await _context.Empresas
            .AnyAsync(e => e.Cnpj == cnpj && e.Id != id);

        if (cnpjExistente)
            throw new InvalidOperationException("Já existe outra empresa com esse CNPJ.");

        empresa.RazaoSocial = request.RazaoSocial.Trim();
        empresa.NomeFantasia = request.NomeFantasia.Trim();
        empresa.Cnpj = cnpj;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
            when (ex.InnerException is PostgresException pgEx &&
                  pgEx.SqlState == PostgresErrorCodes.UniqueViolation &&
                  pgEx.ConstraintName == "IX_Empresas_Cnpj")
        {
            throw new InvalidOperationException(
                "Já existe uma empresa com esse CNPJ.", ex);
        }

        return true;
    }

    public async Task<bool> DesativarAsync(Guid id)
    {
        var empresa = await _context.Empresas
            .FirstOrDefaultAsync(e => e.Id == id && e.Ativo);

        if (empresa == null)
            return false;

        empresa.Ativo = false;

        await _context.SaveChangesAsync();

        return true;
    }
}