
using Shop.API.Compartilhado.Infraestrutura.Dados;
using Shop.API.Compartilhado.Validacao;
using Shop.API.Modulos.Clientes.Aplicacao.DTOs;
using Shop.API.Modulos.Clientes.Dominio.Entidades;
using Microsoft.EntityFrameworkCore;
using Shop.API.Compartilhado.Validadores;

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
        var validacao = new ResultadoValidacao();

        if (empresaId == Guid.Empty)
        {
            validacao.AdicionarErro(
                "empresaId",
                "Empresa inválida."
            );
        }

        if (string.IsNullOrWhiteSpace(request.Nome))
        {
            validacao.AdicionarErro(
                "nome",
                "O nome é obrigatório."
            );
        }
        else if (request.Nome.Trim().Length < 3)
        {
            validacao.AdicionarErro(
                "nome",
                "O nome deve ter pelo menos 3 caracteres."
            );
        }

        string? documento = null;

        if (!string.IsNullOrWhiteSpace(request.CpfCnpj))
        {
            var documentoInformado = request.CpfCnpj.Trim();

            documento = CnpjValidador.Normalizar(documentoInformado);

            if (documento.Length == 11)
            {
                documento = CpfValidador.Normalizar(documentoInformado);

                if (!CpfValidador.EhValido(documento))
                {
                    validacao.AdicionarErro(
                        "cpfCnpj",
                        "O CPF informado é inválido."
                    );
                }
            }
            else if (documento.Length == 14)
            {
                if (!CnpjValidador.EhValido(documento))
                {
                    validacao.AdicionarErro(
                        "cpfCnpj",
                        "O CNPJ informado é inválido."
                    );
                }
            }
            else
            {
                validacao.AdicionarErro(
                    "cpfCnpj",
                    "Informe um CPF ou CNPJ válido."
                );
            }
        }

        if (validacao.EhValido && documento != null)
        {
            var documentoExistente = await _context.Clientes
                .AsNoTracking()
                .AnyAsync(cliente =>
                    cliente.EmpresaId == empresaId &&
                    cliente.Ativo &&
                    cliente.CpfCnpj == documento);

            if (documentoExistente)
            {
                validacao.AdicionarErro(
                    "cpfCnpj",
                    "Já existe um cliente cadastrado com este CPF/CNPJ."
                );
            }
        }

        validacao.LancarSeInvalido();

        var cliente = new Cliente(
            empresaId,
            request.Nome.Trim(),
            documento,
            request.Email,
            request.Telefone
        );

        _context.Clientes.Add(cliente);

        await _context.SaveChangesAsync();

        return cliente.Id;
    }


    public async Task<List<ClienteResponse>> ListarAsync(
        Guid empresaId)
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
        var cliente = await _context.Clientes
            .FirstOrDefaultAsync(c =>
                c.Id == id &&
                c.EmpresaId == empresaId &&
                c.Ativo);

        if (cliente == null)
            return false;

        var validacao = new ResultadoValidacao();

        // Validação do nome
        if (string.IsNullOrWhiteSpace(request.Nome))
        {
            validacao.AdicionarErro(
                "nome",
                "O nome é obrigatório."
            );
        }
        else if (request.Nome.Trim().Length < 3)
        {
            validacao.AdicionarErro(
                "nome",
                "O nome deve ter pelo menos 3 caracteres."
            );
        }

        // Validação do CPF/CNPJ
        string? documento = null;

        if (!string.IsNullOrWhiteSpace(request.CpfCnpj))
        {
            var documentoInformado = request.CpfCnpj.Trim();

            documento = CnpjValidador.Normalizar(documentoInformado);

            if (documento.Length == 11)
            {
                documento = CpfValidador.Normalizar(documentoInformado);

                if (!CpfValidador.EhValido(documento))
                {
                    validacao.AdicionarErro(
                        "cpfCnpj",
                        "O CPF informado é inválido."
                    );
                }
            }
            else if (documento.Length == 14)
            {
                if (!CnpjValidador.EhValido(documento))
                {
                    validacao.AdicionarErro(
                        "cpfCnpj",
                        "O CNPJ informado é inválido."
                    );
                }
            }
            else
            {
                validacao.AdicionarErro(
                    "cpfCnpj",
                    "Informe um CPF ou CNPJ válido."
                );
            }
        }

        // Verificação de duplicidade
        if (validacao.EhValido && documento != null)
        {
            var documentoExistente = await _context.Clientes
                .AsNoTracking()
                .AnyAsync(c =>
                    c.EmpresaId == empresaId &&
                    c.Ativo &&
                    c.Id != id &&
                    c.CpfCnpj == documento);

            if (documentoExistente)
            {
                validacao.AdicionarErro(
                    "cpfCnpj",
                    "Já existe um cliente cadastrado com este CPF/CNPJ."
                );
            }
        }

        validacao.LancarSeInvalido();

        // Atualização dos dados
        cliente.Nome = request.Nome.Trim();
        cliente.CpfCnpj = documento;
        cliente.Email = string.IsNullOrWhiteSpace(request.Email)
            ? null
            : request.Email.Trim();
        cliente.Telefone = string.IsNullOrWhiteSpace(request.Telefone)
            ? null
            : request.Telefone.Trim();

        await _context.SaveChangesAsync();

        return true;
    }


    public async Task<bool> DesativarAsync(
        Guid id,
        Guid empresaId)
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
